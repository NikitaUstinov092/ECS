using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.MyCustom
{
    [RequireComponent(typeof(Button))]
    public class ButtonSpawnRequest : MonoBehaviour
    {
        private UnitCardData _unitCardData;
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _unitCardData = GetComponentInParent<UnitCardData>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        /// <summary>
        /// TO DO Переделать на сервис локатор
        /// </summary>
        private void OnButtonClick()
        {
            UnitSpawnRequestFactory.Instance.CreateUnitRequest(0,
                _unitCardData.Name, _unitCardData.Price,
                SpawnPointService.Instance.GetBlueRandomSpawnPoint());
        }
    }
}
