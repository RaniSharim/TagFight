using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class WeaveSaveClicker : MonoBehaviour, IPointerClickHandler
    {
        [UnityEngine.Serialization.FormerlySerializedAs("runeWeavingBank")]
        [SerializeField] RuneWeavingBank _runeWeavingBank;

        [UnityEngine.Serialization.FormerlySerializedAs("weaveNavigator")]
        [SerializeField] WeaveNavigator _weaveNavigator;

        [UnityEngine.Serialization.FormerlySerializedAs("weaveName")]
        [SerializeField] TMP_InputField _weaveName;

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleSaveWeave();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        bool HandleSaveWeave() {
            var addedToBank = false;
            if (ValidWeaveName.TryParse(_weaveName.text, out var validWeaveName)) {
                addedToBank = true;
                _runeWeavingBank.AddWeave(_weaveNavigator.GenerateWeave(), validWeaveName);
            }

            return addedToBank;
        }
    }
}