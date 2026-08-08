using Game.Scripts.MyComponents.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.MyAuthorings
{
    public class MoneyAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("StartMana")] 
        public int StartMoney = 0;
        public int RegenPerSecond = 1;
    }

    public class ManaBaker : Baker<MoneyAuthoring>
    {
        public override void Bake(MoneyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Money
            {
                Current = authoring.StartMoney,
                RegenPerSecond = authoring.RegenPerSecond,
                RegenTimer = 0f
            });
        }
    }
}