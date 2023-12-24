using System.Threading;

namespace TagFighter.Effects
{
    public class TransientPawnCondition : PawnCondition
    {
        CancellationTokenSource _cancellationSource;

        protected override void SetStatModifier() {
            _cancellationSource = new();
            StatModifier = new(_cancellationSource.Token);
        }

        protected override void OnDestroySpecific() {
            _cancellationSource.Cancel();
            _cancellationSource.Dispose();
        }
    }

}