using TagFighter.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter.UI
{
    public class RuneWeaveSelectUI : MonoBehaviour, IPointerClickHandler
    {
        [UnityEngine.Serialization.FormerlySerializedAs("weaveName")]
        [SerializeField] TextMeshProUGUI _weaveName;

        RuneWeavingContainer _runeWeave;

        [UnityEngine.Serialization.FormerlySerializedAs("eventAggregator")]
        [SerializeField] EventAggregator _eventAggregator;

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleSelectWeave();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }

        void HandleSelectWeave() {
            _eventAggregator.OnRuneWeavingCastSelected(this, new(_runeWeave));
        }

        public void SetRuneWeave(RuneWeavingContainer runeWeave) {
            _runeWeave = runeWeave;
            SetWeaveName(_runeWeave.WeaveName);
        }

        void SetWeaveName(string weaveName) {
            this._weaveName.text = weaveName;
        }
    }
}
