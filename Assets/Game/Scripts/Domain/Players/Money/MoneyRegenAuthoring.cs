using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.Players.Money
{
    public class MoneyRegenAuthoring : MonoBehaviour
    {
        [SerializeField] private int regenCountPerRate = 1;
        [SerializeField] private int secondsRate = 1;

        public class Baker : Baker<MoneyRegenAuthoring>
        {
            public override void Bake(MoneyRegenAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MoneyRegen
                {
                    RegenCountPerRate = authoring.regenCountPerRate,
                    SecondsRate = authoring.secondsRate,
                    RegenTimer = 0f
                });
            }
        }
    }
}