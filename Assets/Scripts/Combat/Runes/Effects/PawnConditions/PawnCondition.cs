using TagFighter.Effects.Triggers;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects
{
    public class PawnCondition : MonoBehaviour
    {
        public Transform Origin { get; set; }
        public EffectContext Context { get; set; }
        public ITrigger ApplyTrigger { get; set; }
        public ITrigger EndTrigger { get; set; }

        protected StatModifierAccessor StatModifier;

        public void Apply() {
            Debug.Log("Apply a new condition");
            Debug.Log($"Origin: {Origin} Caster: {Context.Caster} Target: {Context.EffectLocation}");

            SetStatModifier();

            if (ApplyTrigger != null) {
                ApplyTrigger.TriggerConditionMet += ApplyTrigger_OnTriggerConditionMet;
                ApplyTrigger.Register(this);
            }

            if (EndTrigger != null) {
                EndTrigger.TriggerConditionMet += EndTrigger_OnTriggerConditionMet;
                EndTrigger.Register(this);
            }
        }

        protected virtual void SetStatModifier() {
            StatModifier = StatModifierAccessor.Permanent;
        }

        void EndTrigger_OnTriggerConditionMet(object sender, ConditionTriggerArgs e) {
            Debug.Log($"{nameof(EndTrigger_OnTriggerConditionMet)}");
            Destroy(this);
        }

        void ApplyTrigger_OnTriggerConditionMet(object sender, ConditionTriggerArgs e) {
            Debug.Log($"{nameof(ApplyTrigger_OnTriggerConditionMet)}");

            EffectInput data = new(Context, Context.GetAffectedUnits(), StatModifier);
            foreach (var effect in Context.EffectsToTrigger) {
                effect.DelayedAction(data);
            }
        }

        protected void OnDestroy() {
            if (ApplyTrigger != null) {
                ApplyTrigger.Unregister();
                ApplyTrigger.TriggerConditionMet -= ApplyTrigger_OnTriggerConditionMet;
            }
            if (EndTrigger != null) {
                EndTrigger.Unregister();
                EndTrigger.TriggerConditionMet -= EndTrigger_OnTriggerConditionMet;
            }

            OnDestroySpecific();
        }

        protected virtual void OnDestroySpecific() { }
    }

}
