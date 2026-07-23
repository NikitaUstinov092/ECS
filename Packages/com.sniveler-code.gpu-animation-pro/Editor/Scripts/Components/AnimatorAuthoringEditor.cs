#if UNITY_EDITOR
using SnivelerCode.GpuAnimation.Editor.Utils;
using SnivelerCode.GpuAnimation.Editor.Window;
using SnivelerCode.GpuAnimation.Runtime.Authoring;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if !SC_GPU_ANIMATION_DEBUG
using System.Linq;
#endif

namespace SnivelerCode.GpuAnimation.Editor.Components
{
    [CustomEditor(typeof(AnimatorAuthoring))]
    public sealed class AnimatorAuthoringEditor : UnityEditor.Editor
    {
        private Material _material;

        private void OnSceneGUI()
        {
            var animator = (AnimatorAuthoring) target;
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(animator.gameObject);
            if (prefabStage != null) return;

            var lodGroup = animator.GetComponent<LODGroup>();
            if (lodGroup == null) return;

            var lods = lodGroup.GetLODs();
            if (lods.Length == 0) return;
            var lod0 = lods[0];

            foreach (var renderer in lod0.renderers)
            {
                if (renderer == null) continue;
                Mesh mesh = null;
                switch (renderer)
                {
                    case SkinnedMeshRenderer smr:
                        mesh = smr.sharedMesh;
                        break;

                    case MeshRenderer mr:
                    {
                        if (mr.TryGetComponent<MeshFilter>(out var filter))
                        {
                            mesh = filter.sharedMesh;
                        }

                        break;
                    }
                }

                if (mesh == null) continue;

                _material ??= AnimatorUtils.GetDebugMaterial();
                _material.SetPass(0);

                GL.wireframe = true;
                Graphics.DrawMeshNow(mesh, renderer.transform.localToWorldMatrix);
                GL.wireframe = false;
            }
        }

        public override void OnInspectorGUI()
        {
            var animator = (AnimatorAuthoring) target;

#if SC_GPU_ANIMATION_DEBUG
            base.OnInspectorGUI();
#else
            GUI.enabled = false;
            var bones = animator.Bones.BlobBones
                .Select(b => animator.Bones.BonesNames[b.Index])
                .Distinct();
            serializedObject.Update();
            EditorGUILayout.TextField("BakedBones", string.Join(",", bones));
            EditorGUILayout.TextField("Animations", animator.Animations.Count.ToString());
            EditorGUILayout.TextField($"Parameters", animator.Parameters.Count.ToString());
            EditorGUILayout.TextField("DefaultAnimation", animator.Animations[animator.DefaultAnimation].Name);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AnimatorAuthoring.Matrices)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AnimatorAuthoring.Slots)));

            serializedObject.ApplyModifiedProperties();
#endif

            if (EditorWindow.HasOpenInstances<PrefabAnimatorSettingsWindow>()) return;

            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(animator.gameObject);
            if (prefabStage == null)
            {
                if (!GUILayout.Button("Edit Sockets")) return;

                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(animator.gameObject);
                GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(animator.gameObject);
                if (!string.IsNullOrEmpty(assetPath) && root != null)
                    PrefabStageUtility.OpenPrefab(assetPath, root, PrefabStage.Mode.InContext);
            }
            else
            {
                if (GUILayout.Button("Sockets Window"))
                    PrefabAnimatorSettingsWindow.Open(animator);
            }
        }
    }
}
#endif
