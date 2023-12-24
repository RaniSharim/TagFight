using System.Collections.Generic;
using TagFighter.Resources;

namespace TagFighter.Effects
{
    public interface IResourceLocationAccessor { }
    public interface IResourceLocationGet : IResourceLocationAccessor
    {
        public IEnumerable<Unit<TUnit>> Get<TResource, TUnit>(EffectInput data)
            where TResource : Resource<TUnit>
            where TUnit : IUnitType;
    }
    public interface IResourceLocationSet : IResourceLocationAccessor
    {
        public void Set<TResource, TUnit>(EffectInput data, IEnumerable<Unit<TUnit>> values)
            where TResource : Resource<TUnit>
            where TUnit : IUnitType;
    }
}
