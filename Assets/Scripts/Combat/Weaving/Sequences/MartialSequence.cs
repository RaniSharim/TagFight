using System;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects;
using TagFighter.Martial;
using UnityEngine;

[Serializable]
public class MartialSequence : ISequence
{
    [UnityEngine.Serialization.FormerlySerializedAs("combatMoveRefs")]
    [SerializeField] List<CombatMoveRef> _combatMoveRefs;

    public MartialSequence(IEnumerable<CombatMoveRef> combatMoveRefsSequence) {
        _combatMoveRefs = combatMoveRefsSequence.ToList();
    }

    public bool Advance(ITimeContextContainer timeContextContainer, EffectContext effectContext, float deltaTime) {
        var timeContext = timeContextContainer.GetTimeContext<MartialSequence>();

        if (timeContext.CurrentIdx >= _combatMoveRefs.Count) {
            Debug.Log($"Executed Martial Sequence  {timeContext.CurrentIdx}/{_combatMoveRefs.Count} moves in {timeContext.CurrentTime} seconds");
            return false;
        }
        timeContext.CurrentTime += deltaTime;

        while ((timeContext.CurrentIdx < _combatMoveRefs.Count) && (timeContext.CurrentTime - timeContext.LastTime >= _combatMoveRefs[timeContext.CurrentIdx].CombatMove.Speed)) {
            Debug.Log($"Finished executing Martial move {_combatMoveRefs[timeContext.CurrentIdx].CombatMove.MoveName} {timeContext.CurrentIdx + 1}/{_combatMoveRefs.Count} at {timeContext.CurrentTime}");
            // TODO : Actual Execute Move
            timeContext.LastTime += _combatMoveRefs[timeContext.CurrentIdx].CombatMove.Speed;
            timeContext.CurrentIdx++;
        }
        SetReleaseMultiplier(timeContextContainer, effectContext);

        return true;
    }

    void SetReleaseMultiplier(ITimeContextContainer timeContextContainer, EffectContext context) {
        var currentMove = GetCurrentMove(timeContextContainer);
        if (currentMove == null) {
            return;
        }

        var caster = context.Caster;
        var animator = caster.gameObject.GetComponentInChildren<Animator>();
        var currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        var posInAnimation = currentAnimationState.normalizedTime;

        //Debug.Log($"posInAnimation {posInAnimation}");


        var positionalReleaseMultiplier = 1f;
        if ((currentMove.StartReleaseNormalized < posInAnimation) && (posInAnimation < currentMove.EndReleaseNormalized)) {
            // this means we're in the maximal release section
            positionalReleaseMultiplier = 1f;
        }
        else if (posInAnimation < currentMove.StartReleaseNormalized) {
            // otherwise, get a partial bonus in reletaion to how close we are to the relese
            positionalReleaseMultiplier = posInAnimation / currentMove.StartReleaseNormalized;
        }
        else if (currentMove.EndReleaseNormalized < posInAnimation) {
            // otherwise, get a partial bonus in reletaion to how far we are from the release
            positionalReleaseMultiplier = (1 - posInAnimation) / (1 - currentMove.EndReleaseNormalized);
        }

        // Debug.Log($"positionalReleaseMultiplier {positionalReleaseMultiplier}");

        context.ReleaseMultiplier = new() {
            MatchingAoe = currentMove.MatchingAoe,
            MatchingAoeReleaseMultiplier = currentMove.MatchingAoeReleaseMultiplier * positionalReleaseMultiplier,
            NonMatchingAoeReleaseMultiplier = currentMove.NonMatchingAoeReleaseMultiplier * positionalReleaseMultiplier,
        };
    }

    public void Simulate() {

    }

    public CombatMove GetCurrentMove(ITimeContextContainer timeContextContainer) {
        if (timeContextContainer == null) {
            return null;
        }

        var timeContext = timeContextContainer.GetTimeContext<MartialSequence>();

        CombatMove move = default;
        if (timeContext.CurrentIdx < _combatMoveRefs.Count) {
            move = _combatMoveRefs[timeContext.CurrentIdx];
        }
        return move;
    }

    public IEnumerable<CombatMoveRef> GetSequence() => _combatMoveRefs;

    public float CompletionStatus(ITimeContextContainer timeContextContainer) {
        if (timeContextContainer == null) {
            return 0;
        }

        var timeContext = timeContextContainer.GetTimeContext<MartialSequence>();

        return timeContext.CurrentTime / _combatMoveRefs.Sum((move) => move.CombatMove.Speed);
    }
}
