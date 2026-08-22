using Unity.Entities;

namespace Game.Scripts.Domain.Players.Money
{
    public struct SpendMoneyRequest :  IComponentData, IEnableableComponent
    {
        public int Amount;
        public PurchaseDetails PurchaseDetails;
    }
}