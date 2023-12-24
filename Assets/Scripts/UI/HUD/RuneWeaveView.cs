using System.Linq;
using TagFighter.Equipment;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UI
{
    public class RuneWeaveView : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("weaveSelectButtonTemplate")]
        public Transform WeaveSelectButtonTemplate;

        [UnityEngine.Serialization.FormerlySerializedAs("watchedRuneWeavingBank")]
        public RuneWeavingBank WatchedRuneWeavingBank;

        [UnityEngine.Serialization.FormerlySerializedAs("eventAggregator")]
        [SerializeField] EventAggregator _eventAggregator;
        Outfit _watchedOutfit;
        Weaver _selectedWeaver;
        Weaver SelectedWeaver {
            get {
                return _selectedWeaver;
            }
            set {
                if (_selectedWeaver != null) {
                    _eventAggregator.RuneWeavingBankChanged -= OnRuneWeavingBankChanged;
                    if (_watchedOutfit != null) {
                        _watchedOutfit.OutfitChanged -= OnOutfitChanged;
                    }
                }
                _selectedWeaver = value;

                if (_selectedWeaver != null) {
                    _eventAggregator.RuneWeavingBankChanged += OnRuneWeavingBankChanged;
                    if (SelectedWeaver.TryGetComponent(out _watchedOutfit)) {
                        _watchedOutfit.OutfitChanged += OnOutfitChanged;
                    }
                }
            }
        }


        protected void Start() {
            Clear();
            _eventAggregator.UnitSelected += OnUnitSelected;
        }

        protected void OnDestroy() {
            SelectedWeaver = null;
            _eventAggregator.UnitSelected -= OnUnitSelected;
            Clear();
        }

        void OnRuneWeavingBankChanged(object sender, EventAggregator.RuneWeavingBankChangedEventArgs e) {
            if (WatchedRuneWeavingBank == e.Bank) {
                CreateWeaveSelectionButtons(WatchedRuneWeavingBank, SelectedWeaver);
            }
        }
        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if ((e.UnitTransform.GetComponent<PartyMember>() != null) && e.UnitTransform.TryGetComponent<Weaver>(out var weaver)) {
                SelectedWeaver = weaver;

                CreateWeaveSelectionButtons(WatchedRuneWeavingBank, SelectedWeaver);
            }
        }

        void OnOutfitChanged(object sender, OutfitChangedEventArgs e) {
            CreateWeaveSelectionButtons(WatchedRuneWeavingBank, SelectedWeaver);
        }

        void CreateWeaveSelectionButtons(RuneWeavingBank bank, Weaver weaver) {
            var currentWeaveIndex = 0;
            var originalChildCount = transform.childCount - 1; // excluding template
            foreach (var weave in bank.GetWeaves().Where(weave => weaver.CanUseWeave(weave.RuneWeaving))) {
                RuneWeaveSelectUI runeWeaveSelectUI;
                if (currentWeaveIndex < originalChildCount) {
                    runeWeaveSelectUI = transform.GetChild(currentWeaveIndex + 1).GetComponent<RuneWeaveSelectUI>();
                }
                else {
                    runeWeaveSelectUI = Instantiate(WeaveSelectButtonTemplate, transform).GetComponent<RuneWeaveSelectUI>();
                }
                runeWeaveSelectUI.SetRuneWeave(weave);
                runeWeaveSelectUI.gameObject.SetActive(true);
                currentWeaveIndex++;
            }

            for (; currentWeaveIndex < transform.childCount - 1; currentWeaveIndex++) {
                transform.GetChild(currentWeaveIndex + 1).gameObject.SetActive(false);
            }
        }
        void Clear() {
            // Making sure to skip the template
            foreach (Transform item in transform) {
                if (item.GetComponent<RuneWeaveSelectUI>() != null) {
                    Destroy(item.gameObject);
                }
            }
        }



    }
}
