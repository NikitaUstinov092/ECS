using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.View.Attachments
{
    public sealed class AttacherViewHolderAuthoring : MonoBehaviour
    {
        public sealed class Baker : Baker<AttacherViewHolderAuthoring>
        {
            public override void Bake(AttacherViewHolderAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AttacherView>(entity);
            }
        }
    }
}