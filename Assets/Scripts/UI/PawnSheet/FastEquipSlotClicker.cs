using System.Collections.Generic;
using TagFighter.Equipment;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class FastEquipSlotClicker : MonoBehaviour, IPointerClickHandler
    {
        Image _slotTypeImage;
        Outline _selectedOutline;
        Item _item;
        IItemSlot _itemSlot;

        public FastEquipSlotClicker SetData(IItemSlot itemSlot, Item item, IDictionary<ItemSlotType, Sprite> spriteForItemSlot) {
            if (_itemSlot != null) {
                _selectedOutline.enabled = false;
            }
            _itemSlot = itemSlot;
            _item = item;
            if (_itemSlot.EquippedItem == _item) {
                _selectedOutline.enabled = true;
            }

            if (_slotTypeImage != null && spriteForItemSlot.TryGetValue(itemSlot.GetValidItemSlot(), out var itemSlotSprite)) {
                _slotTypeImage.sprite = itemSlotSprite;
            }

            return this;
        }

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleEquipItem(_itemSlot, _item);
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }
        protected void Awake() {
            _slotTypeImage = GetComponent<Image>();
            _selectedOutline = GetComponent<Outline>();
        }

        bool HandleEquipItem(IItemSlot itemSlot, Item item) {
            var equipSuccess = false;
            if (itemSlot.EquippedItem == item) {
                equipSuccess = itemSlot.TryUnequip();
            }
            else if (itemSlot.TryUnequip()) {
                equipSuccess = itemSlot.TryEquip(item);
            }

            return equipSuccess;
        }
    }


}
