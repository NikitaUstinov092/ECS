using Game.Scripts.Common.Team;
using Unity.Collections;

namespace Game.Scripts.Domain.Players.Money
{
    public struct PurchaseDetails
    {
        public TeamType Team;
        public FixedString32Bytes UnitName;
    }
}