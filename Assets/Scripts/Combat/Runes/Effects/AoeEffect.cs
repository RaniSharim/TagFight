using System;
using System.Linq;
using CareBoo.Serially;
using UnityEngine;

namespace TagFighter.Effects
{

    [ProvideSourceInfo]
    [Serializable]
    public class AoeEffect : IEffect
    {
        [SerializeReference, ShowSerializeReference]
        public AoeShapes.IAoeShape Shape;

        public void Apply(EffectInput data) {
            data.Context.AreaOfEffect = Shape.AreaOfEffect(data);
        }
    }

    namespace AoeShapes
    {
        public interface IAoeShape
        {
            public IAreaOfEffect AreaOfEffect(EffectInput data);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Single : IAoeShape
        {
            SingleTarget _areaOfEffect;

            public IAreaOfEffect AreaOfEffect(EffectInput data) {
                return _areaOfEffect.ShallowCopy();
            }
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Circle : IAoeShape
        {
            public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Radius;
            CircleArea _areaOfEffect;

            public IAreaOfEffect AreaOfEffect(EffectInput data) {
                /// Currently Radius is a Context getter so there's only a single value in the iterator.
                /// To support get from pawns, requires somekind of aggregation operator from <see cref="IResourceOperator"/>
                /// to be included in this 
                _areaOfEffect.Radius = (float)Radius.Get(data).First();
                return _areaOfEffect.ShallowCopy();
            }
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Cone : IAoeShape
        {
            public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Radius;
            public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Angle;
            ConeArea _areaOfEffect;

            public IAreaOfEffect AreaOfEffect(EffectInput data) {
                /// Currently Radius and Angle are a Context getter so there's only a single value in the iterator.
                /// To support get from pawns, requires somekind of aggregation operator from <see cref="IResourceOperator"/>
                /// to be included in this 
                _areaOfEffect.Radius = (float)Radius.Get(data).First();
                _areaOfEffect.Angle = (float)Angle.Get(data).First();
                return _areaOfEffect.ShallowCopy();
            }
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Path : IAoeShape
        {
            public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Width;
            public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Length;
            PathArea _areaOfEffect;


            public IAreaOfEffect AreaOfEffect(EffectInput data) {
                /// Currently Radius and Length are a Context getter so there's only a single value in the iterator.
                /// To support get from pawns, requires somekind of aggregation operator from <see cref="IResourceOperator"/>
                /// to be included in this 
                _areaOfEffect.Width = (float)Width.Get(data).First();
                _areaOfEffect.Length = (float)Length.Get(data).First();
                return _areaOfEffect.ShallowCopy();
            }
        }
    }


}
