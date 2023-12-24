using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UI
{
    public class PawnSheetToggle : MonoBehaviour
    {
        [SerializeField] EventAggregator _eventAggregator;
        bool _show = false;
        Canvas _pawnSheetScreen;

        protected void Awake() {
            _pawnSheetScreen = transform.GetComponentInChildren<Canvas>(true);
            _eventAggregator.PawnSheetToggleSelected += OnPawnSheetToggleSelected;
        }

        protected void Start() {
            SetVisibility(_show);
        }
        protected void OnDestroy() {
            _eventAggregator.PawnSheetToggleSelected -= OnPawnSheetToggleSelected;
        }

        void OnPawnSheetToggleSelected(object sender, EventAggregator.PawnSheetToggleSelectedEventArgs e) {
            _show = !_show;
            SetVisibility(_show);
        }

        void SetVisibility(bool visible) {
            _pawnSheetScreen?.gameObject.SetActive(visible);
            if (visible) {
                _eventAggregator.OnPawnSheetShowed(this, new());
            }
            else {
                _eventAggregator.OnPawnSheetHid(this, new());
            }
        }
    }

    enum PawnSheetState
    {
        Uninit,
        Hidden,
        Shown
    }
}
