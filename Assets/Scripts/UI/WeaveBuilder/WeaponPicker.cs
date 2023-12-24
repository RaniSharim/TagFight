using System.Linq;
using TMPro;
using UnityEngine;

namespace TagFighter.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class WeaponPicker : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("availableWeapons")]
        [SerializeField] Inventory _availableWeapons;

        [UnityEngine.Serialization.FormerlySerializedAs("combatFormPicker")]
        [SerializeField] CombatFormPicker _combatFormPicker;
        TMP_Dropdown _weaponDropDown;
        protected void Awake() {
            _weaponDropDown = GetComponent<TMP_Dropdown>();
        }

        protected void Start() {
            FillWeaponDropDown();
            _weaponDropDown.onValueChanged.AddListener(delegate {
                WeaponDropDownValueChanged(_weaponDropDown);
            });
        }

        void FillWeaponDropDown() {
            _weaponDropDown.ClearOptions();
            _weaponDropDown.AddOptions(_availableWeapons.GetAvailableWeapons().Select(weapon => new TMP_Dropdown.OptionData(weapon.Name)).ToList());
            WeaponDropDownValueChanged(_weaponDropDown);
        }

        void WeaponDropDownValueChanged(TMP_Dropdown change) {
            var selectedWeaponName = change.options[change.value].text;
            var weapon = _availableWeapons.GetAvailableWeapons().Where(weapon => weapon.Name == selectedWeaponName).First();
            _combatFormPicker.OnSelectedWeapon(weapon);
        }
    }
}