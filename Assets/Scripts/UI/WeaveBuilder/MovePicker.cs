using System.Collections.Generic;
using TagFighter.Martial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class MovePicker : MonoBehaviour
    {
        public MoveClicker MovePickUIPrefab;
        public RectTransform Content;
        public WeaveNavigator WeaveNavigator;
        Weapon _selectedWeapon;
        CombatForm _selectedCombatForm;
        Image _currentSelected;
        Dictionary<Effects.IResourceGetter, List<MoveClicker>> _moveButtonCache = new();

        [SerializeField] MoveRequirementRuleset _moveRequirementRules;

        protected void Start() {
            PopulateGrid(_moveRequirementRules);
        }
        void ClearGrid() {
            foreach (Transform move in Content) {
                if (move.gameObject.activeSelf == true) {
                    Destroy(move.gameObject);
                }
            }
        }

        void PopulateGrid(Weapon weapon, CombatForm combatForm) {
            var formedMoveSet = combatForm.FormedMoveSet(weapon.GetCombatMoveSetRef());
            foreach (var move in formedMoveSet.CombatMoves()) {
                var movePickUIObject = Instantiate(MovePickUIPrefab, Content);
                var moveNameLabel = movePickUIObject.GetComponentInChildren<TextMeshProUGUI>();
                if (moveNameLabel != null) {
                    moveNameLabel.text = move.CombatMove.MoveName;
                }
                movePickUIObject.MoveRef = move;
                // if (movePickUIObject.TryGetComponent(out MoveClicker moveClicker)) {
                //     moveClicker.MoveRef = move;
                // }

                movePickUIObject.gameObject.SetActive(true);
            }
        }

        public void OnSelectedCombatForm(Weapon weapon, CombatForm combatForm) {
            _selectedCombatForm = combatForm;
            _selectedWeapon = weapon;
            if ((_selectedWeapon != null) && (_selectedCombatForm != null)) {
                ClearGrid();
                PopulateGrid(_selectedWeapon, _selectedCombatForm);
            }
        }

        public void OnSelectedMove(MoveClicker moveClicker) {
            if (_currentSelected != null) {
                _currentSelected.color = Color.white;
            }

            if (moveClicker.TryGetComponent(out _currentSelected)) {
                _currentSelected.color = Color.green;
            }
        }

        void PopulateGrid(IEnumerable<MoveRequirementRule> moveRules) {
            ClearGrid();
            _moveButtonCache.Clear();
            foreach (var moveRule in moveRules) {
                var movePickUIObject = Instantiate(MovePickUIPrefab, Content);
                var moveNameLabel = movePickUIObject.GetComponentInChildren<TextMeshProUGUI>();
                if (moveNameLabel != null) {
                    moveNameLabel.text = moveRule.Move.name;
                }
                movePickUIObject.MoveRef = moveRule.Move;

                if (!_moveButtonCache.TryGetValue(moveRule.Resource, out var predicateButtonList)) {
                    predicateButtonList = new();
                    _moveButtonCache.Add(moveRule.Resource, predicateButtonList);
                }
                predicateButtonList.Add(movePickUIObject);
            }

        }

        public void FilterIn(Effects.IResourceGetter filterBy) {
            if (_moveButtonCache.TryGetValue(filterBy, out var buttons)) {
                foreach (var movePickUIObject in buttons) {
                    movePickUIObject.gameObject.SetActive(true);
                }
            }
        }
        public void FilterOut(Effects.IResourceGetter filterBy) {
            if (_moveButtonCache.TryGetValue(filterBy, out var buttons)) {
                foreach (var movePickUIObject in buttons) {
                    movePickUIObject.gameObject.SetActive(false);
                }
            }
        }

        public void OnMoveAddToSequence(MoveClicker moveClicker) {
            WeaveNavigator.TryAddMove(moveClicker.MoveRef);
        }

    }
}