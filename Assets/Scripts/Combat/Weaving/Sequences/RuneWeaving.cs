using System;
using System.Collections.Generic;
using TagFighter.Effects;
using TagFighter.Martial;

[Serializable]
public class RuneWeaving : ISequence
{
    [UnityEngine.Serialization.FormerlySerializedAs("martialSequence")]
    public MartialSequence MartialSequence;

    [UnityEngine.Serialization.FormerlySerializedAs("runeBindingSequence")]
    public RuneBindingSequence RuneBindingSequence;

    public RuneWeaving(IEnumerable<CombatMoveRef> combatMovesSequence, IEnumerable<RuneRef> runeSequence) {
        MartialSequence = new(combatMovesSequence);
        RuneBindingSequence = new(runeSequence);
    }

    public bool Advance(ITimeContextContainer timeContextContainer, EffectContext effectContext, float deltaTime) {
        var isAdvanced = MartialSequence.Advance(timeContextContainer, effectContext, deltaTime);
        RuneBindingSequence.Advance(timeContextContainer, effectContext, deltaTime);

        return isAdvanced;
    }

    public CombatMove GetCurrentMove(ITimeContextContainer timeContextContainer) {
        return MartialSequence.GetCurrentMove(timeContextContainer);
    }

    public void Simulate() {
        MartialSequence.Simulate();
        RuneBindingSequence.Simulate();
    }

    public float CompletionStatus(ITimeContextContainer timeContextContainer) {
        return MartialSequence.CompletionStatus(timeContextContainer);
    }

    public ITimeContextContainer CreateTimeContexts() {
        return new TimeContextContainer(typeof(RuneBindingSequence), typeof(MartialSequence));
    }
}
