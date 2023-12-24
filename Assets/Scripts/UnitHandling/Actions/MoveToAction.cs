using System;
using UnityEngine;
using UnityEngine.AI;

namespace TagFighter
{
    public class MoveToAction : IAction
    {
        Vector3 _lastTargetPosition;
        NavMeshAgent _agent;
        float _originalStoppingDistance;
        float _desiredStoppingDistance;
        bool _isFirstAdvance = true;
        static State s_resetState = new(false, 0f, false);
        bool _disposed = false;

        public MoveToAction(Weaver unit, Vector3 target) {
            if (unit == null) {
                throw new ArgumentNullException("unit");
            }

            if (unit.TryGetComponent(out _agent) == false) {
                throw new ArgumentException("Missing NavMeshAgent", "unit");
            }

            _lastTargetPosition = target;
            _desiredStoppingDistance = _agent.stoppingDistance;
        }
        public MoveToAction(Weaver unit, Vector3 target, float stoppingDistance) {
            if (unit == null) {
                throw new ArgumentNullException("unit");
            }

            if (stoppingDistance < 0) {
                throw new ArgumentException("Cannot be < 0", "stoppingDistance");
            }

            if (unit.TryGetComponent(out _agent) == false) {
                throw new ArgumentException("Missing NavMeshAgent", "unit");
            }

            _lastTargetPosition = target;
            _desiredStoppingDistance = stoppingDistance;
        }
        public bool SetDestination(Vector3 target) {
            _lastTargetPosition = target;
            return _agent.SetDestination(target);
        }

        public void SetStoppingDistance(float stoppingDistance) {
            if (stoppingDistance < 0) {
                throw new ArgumentException("Cannot be < 0", "stoppingDistance");
            }
            _desiredStoppingDistance = stoppingDistance;
            _agent.stoppingDistance = _desiredStoppingDistance;
        }

        public IActionState Advance() {
            if (_isFirstAdvance) {
                _originalStoppingDistance = _agent.stoppingDistance;
                _agent.stoppingDistance = _desiredStoppingDistance;
                _agent.isStopped = false;
                _agent.SetDestination(_lastTargetPosition);
                _isFirstAdvance = false;
            }

            var isAdvanced = _agent.hasPath || _agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance;

            return new State(isAdvanced, CompletionStatus(), isAdvanced);
        }

        IActionState Cancel() {
            _agent.isStopped = true;
            _agent.stoppingDistance = _originalStoppingDistance;
            _agent.ResetPath();
            _isFirstAdvance = true;

            return s_resetState;
        }

        public float CompletionStatus() {
            return 0;
        }

        public bool IsSimilarAction(IActionRead action) {
            if (action is not MoveToAction other) {
                return false;
            }

            return other != null && _agent == other._agent && _lastTargetPosition == other._lastTargetPosition;
        }

        public override string ToString() {
            return $"Move -> {_lastTargetPosition}";
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Cancel();
                    _agent = null;
                }

                _disposed = true;
            }
        }

        public readonly struct State : IActionState
        {
            public bool IsAdvanced { get; }
            public float CompletionStatus { get; }
            public bool IsMoving { get; }

            public State(bool isAdvanced, float completionStatus, bool isMoving) {
                IsAdvanced = isAdvanced;
                CompletionStatus = completionStatus;
                IsMoving = isMoving;
            }
        }
    }
}
