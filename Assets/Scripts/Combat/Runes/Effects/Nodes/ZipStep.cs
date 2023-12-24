
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;
    using CareBoo.Serially;

    public class ZipStep : SequenceGetterStep, IOperatorStep
    {
        [UnityEngine.SerializeField]
        MultiPort<SequenceGetterStep> _in = new();

        [UnityEngine.SerializeReference, ShowSerializeReference]
        public IResourceOperator Operator;

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override IEnumerable<double> Get() {
            return _in.Nodes.Select(getter => getter.Get())
            .Aggregate((current, next) =>
                        current.Zip(next, (first, second) =>
                        Operator.Operate(first, second)));
        }

        public override string ShortName {
            get {
                var operatorName = Operator == null ? "*" : Operator.GetType().Name;

                return new($"Zip.{operatorName}");
            }
        }
    }

}