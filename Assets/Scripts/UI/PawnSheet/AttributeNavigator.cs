using System.Threading;
using TagFighter.Events;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.UI
{
    public class AttributeNavigator : MonoBehaviour
    {
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] AttributeUI _attributeUIPrefab;
        PartyMember _selectedPartyMember;
        CancellationTokenSource _cancellationSource;
        PawnSheetState _pawnSheetState = PawnSheetState.Uninit;

        protected void Awake() {
            _eventAggregator.UnitSelected += OnUnitSelected;
            _eventAggregator.PawnSheetShowed += OnPawnSheetShowed;
            _eventAggregator.PawnSheetHid += OnPawnSheetHid;
        }

        protected void OnDestroy() {
            _eventAggregator.UnitSelected -= OnUnitSelected;
            _eventAggregator.PawnSheetShowed -= OnPawnSheetShowed;
            _eventAggregator.PawnSheetHid -= OnPawnSheetHid;
            if (_cancellationSource != null) {
                _cancellationSource.Cancel();
                _cancellationSource.Dispose();
            }
        }

        void RegisterToResource(IWatchableResource resource, IResourceViewer resourceViewer, CancellationToken cancellationToken) {
            void Handler(object sender, ResourceChangeArgs e) {
                resourceViewer.UpdateView(e);
            }

            void UnregisterHandler() {
                resource.ResourceChanged -= Handler;
            }

            resource.ResourceChanged += Handler;
            cancellationToken.Register(UnregisterHandler);
            resourceViewer.UpdateView(resource.Status);
        }

        void OnPawnSheetHid(object sender, EventAggregator.PawnSheetHidEventArgs e) {
            _pawnSheetState = PawnSheetState.Hidden;
            _cancellationSource?.Cancel();
        }

        void OnPawnSheetShowed(object sender, EventAggregator.PawnSheetShowedEventArgs e) {
            _pawnSheetState = PawnSheetState.Shown;
            if (_selectedPartyMember != null) {
                _cancellationSource?.Cancel();
                CreateAttributeList(_selectedPartyMember);
            }
        }

        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if (e.UnitTransform.TryGetComponent<PartyMember>(out var partyMember) && _selectedPartyMember != partyMember) {
                _selectedPartyMember = partyMember;
                if (_pawnSheetState == PawnSheetState.Shown) {
                    _cancellationSource?.Cancel();
                    CreateAttributeList(_selectedPartyMember);
                }
            }
        }

        void CreateAttributeList(PartyMember partyMember) {
            var currentAttributeIndex = 0;

            _cancellationSource = new();
            foreach (var resource in partyMember.GetComponents<IWatchableResource>()) {
                var attributeUI = currentAttributeIndex < transform.childCount ?
                                  transform.GetChild(currentAttributeIndex).GetComponent<AttributeUI>() :
                                  Instantiate(_attributeUIPrefab, transform).GetComponent<AttributeUI>();

                attributeUI.SetName(resource.GetType().Name);
                RegisterToResource(resource, attributeUI, _cancellationSource.Token);

                attributeUI.gameObject.SetActive(true);
                currentAttributeIndex++;
            }

            for (; currentAttributeIndex < transform.childCount; currentAttributeIndex++) {
                transform.GetChild(currentAttributeIndex).gameObject.SetActive(false);
            }
        }
    }
}
