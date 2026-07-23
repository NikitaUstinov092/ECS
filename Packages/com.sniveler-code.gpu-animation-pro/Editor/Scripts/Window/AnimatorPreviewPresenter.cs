using System;
using System.IO;
using System.Linq;
using SnivelerCode.GpuAnimation.Editor.Utils;
using SnivelerCode.GpuAnimation.Runtime.Authoring;
using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Editor.Window
{
    public sealed class AnimatorPreviewPresenter : IDisposable
    {
        private AnimatorAuthoring animator { get; }
        public int CurrentAnimationIndex { get; private set; }
        public int CurrentFrame { get; private set; }
        public int ActiveAttachmentIndex { get; private set; } = -1;
        public BoneOffset CurrentOffset { get; private set; } = new();

        public string[] BoneNames { get; private set; }
        public string[] AnimationNames { get; private set; }

        private Material _attachmentMaterial;
        private readonly RigidTransform _initialMatrix;
        public event Action OnStateChanged;

        public AnimatorPreviewPresenter(AnimatorAuthoring animator)
        {
            this.animator = animator;

            _initialMatrix = new RigidTransform(
                this.animator.transform.localRotation,
                this.animator.transform.localPosition);

            InitializeData();
            SetupMaterial();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            if (_attachmentMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(_attachmentMaterial);
            }
        }

        private void InitializeData()
        {
            if (animator == null) return;

            BoneNames = animator.Bones.BlobBones
                .Select(a => animator.Bones.BonesNames[a.Index])
                .ToArray();

            AnimationNames = new string[animator.Animations.Count];
            for (int i = 0; i < animator.Animations.Count; i++)
            {
                AnimationNames[i] = string.IsNullOrEmpty(animator.Animations[i].Name)
                    ? $"Anim {i}"
                    : animator.Animations[i].Name;
            }

            CurrentAnimationIndex = animator.DefaultAnimation;
            DetermineCurrentStateFromRenderer();
        }

        private void SetupMaterial()
        {
            _attachmentMaterial = AnimatorUtils.GetDebugMaterial();
        }

        private void DetermineCurrentStateFromRenderer()
        {
            var renderer = animator.GetComponentInChildren<MeshRenderer>(true);
            if (renderer == null) return;

            var pb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(pb);
            float renderFrames = pb.GetFloat(AnimatorShaderProperty.InstanceID);
            uint frameOffset = (uint) renderFrames;
            uint boneCount = (uint)animator.Bones.BonesNames.Count;

            for (int i = animator.Animations.Count - 1; i >= 0; i--)
            {
                if (frameOffset < animator.Animations[i].Start) continue;

                CurrentFrame = (int) ((frameOffset - animator.Animations[i].Start) / boneCount);
                CurrentAnimationIndex = i;
                break;
            }
        }

        public void SetAnimation(int index)
        {
            if (CurrentAnimationIndex == index) return;
            CurrentAnimationIndex = index;
            CurrentFrame = 0;
            UpdatePreview();
            OnStateChanged?.Invoke();
        }

        public void SetFrame(int frame)
        {
            if (CurrentFrame == frame) return;

            CurrentFrame = frame;
            UpdatePreview();
            OnStateChanged?.Invoke();
        }

        public void SetActiveAttachment(int index)
        {
            ActiveAttachmentIndex = index;
            if (index >= 0)
            {
                var profile = animator.Slots[index];
                CurrentOffset = profile.IsAttached
                    ? new BoneOffset {Bone = profile.DefaultBoneOffset, Index = 0}
                    : new BoneOffset {Index = -1};
            }

            OnStateChanged?.Invoke();
        }

        public void AddAttachment()
        {
            var profileAsset = ScriptableObject.CreateInstance<AttachmentProfileAsset>();
            animator.Slots.Add(profileAsset);
            ApplyChanges("Add Attachment");
            OnStateChanged?.Invoke();
        }

        public void RemoveAttachment(int index)
        {
            string assetPath = AssetDatabase.GetAssetPath(animator.Slots[index]);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            animator.Slots.RemoveAt(index);
            if (ActiveAttachmentIndex == index) ActiveAttachmentIndex = -1;
            else if (ActiveAttachmentIndex > index) ActiveAttachmentIndex--;

            ApplyChanges("Remove Attachment");
            OnStateChanged?.Invoke();
        }

        public void AddAttachmentEvent(AttachmentProfileAsset attachment, int boneIndex)
        {
            Matrix4x4 oldBoneMatrix = CurrentOffset.Bone.Index >= 0
                ? animator.Bones.BlobBones[CurrentOffset.Bone.Index]
                    .Animations[CurrentAnimationIndex]
                    .Frames[CurrentFrame].ToMatrix()
                : Matrix4x4.identity;

            Matrix4x4 newBoneMatrix = animator.Bones.BlobBones[boneIndex]
                .Animations[CurrentAnimationIndex]
                .Frames[CurrentFrame].ToMatrix();

            Matrix4x4 currentLocalMatrix = oldBoneMatrix * CurrentOffset.Bone.Offset;
            Matrix4x4 newOffsetMatrix = newBoneMatrix.inverse * currentLocalMatrix;
            attachment.EventAdd(
                CurrentAnimationIndex,
                CurrentFrame,
                new AttachmentProfileAsset.BoneOffset
                {
                    Index = boneIndex,
                    Offset = newOffsetMatrix
                });

            ApplyChanges("Add Attachment Event");
            OnStateChanged?.Invoke();
        }

        public void RemoveAttachmentEvent(AttachmentProfileAsset attachment, int eventIndex)
        {
            if (eventIndex < 0 || eventIndex >= attachment.EventsCount) return;

            attachment.EventRemove(eventIndex);
            ApplyChanges("Remove Attachment Event");
            OnStateChanged?.Invoke();
        }

        private void UpdatePreview()
        {
            if (animator == null || animator.Animations.Count <= CurrentAnimationIndex) return;

            var animation = animator.Animations[CurrentAnimationIndex];
            uint boneCount = (uint)animator.Bones.BonesNames.Count;
            uint frameOffset = (uint) (animation.Start + CurrentFrame * boneCount);

            var finalTransform = _initialMatrix;
            if (CurrentFrame < animation.RootMotionFrames.Count)
            {
                var rootMatrix = animation.RootMotionFrames[CurrentFrame];
                finalTransform = math.mul(_initialMatrix, rootMatrix);
            }

            animator.transform.localPosition = finalTransform.pos;
            animator.transform.localRotation = finalTransform.rot;

            Runtime.Utils.AnimatorUtils.SetDummyBuffer(new GpuInstanceAnimState
            {
                FrameA0 = frameOffset,
                FrameA1 = frameOffset,
                LerpA = 1f
            });

            SceneView.RepaintAll();
        }

        public void ApplyChanges(string undoName)
        {
            if (ActiveAttachmentIndex >= 0)
            {
                var profile = animator.Slots[ActiveAttachmentIndex];
                Undo.RecordObject(profile, undoName);

                switch (CurrentOffset.Index)
                {
                    case 0:
                        animator.Slots[ActiveAttachmentIndex]
                            .DefaultOffsetChange(CurrentOffset.Bone);
                        break;

                    case > 0:
                        animator.Slots[ActiveAttachmentIndex]
                            .EventChange(CurrentOffset.Index - 1, CurrentOffset.Bone);
                        break;
                }

                EditorUtility.SetDirty(profile);
            }

            Undo.RecordObject(animator, undoName);
            EditorUtility.SetDirty(animator);
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(animator.gameObject);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (animator == null || animator.Bones?.BonesNames == null) return;
            if (CurrentAnimationIndex == 0) return;

            DrawBones();
            DrawAttachmentGhost(sceneView);
        }

        private void DrawBones()
        {
            Handles.color = Color.green;
            using (new Handles.DrawingScope(animator.transform.localToWorldMatrix))
            {
                foreach (var blobBone in animator.Bones.BlobBones)
                {
                    var boneAnimation = blobBone.Animations[CurrentAnimationIndex];
                    var boneFrame = boneAnimation.Frames[CurrentFrame];
                    Vector3 localBonePosition = boneFrame.GetTranslation();

                    Handles.SphereHandleCap(0, localBonePosition, Quaternion.identity, 0.05f, EventType.Repaint);
                    Handles.BeginGUI();
                    {
                        Vector2 screenPos = HandleUtility.WorldToGUIPoint(localBonePosition);
                        Rect labelRect = new Rect(screenPos.x + 15, screenPos.y - 8, 100, 20);
                        GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
                        {
                            normal =
                            {
                                textColor = Color.white,
                                background = Texture2D.linearGrayTexture
                            },
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 10,
                            fixedWidth = 50
                        };
                        GUI.Label(labelRect, animator.Bones.BonesNames[blobBone.Index], labelStyle);
                    }
                    Handles.EndGUI();
                }
            }
        }

        private void DrawAttachmentGhost(SceneView sceneView)
        {
            if (ActiveAttachmentIndex == -1) return;

            var attachment = animator.Slots[ActiveAttachmentIndex];
            if (attachment.Prefab == null) return;

            var filter = attachment.Prefab.GetComponentInChildren<MeshFilter>();
            if (filter == null) return;

            Matrix4x4 boneWorldMatrix = CalculateBoneWorldMatrix();
            Matrix4x4 ghostWorldMatrix = boneWorldMatrix * CurrentOffset.Bone.Offset;

            Graphics.DrawMesh(
                filter.sharedMesh,
                ghostWorldMatrix,
                _attachmentMaterial,
                0,
                sceneView.camera
            );

            HandleGhostManipulation(boneWorldMatrix, ghostWorldMatrix);
        }

        private Matrix4x4 CalculateBoneWorldMatrix()
        {
            if (CurrentOffset.Bone.Index >= 0)
            {
                var matrix = animator.Bones.BlobBones[CurrentOffset.Bone.Index]
                    .Animations[CurrentAnimationIndex]
                    .Frames[CurrentFrame];

                float4x4 local = animator.transform.localToWorldMatrix;
                return math.mul(local, matrix.ToMatrix());
            }

            return Matrix4x4.identity;
        }

        private void HandleGhostManipulation(Matrix4x4 boneWorldMatrix, Matrix4x4 ghostWorldMatrix)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 worldPos = ghostWorldMatrix.GetPosition();
            Quaternion worldRot = ghostWorldMatrix.rotation;
            switch (Tools.current)
            {
                case Tool.Move:
                    worldPos = Handles.PositionHandle(worldPos, worldRot);
                    break;
                case Tool.Rotate:
                    worldRot = Handles.RotationHandle(worldRot, worldPos);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Matrix4x4 newGhostWorldMatrix = Matrix4x4.TRS(worldPos, worldRot, Vector3.one);
                CurrentOffset.Bone.Offset = boneWorldMatrix.inverse * newGhostWorldMatrix;
                ApplyChanges("Move Attachment");
            }
        }

        public void UpdateAttachmentPrefab(int index, GameObject newPrefab)
        {
            var profileAsset = animator.Slots[index];
            if (profileAsset.Prefab == newPrefab) return;

            string assetPath = AssetDatabase.GetAssetPath(profileAsset);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                profileAsset = ScriptableObject.CreateInstance<AttachmentProfileAsset>();
                animator.Slots[index] = profileAsset;
            }

            ApplyChanges("Change Attachment Prefab");
            profileAsset.PrefabChange(newPrefab);

            if (newPrefab != null)
            {
                string assetRootPath = AssetDatabase.GetAssetPath(animator.Matrices);
                if (!string.IsNullOrEmpty(assetRootPath))
                {
                    string directoryName = Path.GetDirectoryName(assetRootPath);
                    AssetDatabase.CreateAsset(profileAsset, Path.Combine(directoryName, newPrefab.name + ".asset"));
                }
            }

            OnStateChanged?.Invoke();
        }

        public void ChangeOffset(BoneOffset boneOffset) => CurrentOffset = boneOffset;

        public void PasteOffsetKeepingWorldPosition(Matrix4x4 matrix, int boneIndex)
        {
            if (boneIndex < 0 || CurrentOffset.Bone.Index < 0) return;

            Matrix4x4 sourceBoneMatrix = animator.Bones.BlobBones[boneIndex]
                .Animations[CurrentAnimationIndex]
                .Frames[CurrentFrame].ToMatrix();

            Matrix4x4 targetBoneMatrix = animator.Bones.BlobBones[CurrentOffset.Bone.Index]
                .Animations[CurrentAnimationIndex]
                .Frames[CurrentFrame].ToMatrix();

            Matrix4x4 itemRootMatrix = sourceBoneMatrix * matrix;
            Matrix4x4 newOffsetMatrix = targetBoneMatrix.inverse * itemRootMatrix;
            CurrentOffset.Bone.Offset = newOffsetMatrix;

            ApplyChanges("Paste Matrix Keeping Position");
            OnStateChanged?.Invoke();
        }

        public sealed class BoneOffset
        {
            public int Index = -1;

            public AttachmentProfileAsset.BoneOffset Bone = new()
            {
                Index = -1,
                Offset = Matrix4x4.identity
            };
        }
    }
}
