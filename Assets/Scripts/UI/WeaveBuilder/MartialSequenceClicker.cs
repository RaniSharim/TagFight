using TagFighter.Martial;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class MartialSequenceClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] WeaveNavigator _weaveNavigator;
        public CombatMoveRef CombatMoveRef { get; set; }

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    break;
                case PointerEventData.InputButton.Right:
                    HandleMartialSequenceNodeRemove();
                    break;
            }
        }

        public void HandleMartialSequenceNodeRemove() {
            _weaveNavigator.TryRemoveMove(this);
        }
    }
}