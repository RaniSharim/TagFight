using System;
using System.Collections.Generic;
using CareBoo.Serially;
using TagFighter.Resources;
using UnityEngine;


namespace TagFighter.Effects
{
    [CreateAssetMenu(fileName = "NewEffectColors", menuName = "Game/Misc/EffectColors")]
    public class EffectColors : ScriptableObject
    {
        [SerializeField] List<ColorMapping<Resource<IUnitType>, IUnitType>> _colorMapping;
        public Color this[Type resource] {
            get {
                var mappedcolor = _colorMapping.Find(m => m.Resource.Type == resource);
                if (mappedcolor != null) {
                    return mappedcolor.GetColor();
                }
                throw new IndexOutOfRangeException($"has no mapping for ${resource.GetType()}");
            }
        }

        public bool ContainsKey(Type resource) {
            return _colorMapping.Find(m => m.Resource.Type == resource) != null;
        }
    }

    [Serializable]
    public class ColorMapping<TResource, TUnit>
        where TResource : Resource<TUnit>
        where TUnit : IUnitType
    {

        [TypeFilter(derivedFrom: typeof(IWatchableResource))]
        public SerializableType Resource;


        [SerializeField]
        Color _color;

        public Color GetColor() { return _color; }
    }
}
