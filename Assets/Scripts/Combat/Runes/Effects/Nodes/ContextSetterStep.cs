

namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using System.Reflection;
    using TagFighter.Effects.ResourceLocationAccessors.ContextRegisters;

    public class ContextSetterStep : SetterNode, ISetterStep
    {
        [UnityEngine.SerializeField]
        SinglePort<SingleGetterStep> _in = new();
        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(Resources.Resource<>))]
        public CareBoo.Serially.SerializableType Resource;

        [CareBoo.Serially.TypeFilter(derivedFrom: typeof(ContextRegister<>))]
        public CareBoo.Serially.SerializableType Register;
        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _in;
            }
        }

        public override string ShortName {
            get {
                var resourceName = Resource.Type == null ? "*" : Resource.Type.Name;
                var registerName = Register.Type == null ? "*" : Register.Type.Name;

                return new($"Context.Set.{resourceName}.{registerName}");
            }
        }

        public override void Set() {
            if (Register.Type == null || Resource.Type == null) {
                UnityEngine.Debug.Log($"{Guid} Inner types are null");
                return;
            }

            var getSpecificMethod = typeof(ContextSetterStep).GetMethod(nameof(SetSpecific), BindingFlags.NonPublic | BindingFlags.Instance).
                                            MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0], Register.Type);
            var value = getSpecificMethod.Invoke(this, null);

        }

        void SetSpecific<TResource, TUnitType, TContextRegister>()
            where TResource : Resources.Resource<TUnitType>
            where TUnitType : Resources.IUnitType
            where TContextRegister : IContextRegister {

            var result = _in.Node.Get();
            UnityEngine.Debug.Log($"{nameof(ContextSetterStep)} {typeof(TResource)} {typeof(TContextRegister)} result {result}");

            Data.Context.SetResource<TResource, TUnitType, TContextRegister>((Resources.Unit<TUnitType>)result);
        }

    }
}