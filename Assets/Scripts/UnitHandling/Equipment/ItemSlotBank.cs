using System;
using System.Collections;
// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TagFighter.Equipment
{
    [CreateAssetMenu(fileName = "NewItemSlotBank", menuName = "Game/Equipment/ItemSlotBank")]
    public class ItemSlotBank : ScriptableObject, IEnumerable<ItemSlotContainer>
    {
        [SerializeField] List<ItemSlotContainer> _itemSlots;

        public IEnumerator<ItemSlotContainer> GetEnumerator() {
            return _itemSlots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    [Serializable]
    public class ItemSlotContainer
    {
        [SerializeField] ItemSlotType _itemSlotType;
        public ItemSlotType ItemSlotType {
            get => _itemSlotType;
        }

        [SerializeField] Sprite _icon;
        public Sprite Icon {
            get => _icon;
        }
    }

}

