using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Effects.ResourceLocationAccessors.PawnProperties;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects
{
    public interface IResourceGetter
    {
        public IWatchableResource GetWatchableResource(Transform transform);
        public int GetStat(Transform transform, IPawnResourceProperty stat);
    }

    public interface IResourceTypeAccessor
    {
        public IEnumerable<double> Get(EffectInput data, IResourceLocationGet accessor);
        public void Set(EffectInput data, IResourceLocationSet accessor, IEnumerable<int> value);
    }

    public class ResourceTypeAccessor<TResource, TUnit> : IResourceTypeAccessor, IResourceGetter, IEquatable<ResourceTypeAccessor<TResource, TUnit>>
        where TResource : Resource<TUnit>
        where TUnit : IUnitType
    {
        public IEnumerable<double> Get(EffectInput data, IResourceLocationGet accessor) {
            return accessor.Get<TResource, TUnit>(data).Select(resource => (double)resource);
        }

        public void Set(EffectInput data, IResourceLocationSet accessor, IEnumerable<int> values) {
            accessor.Set<TResource, TUnit>(data, values.Select(value => (Unit<TUnit>)value));
        }

        public IWatchableResource GetWatchableResource(Transform transform) {
            return transform.GetComponent<TResource>();
        }

        public int GetStat(Transform transform, IPawnResourceProperty stat) {
            return (int)stat.Get<TResource, TUnit>(transform.GetComponent<TResource>());
        }

        public override string ToString() => $"{typeof(TResource).Name}";

        public bool Equals(ResourceTypeAccessor<TResource, TUnit> other) {
            if ((other == null) || !GetType().Equals(other.GetType())) {
                return false;
            }

            return true;
        }

        public override bool Equals(object other) {
            if ((other == null) || !GetType().Equals(other.GetType())) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }

    namespace ResourceTypeAccessors
    {
        [ProvideSourceInfo][Serializable] public sealed class Pain : ResourceTypeAccessor<Resources.Pain, PainUnit> { }
        [ProvideSourceInfo][Serializable] public sealed class Fatigue : ResourceTypeAccessor<Resources.Fatigue, FatigueUnit> { }
        [ProvideSourceInfo][Serializable] public sealed class BlueTag : ResourceTypeAccessor<Resources.BlueTag, BlueTagUnit> { }
        [ProvideSourceInfo][Serializable] public sealed class RedTag : ResourceTypeAccessor<Resources.RedTag, RedTagUnit> { }
        [ProvideSourceInfo][Serializable] public sealed class GreenTag : ResourceTypeAccessor<Resources.GreenTag, GreenTagUnit> { }
        [ProvideSourceInfo][Serializable] public sealed class Range : ResourceTypeAccessor<Resources.Range, Centimeter> { }
        [ProvideSourceInfo][Serializable] public sealed class RecklessFormKnowledge : ResourceTypeAccessor<Resources.RecklessFormKnowledge, RecklessFormKP> { }
        [ProvideSourceInfo][Serializable] public sealed class SwordKnowledge : ResourceTypeAccessor<Resources.SwordKnowledge, SwordKP> { }
        [ProvideSourceInfo][Serializable] public sealed class ShieldKnowledge : ResourceTypeAccessor<Resources.ShieldKnowledge, ShieldKP> { }

    }
}
