// using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects;
using UnityEngine;

namespace TagFighter.Equipment
{

    public class OutfitChangingEventArgs : EventArgs
    {
        public Item ChangingFrom { get; }
        public Item ChangingTo { get; }
        public IItemSlot ChangingSlot { get; }
        public OutfitChangingEventArgs(IItemSlot changingSlot, Item changingFrom, Item changingTo) {
            ChangingSlot = changingSlot;
            ChangingFrom = changingFrom;
            ChangingTo = changingTo;
        }
    }
    public class OutfitChangedEventArgs : EventArgs
    {
        public Item ChangedFrom { get; }
        public Item ChangedTo { get; }
        public IItemSlot ChangedSlot { get; }

        public OutfitChangedEventArgs(IItemSlot changedSlot, Item changedFrom, Item changedTo) {
            ChangedSlot = changedSlot;
            ChangedFrom = changedFrom;
            ChangedTo = changedTo;
        }
    }

    public interface IWatchableOutfit
    {
        public event EventHandler<OutfitChangingEventArgs> OutfitChanging;
        public event EventHandler<OutfitChangedEventArgs> OutfitChanged;
    }

    public class Outfit : MonoBehaviour, IWatchableOutfit, IEnumerable<IItemSlot>
    {
        [SerializeField] List<OutfitItemSlot> _itemSlots;

        public int SlotCount {
            get => _itemSlots.Count;
        }

        public event EventHandler<OutfitChangingEventArgs> OutfitChanging;
        public event EventHandler<OutfitChangedEventArgs> OutfitChanged;

        public IItemSlot this[int index] {
            get {
                if (index < 0 || index > _itemSlots.Count) {
                    throw new IndexOutOfRangeException("Index out of range");
                }

                return _itemSlots[index];
            }
        }

        protected void OnOutfitChanging(OutfitChangingEventArgs e) {
            OutfitChanging?.Invoke(this, e);
        }
        protected void OnOutfitChanged(OutfitChangedEventArgs e) {
            OutfitChanged?.Invoke(this, e);
        }

        protected void Awake() {
            foreach (var itemSlot in _itemSlots) {
                itemSlot.OwningOutfit = this;
            }
        }

        protected void Start() {
            foreach (var itemSlot in _itemSlots.Where(itemSlot => itemSlot.EquippedItem != null)) {
                itemSlot.Equip(itemSlot.EquippedItem);
                OnOutfitChanging(new(itemSlot, null, itemSlot.EquippedItem));
                OnOutfitChanged(new(itemSlot, null, itemSlot.EquippedItem));
            }
        }

        public IEnumerator<IItemSlot> GetEnumerator() {
            return _itemSlots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        [Serializable]
        class OutfitItemSlot : IItemSlot
        {
            [SerializeField] ItemSlotType _validItemSlot;
            [SerializeField] Item _equippedItem;
            public Item EquippedItem {
                get => _equippedItem;
                private set => _equippedItem = value;
            }
            public Outfit OwningOutfit { get; set; }

            public bool CanEquip(Item item) {
                var canEquip = item.GetEffects(_validItemSlot).Any();
                return canEquip;
            }

            public ItemSlotType GetValidItemSlot() {
                return _validItemSlot;
            }

            public bool IsSlotEmpty() {
                return EquippedItem == null;
            }

            public bool TryEquip(Item item) {
                if (EquippedItem != null) {
                    return false;
                }

                if (OwningOutfit.Any(itemSlot => itemSlot.EquippedItem == item)) {
                    return false;
                }

                if (!CanEquip(item)) {
                    return false;
                }

                // Debug.Log($"Equip {item}");
                var changingFrom = EquippedItem;
                Equip(item);
                OwningOutfit.OnOutfitChanging(new(this, changingFrom, item));
                OwningOutfit.OnOutfitChanged(new(this, changingFrom, item));

                return true;
            }

            public bool TryUnequip() {
                if (IsSlotEmpty()) {
                    return true;
                }
                var changingFrom = EquippedItem;
                EquippedItem = null;
                OwningOutfit.OnOutfitChanging(new(this, changingFrom, null));
                OwningOutfit.OnOutfitChanged(new(this, changingFrom, null));
                return true;
            }

            public void Equip(Item item) {
                EquippedItem = item;
                var ownerTransform = OwningOutfit.transform;

                // Debug.Log($"ApplyItemEffects on {ownerTransform}");

                var context = new EffectContext() {
                    EffectLocation = ownerTransform,
                    Caster = ownerTransform
                };

                foreach (var effect in item.GetEffects(_validItemSlot)) {
                    effect.Mode.ImmediateAction(context, effect.Effect);
                }
            }
        }
    }


    public interface IItemSlot
    {
        public bool CanEquip(Item item);
        public Item EquippedItem { get; }
        public ItemSlotType GetValidItemSlot();
        public bool IsSlotEmpty();
        public bool TryEquip(Item item);
        public bool TryUnequip();

    }

}