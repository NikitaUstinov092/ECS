using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct SpendManaRequest :  IComponentData, IEnableableComponent
    {
        public int Amount;
        public PurchaseDetails PurchaseDetails;
    }
}