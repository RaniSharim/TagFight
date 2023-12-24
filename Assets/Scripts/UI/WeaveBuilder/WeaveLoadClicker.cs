using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class WeaveLoadClicker : MonoBehaviour, IPointerClickHandler
    {
        [UnityEngine.Serialization.FormerlySerializedAs("weaveNavigator")]
        [SerializeField] WeaveNavigator _weaveNavigator;

        [UnityEngine.Serialization.FormerlySerializedAs("runeWeavingBank")]
        [SerializeField] RuneWeavingBank _runeWeavingBank;

        [UnityEngine.Serialization.FormerlySerializedAs("weavePicker")]
        [SerializeField] TMP_Dropdown _weavePicker;

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleLoadWeave();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void HandleLoadWeave() {
            if (ValidWeaveName.TryParse(_weavePicker.options[_weavePicker.value].text, out var validWeaveName)) {
                if (_runeWeavingBank.TryGetWeaveByName(validWeaveName, out var runeWeavingContainer)) {
                    _weaveNavigator.Reload(runeWeavingContainer);
                }
            }
        }
    }
}