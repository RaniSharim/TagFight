using TagFighter.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter
{

    public class TechBuilderClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] EventAggregator _eventAggregator;
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleTechBuilderToggle();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void HandleTechBuilderToggle() {
            _eventAggregator.OnWeaveBuilderScreenToggleSelected(this, new());
        }
    }
}
