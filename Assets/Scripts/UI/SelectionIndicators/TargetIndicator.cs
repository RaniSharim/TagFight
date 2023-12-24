using TagFighter.Events;
using UnityEngine;

namespace TagFighter
{
    public class TargetIndicator : MonoBehaviour
    {
        [SerializeField] Transform _selectionUI;
        // [SerializeField] float scale = 1.25f;
        Transform _selected;
        [SerializeField] EventAggregator _eventAggregator;

        protected void Awake() {
            _selectionUI.gameObject.SetActive(false);
        }
        protected void OnDestroy() {
            _eventAggregator.TargetSelected -= OnTargetSelected;
        }
        protected void Update() {
            SetUnitVisual(_selected);
        }
        protected void Start() {
            _eventAggregator.TargetSelected += OnTargetSelected;
        }

        void OnTargetSelected(object sender, UnitActionEventArgs e) {
            SetSelectedTransform(e.UnitTransform);
        }

        void SetSelectedTransform(Transform selected) {
            _selected = selected;
            _selectionUI.gameObject.SetActive(true);
            SetUnitVisual(_selected);
        }


        void SetUnitVisual(Transform selected) {
            if (selected) {
                var bounds = selected.GetComponent<Renderer>().bounds;
                transform.position = new Vector3(bounds.center.x, 0, bounds.center.z);
                //this.transform.localScale = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z) * scale;
            }
        }

    }
}
