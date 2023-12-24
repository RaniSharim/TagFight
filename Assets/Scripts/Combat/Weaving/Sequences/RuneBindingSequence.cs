using System;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects;
using UnityEngine;

[Serializable]
public class RuneBindingSequence : ISequence
{

    [UnityEngine.Serialization.FormerlySerializedAs("runeRefs")]
    [SerializeField] List<RuneRef> _runeRefs;
    public RuneBindingSequence(IEnumerable<RuneRef> runeSequence) {
        _runeRefs = runeSequence.ToList();
    }

    // TODO: how are we supposed to return the effects?
    public bool Advance(ITimeContextContainer timeContextContainer, EffectContext effectContext, float deltaTime) {
        var timeContext = timeContextContainer.GetTimeContext<RuneBindingSequence>();

        if (timeContext.CurrentIdx >= _runeRefs.Count) {
            return false;
        }

        timeContext.CurrentTime += deltaTime;
        while ((timeContext.CurrentIdx < _runeRefs.Count) && (timeContext.CurrentTime - timeContext.LastTime >= _runeRefs[timeContext.CurrentIdx].Rune.Speed)) {
            Debug.Log($"Finished executing {_runeRefs[timeContext.CurrentIdx].name} - {timeContext.CurrentIdx + 1}/{_runeRefs.Count} at {timeContext.CurrentTime} ");
            _runeRefs[timeContext.CurrentIdx].Rune.Cast(effectContext);
            timeContext.LastTime += _runeRefs[timeContext.CurrentIdx].Rune.Speed;
            timeContext.CurrentIdx++;
        }
        return true;
    }

    public void Simulate() {

    }

    public IEnumerable<RuneRef> GetSequence() => _runeRefs;
    public float CompletionStatus(ITimeContextContainer timeContextContainer) {
        var timeContext = timeContextContainer.GetTimeContext<RuneBindingSequence>();

        return timeContext.CurrentTime / _runeRefs.Sum((runeRef) => runeRef.Rune.Speed);
    }
}
