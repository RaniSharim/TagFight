
namespace TagFighter.Effects
{
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;
    using TagFighter.Effects.ResourceLocationAccessors.ContextRegisters;

    public class ContextGetterStep : SingleGetterStep, IGetterStep
    {
        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(Resources.Resource<>))]
        public CareBoo.Serially.SerializableType Resource;

        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(ContextRegister<>))]
        public CareBoo.Serially.SerializableType Register;

        public override double Get() {
            if (Register.Type == null || Resource.Type == null) {
                UnityEngine.Debug.Log($"{Guid} Inner types are null");
                return 0;
            }

            var getSpecificMethod = typeof(ContextGetterStep).GetMethod(nameof(GetSpecific), BindingFlags.NonPublic | BindingFlags.Instance).
                                            MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0], Register.Type);
            var value = getSpecificMethod.Invoke(this, null);

            return (int)value;
        }

        int GetSpecific<TResource, TUnitType, TContextRegister>()
            where TResource : Resources.Resource<TUnitType>
            where TUnitType : Resources.IUnitType
            where TContextRegister : IContextRegister {
            UnityEngine.Debug.Log($"{nameof(ContextGetterStep)} {typeof(TResource)} {typeof(TContextRegister)}");
            return Data.Context.GetResource<TResource, TUnitType, TContextRegister>().AsPrimitive();
        }

        public override IEnumerable<IPort<EffectStepNode>> Inputs => Enumerable.Empty<IPort<EffectStepNode>>();

        public override string ShortName {
            get {
                var resourceName = Resource.Type == null ? "*" : Resource.Type.Name;
                var registerName = Register.Type == null ? "*" : Register.Type.Name;

                return new($"Context.Get.{resourceName}.{registerName}");
            }
        }
    }
}