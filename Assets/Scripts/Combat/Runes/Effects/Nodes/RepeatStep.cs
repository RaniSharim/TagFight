
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;

    public class RepeatStep : SequenceGetterStep, IOperatorStep
    {
        [UnityEngine.SerializeField]
        SinglePort<SingleGetterStep> _in = new();

        public override IEnumerable<double> Get() {
            var value = _in.Node.Get();
            return Data.Affected.Select(transform => value);
        }

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName => "Repeat";
    }

}