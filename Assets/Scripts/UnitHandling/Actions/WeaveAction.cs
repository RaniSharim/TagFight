using System;
using TagFighter.Effects;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter
{
    public class WeaveAction : IAction
    {
        FollowAction _followAction;
        Transform _target;
        Weaver _weaver;
        RuneWeavingContainer _runeWeaving;
        Resources.Range _range;
        bool _startedWeaving = false;
        static State s_resetState = new(true, 1f, false, default);
        bool _disposed = false;
        int _lastRange;

        EffectContext _context;

        ITimeContextContainer _timeContextContainer;


        public WeaveAction(Weaver unit, RuneWeavingContainer runeWeaving, Transform target, Resources.Range range) {
            if (range is null) {
                throw new ArgumentNullException("range");
            }
            _target = target;
            _range = range;
            _weaver = unit;
            _runeWeaving = runeWeaving;
            var currentRange = _range.GetCurrent();
            _lastRange = currentRange.AsPrimitive();
            _followAction = new(_weaver, _target, currentRange.ToMeter());
            _range.ResourceChanged += OnResourceChanged;
            _context = new EffectContext() {
                EffectLocation = target,
                Caster = unit.transform
            };
        }

        void OnResourceChanged(object sender, ResourceChangeArgs e) {
            if (_lastRange != e.Current) {
                _followAction.Dispose();
                _followAction = new(_weaver, _target, _range.GetCurrent().ToMeter());
                _lastRange = e.Current;
            }
        }

        public IActionState Advance() {
            var stateToReturn = _followAction.Advance();

            if (IsInAttackRange()) {
                if (_startedWeaving == false) {
                    Debug.Log("Target got in range, weaving");
                    _timeContextContainer = _runeWeaving.RuneWeaving.CreateTimeContexts();
                }
                _startedWeaving = true;

                var isAdvanced = _runeWeaving.RuneWeaving.Advance(_timeContextContainer, _context, Time.deltaTime);
                stateToReturn = new State(isAdvanced, CompletionStatus(), false, _runeWeaving.RuneWeaving.GetCurrentMove(_timeContextContainer));
            }
            else {
                if (_startedWeaving) {
                    Debug.Log("Target escaped");
                    stateToReturn = new State(false, CompletionStatus(), stateToReturn.IsMoving, _runeWeaving.RuneWeaving.GetCurrentMove(_timeContextContainer));
                }
            }
            return stateToReturn;
        }

        IActionState Cancel() {
            _timeContextContainer = null;
            return s_resetState;
        }

        public float CompletionStatus() => _runeWeaving.RuneWeaving.CompletionStatus(_timeContextContainer);

        public bool IsSimilarAction(IActionRead action) {
            if (action is not WeaveAction other) {
                return false;
            }

            return other != null && _target == other._target && _weaver == other._weaver && _runeWeaving.WeaveName == other._runeWeaving.WeaveName;
        }

        public override string ToString() {
            return $"{_runeWeaving.WeaveName} -> {_target.name}";
        }
        bool IsInAttackRange() {
            // Incorrect in case there are hills. should calc range from feet.
            Vector3 source = new(_weaver.transform.position.x, 0, _weaver.transform.position.z);
            Vector3 target = new(_target.position.x, 0, _target.position.z);
            return Vector3.Distance(source, target) <= _range.GetCurrent().ToMeter();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Cancel();
                    _followAction.Dispose();
                    _followAction = null;
                    _range.ResourceChanged -= OnResourceChanged;
                    _range = null;
                    _runeWeaving = null;
                    _target = null;
                    _weaver = null;
                }

                _disposed = true;
            }
        }

        public readonly struct State : IActionState
        {
            public bool IsAdvanced { get; }
            public float CompletionStatus { get; }
            public bool IsMoving { get; }

            public CombatMove Move { get; }

            public State(bool isAdvanced, float completionStatus, bool isMoving, CombatMove move) {
                IsAdvanced = isAdvanced;
                CompletionStatus = completionStatus;
                IsMoving = isMoving;
                Move = move;
            }
        }
    }
}
