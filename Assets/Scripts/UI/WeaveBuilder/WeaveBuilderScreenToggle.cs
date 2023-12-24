using System;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UI
{
    public class WeaveBuilderScreenToggle : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("eventAggregator")]
        [SerializeField] EventAggregator _eventAggregator;
        bool _show = false;
        Canvas _weaveBuilderScreen;
        protected void Awake() {
            _weaveBuilderScreen = transform.GetComponentInChildren<Canvas>(true);
        }
        protected void Start() {
            SetVisibility(_show);
            _eventAggregator.WeaveBuilderScreenToggleSelected += OnWeaveBuilderScreenToggleSelected;
        }

        protected void OnDestroy() {
            _eventAggregator.WeaveBuilderScreenToggleSelected -= OnWeaveBuilderScreenToggleSelected;
        }

        void OnWeaveBuilderScreenToggleSelected(object sender, EventArgs e) {
            _show = !_show;
            SetVisibility(_show);
        }

        void SetVisibility(bool visible) {
            _weaveBuilderScreen.gameObject.SetActive(visible);
        }
    }
}
