using System.Collections.Generic;
using System.Linq;
using TagFighter.Actions;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UI
{
    public class PlannedActionsView : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("eventAggregator")]
        [SerializeField] EventAggregator _eventAggregator;
        public List<PlannedActionClicker> PlannedActionsUI;

        ActionPlan _watchedActionPlan;
        ActionPlan WatcedActionPlan {
            get {
                return _watchedActionPlan;
            }
            set {
                if (_watchedActionPlan != null) {
                    _watchedActionPlan.PlanChanged -= OnPlannedActionsChanged;
                }

                _watchedActionPlan = value;

                if (_watchedActionPlan != null) {
                    _watchedActionPlan.PlanChanged += OnPlannedActionsChanged;
                }
            }
        }
        protected void OnDestroy() {
            _eventAggregator.UnitSelected -= OnUnitSelected;

            WatcedActionPlan = null;
        }
        protected void Start() {
            _eventAggregator.UnitSelected += OnUnitSelected;

            for (var i = 0; i < PlannedActionsUI.Count(); ++i) {
                var uiClicker = PlannedActionsUI[i];
                uiClicker.SetActionName("");
                uiClicker.Index = i;
            }
        }

        void OnPlannedActionsChanged(object sender, UnitActionEventArgs e) {
            UpdatePlannedActions(e.UnitTransform.GetComponent<ActionPlan>());
        }

        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if ((e.UnitTransform.GetComponent<PartyMember>() != null) && e.UnitTransform.TryGetComponent<ActionPlan>(out var actionPlan)) {
                WatcedActionPlan = actionPlan;
                UpdatePlannedActions(WatcedActionPlan);
            }
        }

        void UpdatePlannedActions(ActionPlan actionPlan) {
            var count = 0;
            foreach (var item in actionPlan.Zip(PlannedActionsUI, (action, uiClicker) => (action, uiClicker))) {
                item.uiClicker.SetActionName(item.action.ToString());
                if (item.uiClicker.TryGetComponent(out PlannedActionStatus statusWatcher)) {
                    statusWatcher.SetWatched(item.action);
                }
                count++;
            }

            foreach (var uiClicker in PlannedActionsUI.Skip(count)) {
                uiClicker.SetActionName("");
                if (uiClicker.TryGetComponent(out PlannedActionStatus statusWatcher)) {
                    statusWatcher.SetWatched(null);
                }
            }
        }
    }
}
