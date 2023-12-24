
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;

    public class FoldStep : SingleGetterStep, IOperatorStep
    {
        [UnityEngine.SerializeField]
        SinglePort<SequenceGetterStep> _in = new();

        [UnityEngine.SerializeReference, CareBoo.Serially.ShowSerializeReference]
        public IResourceOperator Operator;

        public override double Get() {
            return _in.Node.Get().Aggregate((current, next) => Operator.Operate(current, next));
        }

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName {
            get {
                var operatorName = Operator == null ? "*" : Operator.GetType().Name;

                return new($"Fold.{operatorName}");
            }
        }
    }

}