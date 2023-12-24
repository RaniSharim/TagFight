using System;
using UnityEngine;

namespace TagFighter
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Equipment/Weapon")]
    [Serializable]
    public class WeaponRef : ScriptableObject
    {
        [SerializeField] Weapon _weapon;
        public static implicit operator Weapon(WeaponRef weaponRef) => weaponRef.Value;

        public Weapon Value => _weapon;

    }

}
