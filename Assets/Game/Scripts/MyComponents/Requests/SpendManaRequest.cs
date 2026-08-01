using Game.Scripts.MyComponents.Components;
using Unity.Entities;

namespace Game.Scripts.MyComponents.Requests
{
    public struct SpendManaRequest :  IComponentData, IEnableableComponent
    {
        public int Amount;
        public PurchaseDetails PurchaseDetails;
    }
}