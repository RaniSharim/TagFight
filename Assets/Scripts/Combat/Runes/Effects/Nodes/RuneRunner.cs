using UnityEngine;

namespace TagFighter.Effects
{

    public class RuneRunner : MonoBehaviour
    {
        [ContextMenuItem("Cast Rune", "Cast")]
        public RuneRef Rune;

        public Transform Target;
        public Transform Caster;

        void Cast() {
            if (Rune == null || Target == null || Caster == null) {
                return;
            }
            Debug.Log($"Casting {Rune} By {Caster} On {Target}");
            var context = new EffectContext() {
                EffectLocation = Target,
                Caster = Caster,
            };

            Rune.Cast(context);
        }

    }

}