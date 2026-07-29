using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class UnitCardData : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        public int Price{ get; private set;}
        
        [ShowInInspector, ReadOnly]
        public string Name{ get; private set;}

        public void InstallPrice(int configPrice)
        {
            Price = configPrice;
        }

        public void InstallName(string configName)
        {
            Name = configName;
        }
    }
}
