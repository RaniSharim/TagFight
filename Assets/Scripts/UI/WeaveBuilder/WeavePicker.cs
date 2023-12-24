using System.Linq;
using TagFighter.Events;
using TMPro;
using UnityEngine;

namespace TagFighter.UI
{
    public class WeavePicker : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("watchedRuneWeavingBank")]
        [SerializeField] RuneWeavingBank _watchedRuneWeavingBank;

        [UnityEngine.Serialization.FormerlySerializedAs("eventAggregator")]
        [SerializeField] EventAggregator _eventAggregator;
        TMP_Dropdown _weaveDropDown;
        protected void Awake() {
            _weaveDropDown = GetComponent<TMP_Dropdown>();
        }

        protected void Start() {
            FillWeaveDropDown();
            _eventAggregator.RuneWeavingBankChanged += OnRuneWeavingBankChanged;
        }

        void OnRuneWeavingBankChanged(object sender, EventAggregator.RuneWeavingBankChangedEventArgs e) {
            FillWeaveDropDown();
        }

        void FillWeaveDropDown() {
            _weaveDropDown.ClearOptions();
            _weaveDropDown.AddOptions(_watchedRuneWeavingBank.GetWeaves().Select(weave => new TMP_Dropdown.OptionData(weave.WeaveName)).ToList());
        }

        protected void OnDestroy() {
            _eventAggregator.RuneWeavingBankChanged -= OnRuneWeavingBankChanged;
        }
    }
}
