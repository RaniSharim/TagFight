using System;

namespace TagFighter
{
    public interface IActionRead
    {
        public float CompletionStatus();
        public bool IsSimilarAction(IActionRead action);
    }
    public interface IAction : IActionRead, IDisposable
    {
        public IActionState Advance();
    }

    public interface IActionState
    {
        public bool IsAdvanced { get; }
        public float CompletionStatus { get; }
        public bool IsMoving { get; }
    }
}
