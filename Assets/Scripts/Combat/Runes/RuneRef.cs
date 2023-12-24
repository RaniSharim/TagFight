using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects;
using UnityEngine;

[CreateAssetMenu(fileName = "Rune", menuName = "Game/Combat/Rune")]

public class RuneRef : ScriptableObject//, ISerializationCallbackReceiver
{
    public Sprite RuneSprite;

    [UnityEngine.Serialization.FormerlySerializedAs("rune")]
    public Rune Rune;

    public List<RuneEffect> RuneEffects = new();

    public void Cast(EffectContext context) {
        foreach (var runeEffect in RuneEffects) {
            var effectData = new EffectInput(context, new Transform[] { context.EffectLocation }, TagFighter.Resources.StatModifierAccessor.Permanent);
            var root = runeEffect.Nodes.Where(effectStep => effectStep is EffectEndNode).FirstOrDefault() as EffectEndNode;
            if (root == null) {
                UnityEngine.Debug.Log("No Root Step For Effect");
            }
            else {
                foreach (var effect in runeEffect.Nodes) {
                    effect.Data = effectData;
                }
                root.Run();
            }

        }
    }

    public static implicit operator Rune(RuneRef runeRef) => runeRef.Rune;
}
