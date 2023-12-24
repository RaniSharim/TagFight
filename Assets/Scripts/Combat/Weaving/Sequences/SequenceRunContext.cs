using System.Collections.Generic;
public class SequenceRunContext : ISequenceRunContextContainer
{
    public float CurrentTime { get; set; }
    public float LastRunTime { get; set; }
    public int CurrentIdx { get; set; }

    public SequenceRunContext GetRunContext(string sequenceType) { return this; }
}

public class SequenceRunContextContainer : ISequenceRunContextContainer
{
    SequenceRunContext _matrialContext = new();
    SequenceRunContext _runeWeaveContext = new();

    public SequenceRunContext GetRunContext(string sequenceType) {
        return sequenceType switch {
            nameof(MartialSequence) => _matrialContext,
            nameof(RuneBindingSequence) => _runeWeaveContext,
            _ => throw new KeyNotFoundException(),
        };
    }
}

public interface ISequenceRunContextContainer
{
    SequenceRunContext GetRunContext(string sequenceType);
}