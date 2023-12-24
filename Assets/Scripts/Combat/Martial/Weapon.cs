using System;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Equipment;
using TagFighter.Martial;
using UnityEngine;

namespace TagFighter
{
    [Serializable]
    public class ItemTemp : IItem
    {
        public string Name;
        [SerializeField] List<ItemEffectSlotTuple> _effects;
        public IEnumerable<EffectTypePair> GetEffects(params ItemSlotType[] slots) {
            return _effects.Where(effectSlot => effectSlot.Slot.Intersect(slots).Any()).Select(itemEffectSlot => itemEffectSlot.Effect);
        }

        public virtual IEnumerable<EffectTypePair> MainHandEffects() => _effects.Select(itemEffectSlotTuple => itemEffectSlotTuple.Effect);
    }

    [Serializable]
    public class Weapon : ItemTemp
    {
        [SerializeField] CombatMoveSetRef _weaponMoveSet;
        [SerializeField] List<CombatFormRef> _compatibleCombatForms;

        public IEnumerable<CombatFormRef> GetCompatibleFormRefs() {
            return _compatibleCombatForms;
        }

        public CombatMoveSetRef GetCombatMoveSetRef() => _weaponMoveSet;
    }


}
