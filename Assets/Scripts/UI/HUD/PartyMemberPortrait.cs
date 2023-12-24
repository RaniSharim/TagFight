using TagFighter.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TagFighter.UI
{
    [RequireComponent(typeof(Button))]
    public class PartyMemberPortrait : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] EventAggregator _eventAggregator;
        Button _portrait;
        TextMeshProUGUI _characterName;
        PartyMember _partyMember;

        protected void Awake() {
            _portrait = GetComponent<Button>();
            _characterName = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleSelectPartyMember();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
            }
        }
        public void SetSelected(bool isSelected) {
            _portrait.GetComponent<Outline>().enabled = isSelected;
        }

        public void SetPartyMember(PartyMember partyMember) {
            _partyMember = partyMember;
            SetPartyMemberName(partyMember.name);
        }

        public PartyMember GetPartyMember() => _partyMember;

        void HandleSelectPartyMember() {
            _eventAggregator.OnUnitSelected(this, new(_partyMember.transform));
        }

        void SetPartyMemberName(string partyMemberName) {
            _characterName.text = partyMemberName;
        }


    }
}
