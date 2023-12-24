using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TagFighter.Actions
{
    public class TakeActionEventArgs : EventArgs
    {
        public IActionState ActionState { get; }
        public TakeActionEventArgs(IActionState actionState) {
            ActionState = actionState;
        }
    }

    public class ActionPlan : MonoBehaviour, IEnumerable<IActionRead>
    {
        public event EventHandler<TakeActionEventArgs> ActionTaken;
        public event EventHandler<UnitActionEventArgs> PlanChanged;
        Line<IAction> _plannedActions = new(capacity: 6);

        protected void OnActionTaken(TakeActionEventArgs e) {
            ActionTaken?.Invoke(this, e);
        }

        protected void OnPlannedActionsChanged(UnitActionEventArgs e) {
            PlanChanged?.Invoke(this, e);
        }

        protected void Update() {
            if (_plannedActions.TryPeek(out var action)) {
                var actionState = action.Advance();
                OnActionTaken(new(actionState));

                if (!actionState.IsAdvanced) {
                    action.Dispose();
                    _plannedActions.TryGetFirstOutOfLine(out var _);
                    OnPlannedActionsChanged(new(transform));
                }
            }
        }

        public bool TryAddActionToPlan(IAction action) {
            var success = false;
            if (_plannedActions.TryWaitInLine(action)) {
                OnPlannedActionsChanged(new(transform));
                success = true;
            }
            return success;
        }

        public void Clear() {
            foreach (var action in _plannedActions) {
                action.Dispose();
            }
            _plannedActions.Clear();
            OnPlannedActionsChanged(new(transform));
        }

        public bool TryRemoveActionFromPlan(int index) {
            var success = false;
            if (_plannedActions.TryRemoveAt(index, out var _)) {
                success = true;
                OnPlannedActionsChanged(new(transform));
            }
            return success;
        }

        public IEnumerator<IActionRead> GetEnumerator() {
            return _plannedActions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _plannedActions.GetEnumerator();
        }
    }

}
