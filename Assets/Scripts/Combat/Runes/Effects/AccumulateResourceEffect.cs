using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using UnityEngine;

namespace TagFighter.Effects
{
    [ProvideSourceInfo]
    [Serializable]
    public class UnaryResourceEffect : IEffect
    {
        public ResourceInfoGet<IResourceTypeAccessor, IResourceLocationGet> From;
        public ResourceInfoSet<IResourceTypeAccessor, IResourceLocationSet> To;

        public void Apply(EffectInput data) {
            var effectName = $"UnaryEffect ({From}) To ({To})";

            Debug.Log($"{effectName} : Apply");

            var resourceFrom = From.Get(data);

            To.Set(data, resourceFrom);
        }
    }

    [ProvideSourceInfo]
    [Serializable]
    public class BinaryResourceEffect : IEffect
    {
        [SerializeReference, ShowSerializeReference]
        public IResourceOperator Operator;
        public ResourceInfoGet<IResourceTypeAccessor, IResourceLocationGet> FromA;
        public ResourceInfoGet<IResourceTypeAccessor, IResourceLocationGet> FromB;
        public ResourceInfoSet<IResourceTypeAccessor, IResourceLocationSet> To;

        public void Apply(EffectInput data) {
            var effectName = $"BinaryEffect({FromA} {Operator} {FromB})To({To})";

            if (Operator == null) {
                Debug.Log($"{effectName}: Apply missing operator, skipping");
                return;
            }

            Debug.Log($"{effectName} : Apply");

            var resourceFromA = FromA.Get(data).ToList();
            var resourceFromB = FromB.Get(data).ToList();

            var manipulated = Combine(resourceFromA, resourceFromB);

            To.Set(data, manipulated);

        }

        IEnumerable<double> Combine(List<double> a, List<double> b) {
            var (shortList, maybeLongList) = a.Count switch {
                1 => (a, b),
                _ => (b, a)
            };
            var repeatedShortList = Enumerable.Repeat(shortList, int.MaxValue).SelectMany((val) => val);
            return maybeLongList.Zip(repeatedShortList, (x, y) => Operator.Operate(x, y));
        }
    }
}
