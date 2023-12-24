
namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class PawnGetterStep : SequenceGetterStep, IGetterStep
    {
        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(Resources.Resource<>))]
        public CareBoo.Serially.SerializableType Resource;

        public override IEnumerable<double> Get() {
            if (Resource.Type == null) {
                UnityEngine.Debug.Log($"{Guid} Inner types are null");
                return Data.Affected.Select(transform => 0d);
            }

            var getSpecificMethod = typeof(PawnGetterStep).GetMethod(nameof(GetSpecific), BindingFlags.NonPublic | BindingFlags.Instance).
                                            MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0]);
            var value = getSpecificMethod.Invoke(this, null);

            return value as IEnumerable<double>;
        }

        IEnumerable<double> GetSpecific<TResource, TUnitType>()
            where TResource : Resources.Resource<TUnitType>
            where TUnitType : Resources.IUnitType {
            UnityEngine.Debug.Log($"{nameof(PawnGetterStep)} {typeof(TResource)}");
            return Data.Affected.Select(transform => (double)transform.GetComponent<TResource>().GetCurrent().AsPrimitive());
        }
        public override IEnumerable<IPort<EffectStepNode>> Inputs => Enumerable.Empty<IPort<EffectStepNode>>();

        public override string ShortName {
            get {
                var resourceName = Resource.Type == null ? "*" : Resource.Type.Name;
                var statName = "Current";

                return new($"Pawn.Get.{resourceName}.{statName}");
            }
        }
    }

}
