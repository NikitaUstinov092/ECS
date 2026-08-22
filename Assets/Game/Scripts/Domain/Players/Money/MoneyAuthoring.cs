using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.Players.Money
{
    public class MoneyAuthoring : MonoBehaviour
    {
        private int _startMoney = 0;
        
        public class ManaBaker : Baker<MoneyAuthoring>
        {
            public override void Bake(MoneyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new Money
                {
                    Current = authoring._startMoney
                });
            }
        }
    }
}