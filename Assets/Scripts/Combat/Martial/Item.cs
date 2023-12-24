using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TagFighter.Equipment
{
    public interface IItem
    {
        public IEnumerable<EffectTypePair> GetEffects(params ItemSlotType[] slots);

        public IEnumerable<EffectTypePair> MainHandEffects();
    }

    [Serializable]
    public class ItemEffectSlotTuple
    {
        public List<ItemSlotType> Slot;
        public EffectTypePair Effect;
    }



    [CreateAssetMenu(fileName = "Item", menuName = "Game/Equipment/Item")]
    [Serializable]
    public class Item : ScriptableObject, IItem
    {
        [SerializeField] List<ItemEffectSlotTuple> _effects;
        public IEnumerable<EffectTypePair> GetEffects(params ItemSlotType[] slots) {
            return _effects.Where(effectSlot => effectSlot.Slot.Intersect(slots).Any()).Select(itemEffectSlot => itemEffectSlot.Effect);
        }

        public override string ToString() {
            return name;
        }

        public virtual IEnumerable<EffectTypePair> MainHandEffects() => _effects.Select(itemEffectSlotTuple => itemEffectSlotTuple.Effect);
    }



}