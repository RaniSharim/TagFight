using System.Collections.Generic;
using System.Linq;
using TagFighter.Equipment;
using UnityEngine;

namespace TagFighter.UI
{
    public class FastEquipUI : MonoBehaviour
    {
        [SerializeField] FastEquipSlotClicker _fastEquipSlotClickerPrefab;

        public FastEquipUI SetData(Outfit outfit, Item item, IDictionary<ItemSlotType, Sprite> spriteForItemSlot) {
            var currentItemSlotIndex = 0;
            // item.GetEffects(outfit.Select(itemSlot => itemSlot.GetValidItemSlot()).ToArray()).Select(pair => pair.Slo);

            foreach (var itemSlot in outfit.Where(slot => slot.CanEquip(item))) {
                var itemSlotUI = currentItemSlotIndex < transform.childCount ?
                                  transform.GetChild(currentItemSlotIndex).GetComponent<FastEquipSlotClicker>() :
                                  Instantiate(_fastEquipSlotClickerPrefab, transform).GetComponent<FastEquipSlotClicker>();

                itemSlotUI.SetData(itemSlot, item, spriteForItemSlot);

                itemSlotUI.gameObject.SetActive(true);
                currentItemSlotIndex++;
            }

            for (; currentItemSlotIndex < transform.childCount; currentItemSlotIndex++) {
                transform.GetChild(currentItemSlotIndex).gameObject.SetActive(false);
            }

            return this;
        }
    }
}