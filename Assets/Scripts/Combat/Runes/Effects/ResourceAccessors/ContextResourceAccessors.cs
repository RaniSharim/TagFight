using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CareBoo.Serially;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects.ResourceLocationAccessors
{
    namespace ContextRegisters
    {
        public interface IRegisterType { }

        public class CurrentRegisterType : IRegisterType { }
        public class AddedRegisterType : IRegisterType { }
        public class RemovedRegisterType : IRegisterType { }

        public interface IContextRegister
        {
            public IEnumerable<Unit<TUnit>> Get<TResource, TUnit>(EffectContext context)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType;

            public void Set<TResource, TUnit>(EffectContext context, Unit<TUnit> value)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType;
        }

        public class ContextRegister<TRegisterType> : IContextRegister where TRegisterType : IRegisterType
        {
            public IEnumerable<Unit<TUnit>> Get<TResource, TUnit>(EffectContext context)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                yield return context.GetResource<TResource, TUnit, ContextRegister<TRegisterType>>();
            }

            public void Set<TResource, TUnit>(EffectContext context, Unit<TUnit> value)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                context.SetResource<TResource, TUnit, ContextRegister<TRegisterType>>(value);
            }

            public override string ToString() => typeof(TRegisterType).Name;

        }

        [Guid("47EAE1FF-9FF5-41FD-B08E-0F674B3848E6")][ProvideSourceInfo][Serializable] public sealed class Current : ContextRegister<CurrentRegisterType> { }
        [Guid("F61D55E0-7CE7-4C9A-B241-00B296A36CF3")][ProvideSourceInfo][Serializable] public sealed class Added : ContextRegister<AddedRegisterType> { }
        [Guid("AAB1BF8C-B85E-4EFE-AB12-F24966BEBDAF")][ProvideSourceInfo][Serializable] public sealed class Removed : ContextRegister<RemovedRegisterType> { }

    }

    namespace Get
    {
        using ContextRegisters;

        [ProvideSourceInfo]
        [Serializable]
        public class Context : IResourceLocationGet
        {
            [SerializeReference, ShowSerializeReference]
            public IContextRegister Register;
            public IEnumerable<Unit<TUnit>> Get<TResource, TUnit>(EffectInput data)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                return Register.Get<TResource, TUnit>(data.Context);
            }

            public override string ToString() => $"{nameof(Context)}.{Register}";
        }
    }

    namespace Set
    {
        using ContextRegisters;

        [ProvideSourceInfo]
        [Serializable]
        public class Context : IResourceLocationSet
        {
            [SerializeReference, ShowSerializeReference]
            public IContextRegister Register;

            [SerializeReference, ShowSerializeReference]
            public IResourceOperator SetAs;

            public void Set<TResource, TUnit>(EffectInput data, IEnumerable<Unit<TUnit>> values)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                Register.Set<TResource, TUnit>(data.Context, SetAs.OperateEnum(values));
            }

            public override string ToString() => $"{nameof(Context)}.{Register}";
        }
    }
}
