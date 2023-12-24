using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class RuneBindingSequenceClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] WeaveNavigator _weaveNavigator;
        public RuneRef RuneRef { get; set; }

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    break;
                case PointerEventData.InputButton.Right:
                    HandleRuneBindingSequenceNodeRemove();
                    break;
            }
        }

        public void HandleRuneBindingSequenceNodeRemove() {
            _weaveNavigator.TryRemoveRune(this);
        }
    }
}