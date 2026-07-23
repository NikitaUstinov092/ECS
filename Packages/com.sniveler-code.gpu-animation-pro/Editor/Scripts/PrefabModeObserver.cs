using System;
using SnivelerCode.GpuAnimation.Editor.Utils;
using SnivelerCode.GpuAnimation.Editor.Window;
using SnivelerCode.GpuAnimation.Runtime.Authoring;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AnimatorUtils = SnivelerCode.GpuAnimation.Runtime.Utils.AnimatorUtils;

namespace SnivelerCode.GpuAnimation.Editor
{
    [InitializeOnLoad]
    public static class PrefabModeObserver
    {
        private static bool _wasDirty;
        private static GraphicsBuffer _gpuBufferDqs;
        private static GraphicsBuffer _gpuBufferLbs;

        static PrefabModeObserver()
        {
            AnimatorUtils.InitDummyBuffer();
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
        }

        private static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            GameObject root = prefabStage.prefabContentsRoot;
            var animator = root.GetComponent<AnimatorAuthoring>();
            if (animator == null) return;

            TogglePropertyBlock(animator, 1f);

            if (animator.Matrices.MatricesDqs.Length > 0)
            {
                int lengthDqs = animator.Matrices.MatricesDqs.Length;
                _gpuBufferDqs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, lengthDqs, 32);
                var tempArrayDqs = new NativeArray<DualQuaternion>(lengthDqs, Allocator.Temp);
                for (int i = 0; i < lengthDqs; i++)
                {
                    tempArrayDqs[i] = animator.Matrices.MatricesDqs[i];
                }

                _gpuBufferDqs.SetData(tempArrayDqs);
                tempArrayDqs.Dispose();

                if (_gpuBufferDqs == null || !_gpuBufferDqs.IsValid()) return;
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferDqs, _gpuBufferDqs);
            }
            else if (animator.Matrices.MatricesLbs.Length > 0)
            {
                int lengthLbs = animator.Matrices.MatricesLbs.Length;
                _gpuBufferLbs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, lengthLbs, 48);

                var tempArrayLbs = new NativeArray<float3x4>(lengthLbs, Allocator.Temp);
                for (int i = 0; i < lengthLbs; i++)
                {
                    tempArrayLbs[i] = animator.Matrices.MatricesLbs[i];
                }

                _gpuBufferLbs.SetData(tempArrayLbs);
                tempArrayLbs.Dispose();

                if (_gpuBufferLbs == null || !_gpuBufferLbs.IsValid()) return;
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferLbs, _gpuBufferLbs);
            }

            PrefabAnimatorSettingsWindow.Open(animator);
        }

        private static void OnDomainUnload(object sender, EventArgs e) => Cleanup();

        private static void OnPrefabStageClosing(PrefabStage prefabStage)
        {
            GameObject root = prefabStage.prefabContentsRoot;
            PrefabAnimatorSettingsWindow.CloseWindow();

            AnimatorUtils.SetDummyBuffer();

            _gpuBufferDqs?.Dispose();
            _gpuBufferLbs?.Dispose();

            var animator = root.GetComponent<AnimatorAuthoring>();
            if (animator == null) return;

            TogglePropertyBlock(animator, 0);

            foreach (var slot in animator.Slots)
                AssetDatabase.SaveAssetIfDirty(slot);
        }

        private static void TogglePropertyBlock(AnimatorAuthoring animator, float value)
        {
            var renderers = animator.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                var propertyBlock = new MaterialPropertyBlock();
                r.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(AnimatorShaderProperty.InstanceID, value);
                r.SetPropertyBlock(propertyBlock);
            }
        }

        private static void Cleanup()
        {
            if (_gpuBufferDqs != null)
            {
                if (_gpuBufferDqs.IsValid()) _gpuBufferDqs.Dispose();
                _gpuBufferDqs = null;
            }

            if (_gpuBufferLbs != null)
            {
                if (_gpuBufferLbs.IsValid()) _gpuBufferLbs.Dispose();
                _gpuBufferLbs = null;
            }

            AnimatorUtils.ReleaseDummyBuffer();
        }
    }
}
