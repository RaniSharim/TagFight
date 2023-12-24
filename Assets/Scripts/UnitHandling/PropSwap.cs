using System;
using System.Collections;
using System.Collections.Generic;
using TagFighter.Equipment;
using UnityEngine;

namespace TagFighter
{
    public class PropSwap : MonoBehaviour
    {
        Outfit _watchedOutfit;
        [SerializeField] List<PropSet> _propSets;
        void SetWatchedOutfit(Outfit outfit) {
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged -= OnOutfitChanged;
            }
            _watchedOutfit = outfit;

            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged += OnOutfitChanged;
            }
        }


        protected void Awake() {
            SetWatchedOutfit(GetComponentInParent<Outfit>());
        }
        protected void OnDestroy() {
            SetWatchedOutfit(null);
        }

        void OnOutfitChanged(object sender, OutfitChangedEventArgs e) {
            ChangeActiveProps(e.ChangedFrom, e.ChangedTo);
        }

        void ChangeActiveProps(Item changedFrom, Item changedTo) {
            var changedFromSet = _propSets.Find(set => ReferenceEquals(set.Item, changedFrom));
            if (changedFromSet != default) {
                foreach (var prop in changedFromSet) {
                    prop.SetActive(false);
                }
            }

            var changedToSet = _propSets.Find(set => ReferenceEquals(set.Item, changedTo));
            if (changedToSet != default) {
                foreach (var prop in changedToSet) {
                    prop.SetActive(true);
                }
            }
        }
    }

    [Serializable]
    public class PropSet : IEnumerable<GameObject>
    {
        public Item Item;
        public List<GameObject> ItemProps;

        public IEnumerator<GameObject> GetEnumerator() {
            return ItemProps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}