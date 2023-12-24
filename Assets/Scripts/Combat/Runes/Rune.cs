using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Effects;
using UnityEngine;

[Serializable]
public class Rune
{
    public string DisplayName;
    public float Speed;
    public float ManaCost;
    public List<EffectTypePair> Effects = new();

    public void Cast(EffectContext context) {
        foreach (var effect in Effects.Where(effect => effect.Mode != null)) {
            effect.Mode.ImmediateAction(context, effect.Effect);
        }
    }
}

[Serializable]
public class EffectTypePair
{
    [SerializeReference, ShowSerializeReference]
    public IImmediateEffect Mode = null;

    [SerializeReference, ShowSerializeReference]
    public IEffect Effect;

}