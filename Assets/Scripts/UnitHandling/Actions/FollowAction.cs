using System;
using UnityEngine;

namespace TagFighter
{
    public class FollowAction : IAction
    {
        MoveToAction _moveAction;
        Transform _target;
        Vector3 _lastTargetPosition;
        bool _disposed = false;

        public FollowAction(Weaver unit, Transform target) {
            _target = target != null ? target : throw new ArgumentNullException("targetToFollow");
            _lastTargetPosition = target.position;
            _moveAction = new(unit, _lastTargetPosition);
        }

        public FollowAction(Weaver unit, Transform target, float stoppingDistance) {
            _target = target != null ? target : throw new ArgumentNullException("targetToFollow");
            _lastTargetPosition = target.position;
            _moveAction = new(unit, _lastTargetPosition, stoppingDistance);
        }

        public void SetStoppingDistance(float stoppingDistance) {
            _moveAction.SetStoppingDistance(stoppingDistance);
        }

        public IActionState Advance() {
            if (_lastTargetPosition != _target.position) {
                _lastTargetPosition = _target.position;
                _moveAction.SetDestination(_lastTargetPosition);
            }

            return _moveAction.Advance();
        }

        public float CompletionStatus() {
            return 0;
        }
        public bool IsSimilarAction(IActionRead action) {
            if (action is not FollowAction other) {
                return false;
            }

            return other != null && _target == other._target;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _moveAction.Dispose();
                    _moveAction = null;
                    _target = null;
                }

                _disposed = true;
            }
        }
        public override string ToString() {
            return $"Follow {_target.name}";
        }
    }
}
