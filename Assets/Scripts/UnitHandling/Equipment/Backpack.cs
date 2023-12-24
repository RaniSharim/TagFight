using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TagFighter.Equipment
{
    public class Backpack : MonoBehaviour, IEnumerable<Item>
    {
        [SerializeField] Inventory _inventory;

        public IEnumerator<Item> GetEnumerator() {
            return _inventory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}