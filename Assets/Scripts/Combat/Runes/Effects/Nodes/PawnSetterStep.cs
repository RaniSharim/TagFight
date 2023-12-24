

namespace TagFighter.Effects
{
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Collections.Generic;
    // using System.Linq;

    public class PawnSetterStep : SetterNode, ISetterStep
    {
        [UnityEngine.SerializeField]
        SinglePort<SequenceGetterStep> _in = new();

        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(Resources.Resource<>))]
        public CareBoo.Serially.SerializableType Resource;

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName {
            get {
                var resourceName = Resource.Type == null ? "*" : Resource.Type.Name;
                var statName = "Current";

                return new($"Pawn.Set.{resourceName}.{statName}");
            }
        }

        public override void Set() {
            if (Resource.Type == null) {
                UnityEngine.Debug.Log($"{Guid} Inner types are null");
                return;
            }
            var getSpecificMethod = typeof(PawnSetterStep).GetMethod(nameof(SetSpecific), BindingFlags.NonPublic | BindingFlags.Instance).
                                            MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0]);
            var value = getSpecificMethod.Invoke(this, null);

        }
        void SetSpecific<TResource, TUnitType>()
            where TResource : Resources.Resource<TUnitType>
            where TUnitType : Resources.IUnitType {

            var result = _in.Node.Get();
            var message = new StringBuilder();
            foreach (var res in result) {
                message.Append($"{res} ,");
            }
            UnityEngine.Debug.Log($"{nameof(PawnSetterStep)} result {message}");

            var set = result.Zip(Data.Affected.Select(transform => transform.GetComponent<TResource>()), (value, resource) => (value, resource));
            foreach (var (value, resource) in set) {
                Data.StatAccessor.AddCurrentModifier(resource, (Resources.Unit<TUnitType>)value);
            }
        }
    }
}