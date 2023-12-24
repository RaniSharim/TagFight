using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Equipment;
using UnityEngine;

namespace TagFighter
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Game/Equipment/Inventory")]
    public class Inventory : ScriptableObject, IEnumerable<Item>
    {
        [SerializeField] List<Item> _availableItems;
        [SerializeField] List<WeaponRef> _availableWeapons;
        Dictionary<string, Weapon> _availableWeaponsByName;

        protected void OnEnable() {
            if (_availableWeapons?.Any() == true) {
                _availableWeaponsByName = _availableWeapons.Distinct().ToDictionary(weaponRef => weaponRef.Value.Name, weaponRef => weaponRef.Value);
            }
        }

        protected void OnDisable() {
            _availableWeaponsByName?.Clear();
        }

        public bool TryGetWeaponByName(string weaponName, out Weapon weapon) {
            var isWeaponInInventory = false;
            weapon = null;
            if (_availableWeaponsByName != null) {
                isWeaponInInventory = _availableWeaponsByName.TryGetValue(weaponName, out weapon);
            }
            return isWeaponInInventory;
        }

        public IEnumerable<Weapon> GetAvailableWeapons() => _availableWeapons.Select(weaponRef => weaponRef.Value);
        public IEnumerable<Item> GetAvailableItems() => _availableItems;

        public IEnumerator<Item> GetEnumerator() {
            return _availableItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
