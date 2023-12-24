using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class WeaveDeleteClicker : MonoBehaviour, IPointerClickHandler
    {
        [UnityEngine.Serialization.FormerlySerializedAs("runeWeavingBank")]
        [SerializeField] RuneWeavingBank _runeWeavingBank;

        [UnityEngine.Serialization.FormerlySerializedAs("weavePicker")]
        [SerializeField] TMP_Dropdown _weavePicker;
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleDeleteWeave();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        bool HandleDeleteWeave() {
            var deletedFromBank = false;
            if (ValidWeaveName.TryParse(_weavePicker.options[_weavePicker.value].text, out var validWeaveName)) {
                deletedFromBank = _runeWeavingBank.TryDeleteWeaveByName(validWeaveName);
            }
            return deletedFromBank;
        }


    }
}