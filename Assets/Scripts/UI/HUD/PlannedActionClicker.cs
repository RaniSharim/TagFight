using TagFighter.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class PlannedActionClicker : MonoBehaviour, IPointerClickHandler
    {

        public int Index { get; set; }
        TMP_Text _actionName;
        [SerializeField] EventAggregator _eventAggregator;
        protected void Awake() {
            _actionName = GetComponentInChildren<TMP_Text>();
        }
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    break;
                case PointerEventData.InputButton.Right:
                    HandleRemoveFromPlannedActions();
                    break;
            }
        }

        public void SetActionName(string actionName) {
            _actionName.text = actionName;
        }

        void HandleRemoveFromPlannedActions() {
            _eventAggregator.OnRemovePlannedActionSelected(this, new(Index));
        }
    }
}
