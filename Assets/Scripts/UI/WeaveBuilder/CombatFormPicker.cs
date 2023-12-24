using System.Collections.Generic;
using System.Linq;
using TagFighter.Martial;
using TMPro;
using UnityEngine;

namespace TagFighter.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class CombatFormPicker : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("availableCombatForms")]
        [SerializeField] List<CombatFormRef> _availableCombatForms;

        [UnityEngine.Serialization.FormerlySerializedAs("movePicker")]
        [SerializeField] MovePicker _movePicker;
        Weapon _selectedWeapon;
        TMP_Dropdown _combatFormDropDown;

        protected void Awake() {
            _combatFormDropDown = GetComponent<TMP_Dropdown>();
            _combatFormDropDown.onValueChanged.AddListener(delegate {
                CombatFormDropDownValueChanged(_combatFormDropDown);
            });
        }
        void FillCombatFormDropDown() {
            _combatFormDropDown.ClearOptions();
            _combatFormDropDown.AddOptions(
                _selectedWeapon.GetCompatibleFormRefs().Select(combatFormRef => combatFormRef.Value)
                .Intersect(_availableCombatForms.Select(combatFormRef => combatFormRef.Value))
                .Select(combatForm => new TMP_Dropdown.OptionData(combatForm.Name))
                .ToList());
            CombatFormDropDownValueChanged(_combatFormDropDown);
        }

        public void OnSelectedWeapon(Weapon weapon) {
            _selectedWeapon = weapon;
            if (_selectedWeapon != null) {
                FillCombatFormDropDown();
            }
        }
        void CombatFormDropDownValueChanged(TMP_Dropdown change) {
            var selectedCombatFormName = change.options[change.value].text;
            var combatFormRef = _availableCombatForms.Where(combatFormRef => combatFormRef.Value.Name == selectedCombatFormName).First();
            _movePicker.OnSelectedCombatForm(_selectedWeapon, combatFormRef.Value);
        }
    }
}