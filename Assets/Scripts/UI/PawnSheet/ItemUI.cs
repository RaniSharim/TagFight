using System.Collections.Generic;
using TagFighter.Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] FastEquipUI _fastEquip;
        [SerializeField] Image _itemIcon;

        protected void Awake() {
            if (_name == null) {
                _name = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            }
        }

        public ItemUI SetData(Outfit outfit, Item item, IDictionary<ItemSlotType, Sprite> spriteForItemSlotType) {
            _name.text = item.ToString();
            _fastEquip.SetData(outfit, item, spriteForItemSlotType);
            return this;
        }
    }
}