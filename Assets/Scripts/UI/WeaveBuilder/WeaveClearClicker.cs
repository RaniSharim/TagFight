using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class WeaveClearClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] WeaveNavigator _weaveNavigator;
        [SerializeField] TMP_InputField _weaveName;
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleClearWeave();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void HandleClearWeave() {
            _weaveNavigator.Clear();
            _weaveName.text = "";
        }

    }
}