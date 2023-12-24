
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;
    public class ConstValueStep : SingleGetterStep, IGetterStep
    {
        [UnityEngine.SerializeField]
        double _value;

        public override double Get() {
            UnityEngine.Debug.Log($"{nameof(ConstValueStep)} = {_value}");
            return _value;
        }

        public override IEnumerable<IPort<EffectStepNode>> Inputs => Enumerable.Empty<IPort<EffectStepNode>>();

        public override string ShortName => "Const";
    }
}