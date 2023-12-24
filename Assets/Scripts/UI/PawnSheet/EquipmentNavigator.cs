using System.Collections.Generic;
using System.Linq;
using TagFighter.Equipment;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UI
{
    public class EquipmentNavigator : MonoBehaviour
    {
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] ItemUI _itemUIPrefab;
        PartyMember _selectedPartyMember;
        Outfit _watchedOutfit;
        PawnSheetState _pawnSheetState = PawnSheetState.Uninit;
        [SerializeField] ItemSlotBank _itemSlotTypeIcons;
        IDictionary<ItemSlotType, Sprite> _spriteForItemSlotType;

        protected void Awake() {
            _eventAggregator.UnitSelected += OnUnitSelected;
            _eventAggregator.PawnSheetShowed += OnPawnSheetShowed;
            _eventAggregator.PawnSheetHid += OnPawnSheetHid;

            if (_itemSlotTypeIcons) {
                _spriteForItemSlotType = new Dictionary<ItemSlotType, Sprite>();
                foreach (var p in _itemSlotTypeIcons) {
                    _spriteForItemSlotType.TryAdd(p.ItemSlotType, p.Icon);
                }
            }
        }

        protected void OnDestroy() {
            _eventAggregator.UnitSelected -= OnUnitSelected;
            _eventAggregator.PawnSheetShowed -= OnPawnSheetShowed;
            _eventAggregator.PawnSheetHid -= OnPawnSheetHid;
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged -= OnOutfitChanged;
            }
        }

        void OnPawnSheetHid(object sender, EventAggregator.PawnSheetHidEventArgs e) {
            _pawnSheetState = PawnSheetState.Hidden;
        }

        void OnPawnSheetShowed(object sender, EventAggregator.PawnSheetShowedEventArgs e) {
            _pawnSheetState = PawnSheetState.Shown;
            if (_selectedPartyMember != null) {
                CreateEquipmentList(_selectedPartyMember);
            }
        }

        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if (e.UnitTransform.TryGetComponent<PartyMember>(out var partyMember) && _selectedPartyMember != partyMember) {
                _selectedPartyMember = partyMember;

                ChangeOutfit(partyMember.GetComponent<Outfit>());

                if (_pawnSheetState == PawnSheetState.Shown) {
                    CreateEquipmentList(_selectedPartyMember);
                }
            }
        }

        void ChangeOutfit(Outfit outfit) {
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged -= OnOutfitChanged;
            }

            if (outfit != null) {
                outfit.OutfitChanged += OnOutfitChanged;
            }

            _watchedOutfit = outfit;
        }

        void OnOutfitChanged(object sender, OutfitChangedEventArgs e) {
            if (_pawnSheetState == PawnSheetState.Shown) {
                CreateEquipmentList(_selectedPartyMember);
            }
        }

        void CreateEquipmentList(PartyMember partyMember) {
            var currentItemIndex = 0;
            HashSet<Item> equippedItems = new();

            foreach (var item in _watchedOutfit.Select(itemSlot => itemSlot.EquippedItem).Where(item => item != null)) {
                var itemUI = currentItemIndex < transform.childCount ?
                                  transform.GetChild(currentItemIndex).GetComponent<ItemUI>() :
                                  Instantiate(_itemUIPrefab, transform).GetComponent<ItemUI>();

                equippedItems.Add(item);
                itemUI.SetData(_watchedOutfit, item, _spriteForItemSlotType);
                itemUI.gameObject.SetActive(true);

                currentItemIndex++;
            }

            foreach (var item in partyMember.GetComponent<Backpack>().Where(item => !equippedItems.Contains(item))) {
                var itemUI = currentItemIndex < transform.childCount ?
                                  transform.GetChild(currentItemIndex).GetComponent<ItemUI>() :
                                  Instantiate(_itemUIPrefab, transform).GetComponent<ItemUI>();

                itemUI.SetData(_watchedOutfit, item, _spriteForItemSlotType);
                itemUI.gameObject.SetActive(true);

                currentItemIndex++;
            }

            for (; currentItemIndex < transform.childCount; currentItemIndex++) {
                transform.GetChild(currentItemIndex).gameObject.SetActive(false);
            }
        }
    }
}
