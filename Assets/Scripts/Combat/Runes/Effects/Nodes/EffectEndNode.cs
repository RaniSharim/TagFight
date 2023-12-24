

namespace TagFighter.Effects
{
    using System.Collections.Generic;
    // using System.Linq;

    public class EffectEndNode : EffectStepNode
    {
        [UnityEngine.SerializeField]
        SinglePort<SetterNode> _in = new();

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName => "Root";

        // public override IEnumerable<IPort<EffectStepNode>> Outputs => Enumerable.Empty<IPort<EffectStepNode>>();

        public void Run() {
            _in.Node.Set();
        }
    }

}
