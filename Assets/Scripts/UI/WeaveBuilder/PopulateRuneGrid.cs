using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class PopulateRuneGrid : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("runeImagePrefab")]
        [SerializeField] GameObject _runeImagePrefab;
        [SerializeField] WeaveNavigator _weaveNavigator;

        [UnityEngine.Serialization.FormerlySerializedAs("runeDescriptionCanvas")]
        [SerializeField] Canvas _runeDescriptionCanvas;
        RuneDescriptionUI _runeDescriptionUI;
        public RuneBank RunesBank;
        readonly List<RuneClicker> _gridItems = new();
        bool _isSelected;
        int _currentSelectedGridLocation;

        protected void Awake() {
            _runeDescriptionUI = _runeDescriptionCanvas.GetComponent<RuneDescriptionUI>();
        }

        protected void Start() {
            PopulateGrid();
            SelectFirstRune();
        }

        void PopulateGrid() {
            foreach (var (runeRef, index) in RunesBank.Runes.Select((runeRef, index) => (runeRef, index))) {
                var runeImageObject = Instantiate(_runeImagePrefab, transform);
                if (runeImageObject.TryGetComponent(out RuneClicker runeClicker) && runeImageObject.TryGetComponent(out Image runeImage)) {
                    runeImage.sprite = runeRef.RuneSprite;
                    runeClicker.GridLocation = index;
                    _gridItems.Add(runeClicker);
                    runeImageObject.SetActive(true);
                }
            }
        }

        void SelectFirstRune() {
            if (_gridItems.Count > 0) {
                OnRuneViewInfo(0);
            }
        }
        public void OnRuneViewInfo(int gridLocation) {
            Outline outline;
            if (_isSelected == true) {
                if (_gridItems[_currentSelectedGridLocation].TryGetComponent<Outline>(out outline)) {
                    outline.enabled = false;
                }
            }
            SetSelectedGridLocation(gridLocation);

            if (_gridItems[_currentSelectedGridLocation].TryGetComponent<Outline>(out outline)) {
                outline.enabled = true;
            }

            _runeDescriptionUI.OnRuneLeftClicked(RunesBank.Runes[gridLocation]);
        }

        public void OnRuneAddToSequence(int gridLocation) {
            _weaveNavigator.TryAddRune(RunesBank.Runes[gridLocation]);
        }

        void SetSelectedGridLocation(int gridLocation) {
            _isSelected = true;
            _currentSelectedGridLocation = gridLocation;
        }
    }
}