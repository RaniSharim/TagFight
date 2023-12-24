
using TagFighter.Effects;

public interface ISequence
{
    // Should be returning an Effect?
    public void Simulate();

    public bool Advance(ITimeContextContainer timeContextContainer, EffectContext effectContext, float deltaTime);

    public float CompletionStatus(ITimeContextContainer timeContextContainer);
}