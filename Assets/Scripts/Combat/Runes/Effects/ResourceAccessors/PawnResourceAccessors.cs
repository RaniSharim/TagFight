using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Resources;
using UnityEngine;


namespace TagFighter.Effects.ResourceLocationAccessors
{
    namespace PawnProperties
    {
        public interface IPawnResourceProperty
        {
            public Unit<TUnit> Get<TResource, TUnit>(TResource resource)
                where TResource : IAccessibleResource<TUnit> where TUnit : IUnitType;
            public void Set<TResource, TUnit>(EffectInput data, TResource resource, Unit<TUnit> value)
                where TResource : Resource<TUnit> where TUnit : IUnitType;
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Current : IPawnResourceProperty
        {
            public Unit<TUnit> Get<TResource, TUnit>(TResource resource)
                where TResource : IAccessibleResource<TUnit> where TUnit : IUnitType => resource.GetCurrent();
            public void Set<TResource, TUnit>(EffectInput data, TResource resource, Unit<TUnit> value)
                where TResource : Resource<TUnit> where TUnit : IUnitType => data.StatAccessor.AddCurrentModifier(resource, value);

            public override string ToString() => nameof(Current);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Capacity : IPawnResourceProperty
        {
            public Unit<TUnit> Get<TResource, TUnit>(TResource resource)
                where TResource : IAccessibleResource<TUnit> where TUnit : IUnitType => resource.GetCapacity();
            public void Set<TResource, TUnit>(EffectInput data, TResource resource, Unit<TUnit> value)
                where TResource : Resource<TUnit> where TUnit : IUnitType => data.StatAccessor.AddCapacityModifier(resource, value);

            public override string ToString() => nameof(Capacity);
        }
    }

    namespace Get
    {
        using PawnProperties;

        [ProvideSourceInfo]
        [Serializable]
        public class Pawn : IResourceLocationGet
        {
            [SerializeReference, ShowSerializeReference]
            public IPawnResourceProperty Property;
            public IEnumerable<Unit<TUnit>> Get<TResource, TUnit>(EffectInput data)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                return data.Affected.Select(transform => transform.GetComponent<TResource>())
                                    .Select(resource => resource ? Property.Get<TResource, TUnit>(resource) : (Unit<TUnit>)0);
            }

            public override string ToString() => $"{nameof(Pawn)}.{Property}";
        }
    }

    namespace Set
    {
        using PawnProperties;

        [ProvideSourceInfo]
        [Serializable]
        public class Pawn : IResourceLocationSet
        {
            [SerializeReference, ShowSerializeReference]
            public IPawnResourceProperty Property;
            public void Set<TResource, TUnit>(EffectInput data, IEnumerable<Unit<TUnit>> values)
                where TResource : Resource<TUnit>
                where TUnit : IUnitType {
                var repeatedValues = Enumerable.Repeat(values.ToList(), int.MaxValue).SelectMany(value => value);
                foreach (var tuple in data.Affected.Zip(repeatedValues, (transform, value) => (resource: transform.GetComponent<TResource>(), value))
                                                   .Where(tuple => tuple.resource)) {
                    Property.Set(data, tuple.resource, tuple.value);
                }
            }

            public override string ToString() => $"{nameof(Pawn)}.{Property}";
        }
    }
}
