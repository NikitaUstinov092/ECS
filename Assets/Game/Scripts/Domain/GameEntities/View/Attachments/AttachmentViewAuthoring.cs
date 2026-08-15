using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public class AttachmentViewAuthoring : MonoBehaviour
    {
        public class AttachmentTagBaker : Baker<AttachmentViewAuthoring>
        {
            public override void Bake(AttachmentViewAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AttachmentView>(entity);
            }
        }
    }
}