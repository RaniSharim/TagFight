using TagFighter.Events;
using TagFighter.Resources;
using UnityEngine;


namespace TagFighter.UI
{
    public class PartyView : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("partyMemberUnitFrame")]
        [SerializeField] Transform _partyMemberUnitFrame;

        [UnityEngine.Serialization.FormerlySerializedAs("partyToDisplay")]
        [SerializeField] Party _partyToDisplay;
        [SerializeField] EventAggregator _eventAggregator;


        protected void OnDestroy() {
            _partyToDisplay.OnCompositionChanged -= Party_OnCompositionChanged;
            _eventAggregator.UnitSelected -= OnUnitSelected;
        }

        protected void Start() {
            _partyToDisplay.OnCompositionChanged += Party_OnCompositionChanged;
            _eventAggregator.UnitSelected += OnUnitSelected;
        }

        void Party_OnCompositionChanged(object sender, PartyCompositionArgs e) {
            foreach (Transform partyMemberPortrait in transform) {
                Destroy(partyMemberPortrait.gameObject);
            }

            foreach (var partyMember in e.PartyMembers) {
                var partyMemberPortraitTransform = Instantiate(_partyMemberUnitFrame, transform);
                var partyMemberPortrait = partyMemberPortraitTransform.GetComponentInChildren<PartyMemberPortrait>();
                partyMemberPortrait.SetPartyMember(partyMember);
                var partyMemberResources = partyMemberPortraitTransform.GetComponentsInChildren<ResourceWatcher>();
                partyMemberResources[0].WatchedResource = partyMember.GetComponent<Pain>();
                partyMemberResources[1].WatchedResource = partyMember.GetComponent<Fatigue>();
            }
        }

        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if (e.UnitTransform.TryGetComponent(out PartyMember partyMember)) {
                SetSelectedPortrait(partyMember);
            }
        }

        void SetSelectedPortrait(PartyMember partyMember) {
            foreach (Transform partyMemberPortraitTransform in transform) {
                var partyMemberPortrait = partyMemberPortraitTransform.GetComponentInChildren<PartyMemberPortrait>();
                partyMemberPortrait.SetSelected(partyMemberPortrait.GetPartyMember() == partyMember);
            }
        }
    }
}
