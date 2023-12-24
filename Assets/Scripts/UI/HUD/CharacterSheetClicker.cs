using TagFighter.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter
{
    public class CharacterSheetClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] EventAggregator _eventAggregator;
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleCharacterSheetToggle();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void HandleCharacterSheetToggle() {
            _eventAggregator.OnPawnSheetToggleSelected(this, new());
        }
    }
}
