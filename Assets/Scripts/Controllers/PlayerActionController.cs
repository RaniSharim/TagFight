using System;
using TagFighter.Actions;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter.UnitControl
{
    public class PlayerActionController : MonoBehaviour
    {
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] UnitControllerType _controllingUnitsOfType;
        Transform _targetUnit;
        Transform TargetUnit {
            get {
                return _targetUnit;
            }
            set {
                if (_targetUnit != null) {
                    TryUnregisterRuneWeavingCast();
                }

                _targetUnit = value;

                if (_targetUnit != null) {
                    TryRegisterRuneWeavingCast();
                }
            }
        }
        Weaver _selectedWeaver;
        Weaver SelectedWeaver {
            get {
                return _selectedWeaver;
            }
            set {
                if (_selectedWeaver != null) {
                    _eventAggregator.PlannedActionsClearSelected -= OnPlannedActionsClearSelected;
                    _eventAggregator.RemovePlannedActionSelected -= OnRemovePlannedActionSelected;
                    _eventAggregator.UnitMove -= OnUnitMove;
                    TryUnregisterRuneWeavingCast();
                }
                _selectedWeaver = value;

                if (_selectedWeaver != null) {
                    _eventAggregator.PlannedActionsClearSelected += OnPlannedActionsClearSelected;
                    _eventAggregator.RemovePlannedActionSelected += OnRemovePlannedActionSelected;
                    _eventAggregator.UnitMove += OnUnitMove;
                    TryRegisterRuneWeavingCast();
                }
            }
        }

        ActionPlan _actionPlan;

        protected void Start() {
            _eventAggregator.UnitSelected += OnUnitSelected;

            _eventAggregator.TargetSelected += OnTargetSelected;

        }

        protected void OnDestroy() {
            _eventAggregator.UnitSelected -= OnUnitSelected;

            _eventAggregator.TargetSelected -= OnTargetSelected;

            SelectedWeaver = null;
            TargetUnit = null;
        }

        bool TryUnregisterRuneWeavingCast() {
            var unregistered = false;
            if (SelectedWeaver && TargetUnit) {
                _eventAggregator.RuneWeavingCastSelected -= OnRuneWeavingCastSelected;
                unregistered = true;
            }
            return unregistered;
        }
        bool TryRegisterRuneWeavingCast() {
            var registered = false;
            if (SelectedWeaver && TargetUnit) {
                _eventAggregator.RuneWeavingCastSelected += OnRuneWeavingCastSelected;
                registered = true;
            }
            return registered;
        }

        void OnPlannedActionsClearSelected(object sender, EventArgs e) {
            _actionPlan.Clear();
        }

        void OnRemovePlannedActionSelected(object sender, EventAggregator.TryRemovePlannedActionEventArgs e) {
            _actionPlan.TryRemoveActionFromPlan(e.Index);
        }

        void OnRuneWeavingCastSelected(object sender, EventAggregator.WeaveCastEventArgs e) {
            RuneWeavingContainer weaving = new(e.Weave.WeaveName, e.Weave.RuneWeaving);
            WeaveAction weaveAction = new(SelectedWeaver, weaving, TargetUnit, SelectedWeaver.GetComponent<Resources.Range>());
            _actionPlan.TryAddActionToPlan(weaveAction);
        }

        void OnUnitMove(object sender, UnitMoveEventArgs e) {
            _actionPlan.TryAddActionToPlan(new MoveToAction(SelectedWeaver, e.Point));
        }
        void OnTargetSelected(object sender, UnitActionEventArgs e) {
            TargetUnit = e.UnitTransform;
        }

        void OnUnitSelected(object sender, UnitActionEventArgs e) {
            if (e.UnitTransform.TryGetComponent<Weaver>(out var weaver)) {
                SelectedWeaver = weaver;
            }

            if (e.UnitTransform.TryGetComponent<ActionPlan>(out var actionPlan)) {
                _actionPlan = actionPlan;
            }

        }

    }
}
