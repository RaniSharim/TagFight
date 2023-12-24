using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    [RequireComponent(typeof(Image))]
    public class PlannedActionStatus : MonoBehaviour
    {
        Image _status;
        IActionRead _watched;
        protected void Start() {
            _status = GetComponent<Image>();
        }

        protected void Update() {
            if (_watched != null) {
                _status.fillAmount = _watched.CompletionStatus();
            }
        }
        public void SetWatched(IActionRead action) {
            _watched = action;
            if (_watched == null) {
                _status.fillAmount = 0;
            }
            else {
                _status.fillAmount = _watched.CompletionStatus();
            }
        }
    }
}
