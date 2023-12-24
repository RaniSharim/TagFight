
namespace TagFighter.Testing.Optics
{
    using CareBoo.Serially;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TagFighter.Effects;
    using TagFighter.Effects.ResourceLocationAccessors.ContextRegisters;
    using TagFighter.Resources;
    using UnityEngine;

    public interface ILens<Whole, Part>
    {
        Part Get(Whole whole);
        Whole Set(Part part, Whole whole);
    }

    public readonly struct Lens<TWhole, TPart>
    {

        public readonly Func<TWhole, TPart> Get;

        public readonly Func<TWhole, TPart, TWhole> Set;

        public Lens(Func<TWhole, TPart> getter, Func<TWhole, TPart, TWhole> setter) {
            Get = getter ?? throw new ArgumentNullException(nameof(getter));
            Set = setter ?? throw new ArgumentNullException(nameof(setter));
        }
    }

    public static class LensExtensions
    {
        public static Lens<TWhole, TPart> Lens<TWhole, TPart>(
            this TWhole _,
            Func<TWhole, TPart> getter, Func<TWhole, TPart, TWhole> setter) =>
            new(getter, setter);

        //public static TWhole Update<TWhole, TPart>(
        //    this Lens<TWhole, TPart> lens,
        //    TWhole whole, Func<TPart, TPart> update) =>
        //    lens.Set(whole, update(lens.Get(whole)));

        public static Lens<TWhole, TSubPart> Compose<TWhole, TPart, TSubPart>(
            this Lens<TWhole, TPart> parent, Lens<TPart, TSubPart> child) =>
            new(
              whole => child.Get(parent.Get(whole)),
              (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part)));
    }

    public interface IResourceStatLensCreator
    {
        public Lens<TResource, Unit<TUnitType>> Create<TResource, TUnitType>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType;
    }

    [ProvideSourceInfo]
    [Serializable]
    public class ResourceCurrentLensCreator : IResourceStatLensCreator
    {
        public Lens<TResource, Unit<TUnitType>> Create<TResource, TUnitType>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType {
            return new Lens<TResource, Unit<TUnitType>>(r => r.GetCurrent(), (r, u) => r);
        }
    }

    [ProvideSourceInfo]
    [Serializable]
    public class ResourceCapacityLensCreator : IResourceStatLensCreator
    {
        public Lens<TResource, Unit<TUnitType>> Create<TResource, TUnitType>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType {
            return new Lens<TResource, Unit<TUnitType>>(r => r.GetCapacity(), (r, u) => r);
        }
    }

    public interface ILocationAccessor { };
    public interface IPawnAccessor
    {
        public Lens<Transform, int> CreateSpecific<TResource, TUnitType>() where TResource : Resource<TUnitType> where TUnitType : IUnitType;
    };

    [ProvideSourceInfo]
    [Serializable]
    public class PawnAccessor : IPawnAccessor, ILocationAccessor
    {
        [SerializeReference, ShowSerializeReference]
        public IResourceStatLensCreator ResourceLensCreator; // Current/Capacity


        public Lens<Transform, int> CreateSpecific<TResource, TUnitType>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType {
            var resource = new Lens<Transform, TResource>(t => t.GetComponent<TResource>(), (t, r) => t);
            var statLens = ResourceLensCreator.Create<TResource, TUnitType>();
            var valueLens = new Lens<Unit<TUnitType>, int>(u => u.AsPrimitive(), (u, v) => u = (Unit<TUnitType>)v);
            var lens = resource.Compose(statLens).Compose(valueLens);
            return lens;
        }
    }

    public interface IContextAccessor
    {
        public Lens<EffectContext, int> CreateSpecificResource<TResource, TUnitType>()
            where TResource : Resource<TUnitType> where TUnitType : IUnitType;
        public Lens<EffectContext, int> CreateSpecific<TResource, TUnitType, TContextRegister>()
            where TResource : Resource<TUnitType> where TUnitType : IUnitType where TContextRegister : IContextRegister;
    }

    [ProvideSourceInfo]
    [Serializable]
    public class ContextAccessor : IContextAccessor, ILocationAccessor
    {
        [TypeFilter(derivedFrom: typeof(ContextRegister<>))]
        [SerializeField]
        public SerializableType Register;

        [SerializeReference, ShowSerializeReference]
        public IResourceOperator Fold;

        public Lens<EffectContext, int> CreateSpecificResource<TResource, TUnitType>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType {
            var createSpecificLensMethod = typeof(ContextAccessor).GetMethod(nameof(CreateSpecific)).
                                MakeGenericMethod(typeof(TResource), typeof(TUnitType), Register.Type);

            var lens = createSpecificLensMethod.Invoke(this, null);
            return (Lens<EffectContext, int>)lens;
        }

        public Lens<EffectContext, int> CreateSpecific<TResource, TUnitType, TContextRegister>()
            where TResource : Resource<TUnitType>
            where TUnitType : IUnitType
            where TContextRegister : IContextRegister {
            var registerLens = new Lens<EffectContext, Unit<TUnitType>>(
                context => context.GetResource<TResource, TUnitType, TContextRegister>(),
                (context, v) => {
                    context.SetResource<TResource, TUnitType, TContextRegister>(v);
                    return context;
                });
            var valueLens = new Lens<Unit<TUnitType>, int>(u => u.AsPrimitive(), (u, v) => u = (Unit<TUnitType>)v);
            return registerLens.Compose(valueLens);
        }
    }


    [ProvideSourceInfo]
    [Serializable]
    public class EffectDataAccessor
    {
        [TypeFilter(derivedFrom: typeof(Resource<>))]
        [SerializeField]
        public SerializableType Resource;

        [SerializeReference, ShowSerializeReference] ILocationAccessor _location;

        public IEnumerable<int> Get(EffectInput ei) {
            switch (_location) {
                case PawnAccessor accessor:
                    var pawnLens = PawnLens(accessor);
                    return ei.Affected.Select(p => pawnLens.Get(p));
                case ContextAccessor accessor:
                    var contextLens = ContextLens(accessor);
                    return Enumerable.Empty<int>().Append(contextLens.Get(ei.Context));
                default:
                    return Enumerable.Empty<int>();
            }
        }

        public EffectInput Set(EffectInput ei, IEnumerable<int> values) {
            switch (_location) {
                case PawnAccessor accessor:
                    var pawnLens = PawnLens(accessor);
                    var repeatedValues = Enumerable.Repeat(values.ToList(), int.MaxValue).SelectMany(value => value);
                    foreach (var tuple in ei.Affected.Zip(repeatedValues, (transform, value) => (transform, value))) {
                        pawnLens.Set(tuple.transform, tuple.value);
                    }
                    break;
                case ContextAccessor accessor:
                    var contextLens = ContextLens(accessor);
                    contextLens.Set(ei.Context, accessor.Fold.OperateEnum(values));
                    break;
            }
            return ei;
        }


        Lens<Transform, int> PawnLens(PawnAccessor accessor) {
            var createSpecificLensMethod = typeof(PawnAccessor).GetMethod(nameof(PawnAccessor.CreateSpecific)).
                                MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0]);

            var lens = createSpecificLensMethod.Invoke(accessor, null);
            return (Lens<Transform, int>)lens;
        }

        Lens<EffectContext, int> ContextLens(ContextAccessor accessor) {
            var createSpecificLensMethod = typeof(ContextAccessor).GetMethod(nameof(ContextAccessor.CreateSpecificResource)).
                                MakeGenericMethod(Resource.Type, Resource.Type.BaseType.GetGenericArguments()[0]);

            var lens = createSpecificLensMethod.Invoke(accessor, null);
            return (Lens<EffectContext, int>)lens;
        }
    }
}