using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class MoveFilterClicker : MonoBehaviour, IPointerClickHandler
    {
        public MovePicker MoveDisplay { get; set; }
        public Effects.IResourceGetter Predicate { get; set; }
        bool _filterState = false;
        TextMeshProUGUI _filterLabel;

        protected void Start() {
            _filterLabel = GetComponentInChildren<TextMeshProUGUI>();
        }
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    ToggleFilter();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void ToggleFilter() {
            _filterState = !_filterState;
            if (_filterState == true) {
                MoveDisplay.FilterIn(Predicate);
                _filterLabel.text = $"{Predicate} *";
            }
            else {
                MoveDisplay.FilterOut(Predicate);
                _filterLabel.text = $"{Predicate}";
            }
        }
    }
}