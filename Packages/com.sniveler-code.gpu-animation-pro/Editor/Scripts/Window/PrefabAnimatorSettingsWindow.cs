using System.Collections.Generic;
using SnivelerCode.GpuAnimation.Runtime.Authoring;
using SnivelerCode.GpuAnimation.Runtime.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SnivelerCode.GpuAnimation.Editor.Window
{
    public sealed class PrefabAnimatorSettingsWindow : EditorWindow
    {
        private AnimatorAuthoring _animator;
        private AnimatorPreviewPresenter _presenter;

        private SliderInt _frameSlider;
        private PopupField<string> _animationPopup;
        private VisualElement _attachmentsList;
        private VisualElement _activeDetails;
        private Matrix4x4 _copiedMatrix = Matrix4x4.identity;
        private int _copiedBoneIndex = -1;

        private const string _guiPath = "Packages/com.sniveler-code.gpu-animation-pro/Editor/Gui/";
        private const string _mainUxml = _guiPath + "PrefabAnimatorSettings.uxml";
        private const string _itemTemplate = _guiPath + "AttachmentItemTemplate.uxml";
        private const string _defaultStateTemplate = _guiPath + "AttachmentDefaultStateTemplate.uxml";
        private const string _eventItemTemplate = _guiPath + "AttachmentEventItemTemplate.uxml";
        private const string _addEventTemplate = _guiPath + "AddAttachmentEventTemplate.uxml";

        public static void Open(AnimatorAuthoring animator)
        {
            var window = GetWindow<PrefabAnimatorSettingsWindow>("Prefab Animation Settings");
            window._animator = animator;
            window.InitializePresenter();
            window.Show();
        }

        public static void CloseWindow()
        {
            if (HasOpenInstances<PrefabAnimatorSettingsWindow>())
            {
                GetWindow<PrefabAnimatorSettingsWindow>().Close();
            }
        }

        private void OnEnable()
        {
            if (_animator != null && _presenter == null)
            {
                InitializePresenter();
            }
        }

        private void OnDisable()
        {
            _presenter?.Dispose();
            _presenter = null;
        }

        private void InitializePresenter()
        {
            _presenter?.Dispose();
            _presenter = new AnimatorPreviewPresenter(_animator);
            _presenter.OnStateChanged += RefreshUI;
            rootVisualElement.Clear();
            CreateGUI();
        }

        public void CreateGUI()
        {
            if (_animator == null)
            {
                rootVisualElement.Add(new Label("No AnimatorAuthoring found. Please open from a prefab."));
                return;
            }

            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_mainUxml);
            if (uxml == null)
            {
                rootVisualElement.Add(new Label($"UXML not found at {_mainUxml}"));
                return;
            }

            rootVisualElement.Add(uxml.CloneTree());
            BindStaticElements(rootVisualElement);
            RefreshUI();
        }

        private void BindStaticElements(VisualElement root)
        {
            var animContainer = root.Q<VisualElement>("animation-container");
            _animationPopup = new PopupField<string>("Animation", new List<string>(_presenter.AnimationNames),
                _presenter.CurrentAnimationIndex);
            _animationPopup.RegisterValueChangedCallback(_ => _presenter.SetAnimation(_animationPopup.index));
            animContainer.Add(_animationPopup);

            _frameSlider = root.Q<SliderInt>("frame-slider");
            _frameSlider.RegisterValueChangedCallback(evt => _presenter.SetFrame(evt.newValue));

            _attachmentsList = root.Q<VisualElement>("attachments-list");
            root.Q<Button>("add-attachment-btn").clicked += _presenter.AddAttachment;

            _activeDetails = root.Q<VisualElement>("active-attachment-details");
        }

        private void RefreshUI()
        {
            if (_presenter == null) return;

            var currentAnim = _animator.Animations[_presenter.CurrentAnimationIndex];
            _frameSlider.highValue = currentAnim.Frames - 1;
            _frameSlider.SetValueWithoutNotify(_presenter.CurrentFrame);
            _animationPopup.SetValueWithoutNotify(_presenter.AnimationNames[_presenter.CurrentAnimationIndex]);

            RefreshAttachmentsList();
            RefreshActiveDetails();
        }

        private void RefreshAttachmentsList()
        {
            _attachmentsList.Clear();
            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_itemTemplate);
            if (template == null) return;

            for (int i = 0; i < _animator.Slots.Count; i++)
            {
                int index = i;
                var profile = _animator.Slots[i];
                var item = template.CloneTree();

                var objField = item.Q<ObjectField>("attachment-prefab-field");
                objField.label = $"Attachment {i}";
                objField.value = profile?.Prefab;
                objField.RegisterValueChangedCallback(evt =>
                    _presenter.UpdateAttachmentPrefab(index, (GameObject) evt.newValue));

                var showBtn = item.Q<Button>("show-hide-button");
                showBtn.text = _presenter.ActiveAttachmentIndex == index ? "hide" : "show";
                showBtn.clicked += () => _presenter.SetActiveAttachment(
                    _presenter.ActiveAttachmentIndex == index ? -1 : index);

                item.Q<Button>("remove-button").clicked += () => _presenter.RemoveAttachment(index);

                _attachmentsList.Add(item);
            }
        }

        private void RefreshActiveDetails()
        {
            if (_presenter.ActiveAttachmentIndex == -1)
            {
                _activeDetails.style.display = DisplayStyle.None;
                return;
            }

            _activeDetails.style.display = DisplayStyle.Flex;
            var attachment = _animator.Slots[_presenter.ActiveAttachmentIndex];

            RefreshDefaultState(attachment);
            RefreshEventsList(attachment);
            RefreshAddEventSection(attachment);
        }

        private void RefreshDefaultState(AttachmentProfileAsset attachment)
        {
            var container = _activeDetails.Q<VisualElement>("default-state-container");
            container.Clear();

            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_defaultStateTemplate);
            if (template == null) return;

            var root = template.CloneTree();

            root.style.backgroundColor = GetBackground(0);
            root.Q<Label>("default-state-label").text =
                $"Default State: {(attachment.IsAttached ? "Attached" : "Empty")}";

            var boneLabel = root.Q<Label>("bone-label");
            var detachBtn = root.Q<Button>("detach-button");
            var radioBtn = root.Q<RadioButton>();
            radioBtn.value = _presenter.CurrentOffset.Index == 0;

            var attachContainer = root.Q<VisualElement>("attach-container");
            if (attachment.IsAttached)
            {
                if (_presenter.CurrentOffset.Index != 0)
                {
                    radioBtn.RegisterValueChangedCallback(evt =>
                    {
                        if (!evt.newValue) return;
                        _presenter.ChangeOffset(new AnimatorPreviewPresenter.BoneOffset
                        {
                            Index = 0,
                            Bone = new AttachmentProfileAsset.BoneOffset
                            {
                                Index = attachment.DefaultBoneOffset.Index,
                                Offset = attachment.DefaultBoneOffset.Offset
                            }
                        });

                        RefreshUI();
                    });
                }

                boneLabel.style.display = DisplayStyle.Flex;
                boneLabel.text = $"Bone: {_presenter.BoneNames[attachment.DefaultBoneOffset.Index]}";

                detachBtn.style.display = DisplayStyle.Flex;
                detachBtn.clicked += () =>
                {
                    attachment.DefaultOffsetChange(new AttachmentProfileAsset.BoneOffset
                    {
                        Index = -1,
                        Offset = Matrix4x4.identity
                    });

                    _presenter.SetActiveAttachment(_presenter.ActiveAttachmentIndex);
                    _presenter.ApplyChanges("Detach");
                    RefreshUI();
                };
            }
            else
            {
                attachContainer.style.display = DisplayStyle.Flex;
                var bonePopup = new PopupField<string>(new List<string>(_presenter.BoneNames), 0);
                attachContainer.Q<VisualElement>("bone-popup-container").Add(bonePopup);
                attachContainer.Q<Button>("attach-button").clicked += () =>
                {
                    attachment.DefaultOffsetChange(new AttachmentProfileAsset.BoneOffset
                    {
                        Index = bonePopup.index,
                        Offset = Matrix4x4.identity
                    });

                    _presenter.SetActiveAttachment(_presenter.ActiveAttachmentIndex);
                    _presenter.ApplyChanges("Attach");
                    RefreshUI();
                };
            }

            container.Add(root);
        }

        private void RefreshEventsList(AttachmentProfileAsset attachment)
        {
            var container = _activeDetails.Q<VisualElement>("events-container");
            container.Clear();

            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_eventItemTemplate);
            if (template == null) return;

            for (int i = 0; i < attachment.EventsCount; i++)
            {
                int eventIndex = i;
                var @event = attachment.EventGet(i);
                var item = template.CloneTree();

                var animatorAnimation = _animator.Animations[@event.AnimationIndex];
                item.style.backgroundColor = GetBackground(eventIndex + 1);
                item.Q<Label>("anim-label").text = $"anim: {animatorAnimation.Name}";
                item.Q<Label>("frame-label").text = $"frame: {@event.TriggerFrame}";
                item.Q<Label>("bone-label").text = $"bone: {_presenter.BoneNames[@event.BoneOffset.Index]}";
                item.Q<Button>("remove-event-button").clicked +=
                    () => _presenter.RemoveAttachmentEvent(attachment, eventIndex);

                var radioBtn = item.Q<RadioButton>();
                radioBtn.value = _presenter.CurrentOffset.Index == eventIndex + 1;
                radioBtn.RegisterValueChangedCallback(evt =>
                {
                    if (!evt.newValue) return;
                    _presenter.ChangeOffset(new AnimatorPreviewPresenter.BoneOffset
                    {
                        Index = eventIndex + 1,
                        Bone = new AttachmentProfileAsset.BoneOffset
                        {
                            Index = @event.BoneOffset.Index,
                            Offset = @event.BoneOffset.Offset
                        }
                    });

                    RefreshUI();
                });

                container.Add(item);
            }
        }

        private Color GetBackground(int index)
        {
            string selectedColor = _presenter.CurrentOffset.Index == index ? "#3B862E0A" : "#0000000A";
            return ColorUtility.TryParseHtmlString(selectedColor, out var color) ? color : Color.clear;
        }

        private void RefreshAddEventSection(AttachmentProfileAsset attachment)
        {
            var container = _activeDetails.Q<VisualElement>("add-event-container");
            container.Clear();

            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_addEventTemplate);
            if (template == null) return;

            var root = template.CloneTree();
            root.Q<Label>("current-frame-label").text = $"Frame: {_presenter.CurrentFrame}";

            var bonePopup = new PopupField<string>(new List<string>(_presenter.BoneNames), 0);
            root.Q<VisualElement>("bone-popup-container").Add(bonePopup);

            root.Q<Button>("add-event-button").clicked += () =>
                _presenter.AddAttachmentEvent(attachment, bonePopup.index);

            root.Q<Button>("copy-matrix").clicked += () =>
            {
                _copiedMatrix = _presenter.CurrentOffset.Bone.Offset;
                _copiedBoneIndex = _presenter.CurrentOffset.Bone.Index;
                Debug.Log("Matrix Copied!");
            };

            root.Q<Button>("paste-matrix").clicked += () =>
            {
                if (_copiedBoneIndex == -1) return;
                _presenter.PasteOffsetKeepingWorldPosition(_copiedMatrix, _copiedBoneIndex);
            };

            container.Add(root);
        }
    }
}
