
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;

    public class OperatorStep : SingleGetterStep, IOperatorStep
    {
        [UnityEngine.SerializeField]
        MultiPort<SingleGetterStep> _in = new();

        [UnityEngine.SerializeReference, CareBoo.Serially.ShowSerializeReference]
        public IResourceOperator Operator;

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName {
            get {
                var operatorName = Operator == null ? "*" : Operator.GetType().Name;

                return new($"{operatorName}");
            }
        }

        public override double Get() {
            return Operator.OperateEnum(_in.Nodes.Select(getter => getter.Get()));
        }
    }
}