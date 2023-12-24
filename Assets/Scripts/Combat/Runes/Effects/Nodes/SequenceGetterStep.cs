// using System.Collections;
// using UnityEngine;
using System.Collections.Generic;

namespace TagFighter.Effects
{
    public abstract class SequenceGetterStep : EffectStepNode
    {
        public abstract IEnumerable<double> Get();
    }

}