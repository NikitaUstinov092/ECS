using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    [Serializable]
    public sealed class AttachmentProfileAsset : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private BoneOffset defaultBoneOffset = new() {Index = -1, Offset = Matrix4x4.identity};
        [SerializeField] private List<Event> events = new();

        public GameObject Prefab => prefab;
        public BoneOffset DefaultBoneOffset => defaultBoneOffset;
        public bool IsAttached => defaultBoneOffset.Index >= 0;
        public int EventsCount => events.Count;

        [Serializable]
        public struct BoneOffset
        {
            public int Index;
            public Matrix4x4 Offset;
        }

        [Serializable]
        public struct Event
        {
            public int AnimationIndex;
            public int TriggerFrame;
            public BoneOffset BoneOffset;
        }

        public void DefaultOffsetChange(BoneOffset offset)
        {
            defaultBoneOffset = offset;
        }

        public void EventChange(int index, BoneOffset offset) =>
            events[index] = new Event
            {
                BoneOffset = offset,
                AnimationIndex = events[index].AnimationIndex,
                TriggerFrame = events[index].TriggerFrame
            };

        public void EventAdd(int animation, int frame, BoneOffset offset)
        {
            events.Add(new Event
            {
                AnimationIndex = animation,
                TriggerFrame = frame,
                BoneOffset = offset
            });
        }

        public void EventRemove(int index) => events.RemoveAt(index);

        public void PrefabChange(GameObject value) => prefab = value;

        public List<Event> EventsByAnimation(int animation) =>
            events.Where(e => e.AnimationIndex == animation).ToList();

        public Event EventGet(int i) => events[i];
    }
}
