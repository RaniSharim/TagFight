using System.Collections.Generic;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects
{
    public interface IEffect
    {
        void Apply(EffectInput data);
    }

    public class EffectInput
    {
        public EffectContext Context;
        public IEnumerable<Transform> Affected;
        public IStatAccessor StatAccessor;

        public EffectInput(EffectContext context, IEnumerable<Transform> affected, IStatAccessor statAccessor) {
            Context = context;
            Affected = affected;
            StatAccessor = statAccessor;
        }
    }
}