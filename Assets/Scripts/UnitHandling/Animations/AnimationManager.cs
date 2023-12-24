using System.Collections.Generic;
using TagFighter.Actions;
using TagFighter.Equipment;
using UnityEngine;

namespace TagFighter
{
    [RequireComponent(typeof(Weaver))]
    public class AnimationManager : MonoBehaviour
    {
        List<string> _idleStatesNames = new();
        string _idleStateName;
        string _runStateName;

        Animator _animator;
        AnimatorOverrideController _animatorOverrideController;
        List<KeyValuePair<AnimationClip, AnimationClip>> _clipOverrides;

        IActionState _currentActionState;
        IActionState _nextActionState;
        int _layer = 0;

        [SerializeField] AnimationClipSetBank _animationBank;
        Dictionary<string, string> _moveToStateMap = new();
        Dictionary<string, float> _animationLengths = new();

        Outfit _watchedOutfit;
        ActionPlan _actionPlan;

        void SetActionPlan(ActionPlan actionPlan) {
            if (_actionPlan != null) {
                _actionPlan.ActionTaken -= OnActionTaken;
            }
            _actionPlan = actionPlan;
            if (_actionPlan != null) {
                _actionPlan.ActionTaken += OnActionTaken;
            }
        }
        void SetWatchedOutfit(Outfit outfit) {
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged -= OnOutfitChanged;
            }
            _watchedOutfit = outfit;
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanged += OnOutfitChanged;
            }
        }



        protected void Awake() {
            _animator = GetComponentInChildren<Animator>();
            _currentActionState = default(IdleState);
            _nextActionState = default(IdleState);

            SetActionPlan(GetComponent<ActionPlan>());
            SetWatchedOutfit(GetComponent<Outfit>());
            PopulateAnimations(_animationBank.AnimationClipSets[0].ClipSet);

        }

        protected void OnDestroy() {
            SetWatchedOutfit(null);
            SetActionPlan(null);
        }

        void OnActionTaken(object sender, TakeActionEventArgs e) {
            SetActionState(e.ActionState);
        }

        void OnOutfitChanged(object sender, OutfitChangedEventArgs e) {
            ChangeClipSet(e.ChangedTo);
        }

        void ChangeClipSet(Item item) {
            var clipSet = _animationBank.AnimationClipSets.Find(set => ReferenceEquals(set.Item, item))?.ClipSet;

            if (clipSet != default) {
                PopulateAnimations(clipSet);
                ResetNextState();
                _idleStateName = GetNewIdleState();
                _animator.Play(_idleStateName, _layer, 0f);
            }
        }

        void PopulateAnimations(AnimationClipSet animationClipSet) {
            _animationLengths.Clear();
            _moveToStateMap.Clear();
            _idleStatesNames.Clear();

            _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorOverrideController;
            _clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(_animatorOverrideController.overridesCount);
            _animatorOverrideController.GetOverrides(_clipOverrides);

            for (var i = 0; i < _clipOverrides.Count; ++i) {
                var clipOverride = _clipOverrides[i];
                var overrideClip = clipOverride.Value;
                // Assuming State name and Clip Key Name are the same 
                var stateName = clipOverride.Key.name;
                if (stateName.Contains("Idle")) {
                    if (_idleStatesNames.Count < animationClipSet.Idle.Count) {
                        _idleStatesNames.Add(stateName);
                        overrideClip = animationClipSet.Idle[_idleStatesNames.Count - 1];
                    }
                }
                else if (stateName.Contains("Run")) {
                    _runStateName = stateName;
                    overrideClip = animationClipSet.Run;
                }
                else if (stateName.Contains("Attack")) {
                    var attackCount = _moveToStateMap.Count;
                    if (attackCount < animationClipSet.Attacks.Count) {
                        var moveName = animationClipSet.Attacks[attackCount].CombatMove.Value.MoveName;
                        overrideClip = animationClipSet.Attacks[attackCount].AttackAnimation;
                        _moveToStateMap[moveName] = stateName;
                        _animationLengths[moveName] = overrideClip.length;
                    }
                }
                _clipOverrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(_clipOverrides[i].Key, overrideClip);
            }

            _animatorOverrideController.ApplyOverrides(_clipOverrides);
        }

        void SetActionState(IActionState nextActionState) {
            _nextActionState = nextActionState.IsAdvanced ? nextActionState : default(IdleState);
        }

        void ResetNextState() {
            _nextActionState = default(IdleState);
        }

        void HandleIdleState() {
            switch (_nextActionState) {
                case IdleState:
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99) {
                        var newIdle = GetNewIdleState();
                        if (_idleStateName != newIdle) {
                            _idleStateName = newIdle;
                            _animator.Play(_idleStateName, _layer, 0f);
                        }
                    }
                    break;
                case MoveToAction.State:
                    _animator.Play(_runStateName, _layer, 0f);
                    break;
                case WeaveAction.State weave:
                    StartWeave(weave);
                    break;
            }
        }
        void HandleMoveToState() {
            switch (_nextActionState) {
                case IdleState:
                    _animator.Update(-Mathf.Floor(_animator.GetCurrentAnimatorStateInfo(_layer).normalizedTime));
                    _animator.CrossFade(_idleStateName, 0.25f, _layer);
                    break;
                case MoveToAction.State:
                    break;
                case WeaveAction.State weave:
                    StartWeave(weave);
                    break;

            }
        }

        void HandleWeaveState(WeaveAction.State currentWeave) {
            switch (_nextActionState) {
                case IdleState:
                    _animator.Play(_idleStateName, _layer, 0f);
                    break;
                case MoveToAction.State:
                    _animator.Update(-Mathf.Floor(_animator.GetCurrentAnimatorStateInfo(_layer).normalizedTime));
                    _animator.CrossFade(_runStateName, 0.25f, _layer);
                    break;
                case WeaveAction.State weave:
                    if (weave.CompletionStatus < 1) {
                        TransitionWeave(currentWeave, weave);
                    }
                    break;
            }
        }

        void ApplyActionState() {
            switch (_currentActionState) {
                case IdleState:
                    HandleIdleState();
                    break;
                case MoveToAction.State:
                    HandleMoveToState();
                    break;
                case WeaveAction.State currentWeave:
                    HandleWeaveState(currentWeave);
                    break;
            }
            _currentActionState = _nextActionState;
        }

        protected void LateUpdate() {
            if (_animator) {
                ApplyActionState();
                ResetNextState();
            }
        }

        void StartWeave(WeaveAction.State nextWeave) {
            var moveName = nextWeave.Move.MoveName;
            var moveStateName = _moveToStateMap[moveName];
            _animator.Update(-Mathf.Floor(_animator.GetCurrentAnimatorStateInfo(_layer).normalizedTime));
            if (_animator.GetCurrentAnimatorStateInfo(_layer).normalizedTime > 0.9f) {
                _animator.Play(moveStateName);
            }
            else {
                _animator.CrossFade(moveStateName, 0.1f, _layer);
            }
            var animationLength = _animationLengths[moveName];
            if (nextWeave.Move.Speed > 0) {
                var desiredSpeed = animationLength / nextWeave.Move.Speed;
                _animator.SetFloat($"Speed", desiredSpeed);
            }
        }
        void TransitionWeave(WeaveAction.State currentWeave, WeaveAction.State nextWeave) {
            if (currentWeave.Move != nextWeave.Move) {
                StartWeave(nextWeave);
            }
        }

        string GetNewIdleState() {
            var idleAnimationIndex = Random.Range(0, _idleStatesNames.Count);
            return _idleStatesNames[idleAnimationIndex];
        }

        internal readonly struct IdleState : IActionState
        {
            public bool IsAdvanced { get; }
            public float CompletionStatus { get; }
            public bool IsMoving { get; }

            public IdleState(bool isAdvanced, float completionStatus) {
                IsAdvanced = isAdvanced;
                CompletionStatus = completionStatus;
                IsMoving = false;
            }
        }
    }
}