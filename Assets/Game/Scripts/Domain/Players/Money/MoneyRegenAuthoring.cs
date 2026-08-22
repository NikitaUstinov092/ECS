using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.Players.Money
{
    public class MoneyRegenAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("regenCountPerRate")] 
        [SerializeField] private int _regenCountPerRate = 1;
        [FormerlySerializedAs("secondsRate")] 
        [SerializeField] private int _secondsRate = 1;

        public class Baker : Baker<MoneyRegenAuthoring>
        {
            public override void Bake(MoneyRegenAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MoneyRegen
                {
                    RegenCountPerRate = authoring._regenCountPerRate,
                    SecondsRate = authoring._secondsRate,
                    RegenTimer = 0f
                });
            }
        }
    }
}