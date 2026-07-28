using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class Price : MonoBehaviour
    {
        [ShowInInspector]
        public int PriceValue{ get; private set;}

        public void InstallPrice(int configPrice)
        {
            PriceValue = configPrice;
        }
    }
}
