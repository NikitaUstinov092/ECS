using SampleGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.MyCustom
{
    [RequireComponent(typeof(Button))]
    public class ButtonSpawnRequest : MonoBehaviour
    {
        [SerializeField]
        private TeamType _teamType = TeamType.Blue;
        
        private UnitCardData _unitCardData;
        private Button _button;
        private UnitSpawnRequestFactory _unitSpawnRequestFactory;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _unitCardData = GetComponentInParent<UnitCardData>();
            _unitSpawnRequestFactory =  GetComponentInParent<UnitSpawnRequestFactory>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
        
        private void OnButtonClick()
        {
            _unitSpawnRequestFactory.CreateUnitRequest(_teamType,
                _unitCardData.Name, _unitCardData.Price);
        }
    }
}
