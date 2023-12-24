using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Effects.ResourceLocationAccessors.ContextRegisters;
using TagFighter.Resources;
using UnityEngine;


namespace TagFighter.Effects
{
    public interface IDelayedEffect
    {
        void DelayedAction(EffectInput data);
    }

    public interface IImmediateEffect
    {
        void ImmediateAction(EffectContext context, IEffect effect);
    }
    public interface IEffectMode
    {
        void Apply(EffectContext context);
        void Effect(EffectContext context, IEffect effect);
    }

    [ProvideSourceInfo]
    [Serializable]
    public class DelayedEffect : IImmediateEffect, IDelayedEffect
    {
        IEffect _effect;

        public void DelayedAction(EffectInput data) {
            _effect.Apply(data);

            // direction towards target
            var directionVector = (data.Context.EffectLocation.position - data.Context.Caster.position).normalized;
            directionVector.y = 0;
            var direction = Quaternion.LookRotation(directionVector, Vector3.up);

            var appliedResource = data.Context.GetAllResourcesInRegister<Added>();

            data.Context.EffectSystem.ApplyTagsEffect(appliedResource, data.Context.EffectLocation, direction, data.Context.AreaOfEffect);
        }

        public void ImmediateAction(EffectContext context, IEffect effect) {
            if (effect != null) {
                _effect = effect;
                context.EffectsToTrigger.Add(this);
            }
        }
    }

    [ProvideSourceInfo]
    [Serializable]
    public class ImmediateEffect : IImmediateEffect
    {
        [SerializeReference, ShowSerializeReference]
        AoeShapes.IAoeShape _areaOfEffect;

        public void ImmediateAction(EffectContext context, IEffect effect) {
            if (effect != null) {
                // direction towards target
                var directionVector = (context.EffectLocation.position - context.Caster.position).normalized;
                directionVector.y = 0;
                var direction = Quaternion.LookRotation(directionVector, Vector3.up);

                EffectInput data = new(context, Enumerable.Empty<Transform>(), StatModifierAccessor.Permanent);
                var areaOfEffect = _areaOfEffect.AreaOfEffect(data);
                // Copy the location of the effect and merge with the direction caster -> target
                GameObject tmpGo = new();
                tmpGo.transform.position = context.EffectLocation.position;
                tmpGo.transform.rotation = direction;

                data.Affected = areaOfEffect.GetAffectedUnits(tmpGo.transform);
                GameObject.Destroy(tmpGo);

                effect.Apply(data);

                var appliedResource = context.GetAllResourcesInRegister<Added>();

                context.EffectSystem.ApplyTagsEffect(appliedResource, context.EffectLocation, direction, areaOfEffect);
            }
        }
    }

    [ProvideSourceInfo]
    [Serializable]
    public class ImmediateWeave : IImmediateEffect
    {
        public void ImmediateAction(EffectContext context, IEffect effect) {
            EffectInput data = new(context, context.GetAffectedUnits(), StatModifierAccessor.Permanent);
            if (effect != null) {
                effect.Apply(data);
                data.Affected = context.GetAffectedUnits();
            }
            Materialize(data);
        }

        void Materialize(EffectInput data) {
            foreach (var effect in data.Context.EffectsToTrigger) {
                effect.DelayedAction(data);
            }
            data.Context.EffectsToTrigger.Clear();
        }
    }

    [ProvideSourceInfo]
    [Serializable]
    public class PeriodicWeave : IImmediateEffect
    {
        [SerializeReference, ShowSerializeReference]
        public AoeShapes.IAoeShape AreaOfEffect;

        [SerializeReference, ShowSerializeReference]
        public Triggers.ITrigger ApplyTrigger;

        [SerializeReference, ShowSerializeReference]
        public Triggers.ITrigger EndTrigger;

        public bool IsPermanent;

        public void ImmediateAction(EffectContext context, IEffect effect) {
            EffectInput data = new(context, context.GetAffectedUnits(), StatModifierAccessor.Permanent);
            if (effect != null) {
                effect.Apply(data);
                data.Affected = context.GetAffectedUnits();
            }
            Materialize(data);
        }

        void Materialize(EffectInput data) {
            var areaOfEffect = AreaOfEffect.AreaOfEffect(data);

            /// Move all effects in context to condition context.
            List<IDelayedEffect> effectsToTrigger = new(data.Context.EffectsToTrigger);
            data.Context.EffectsToTrigger.Clear();

            var origin = data.Context.Caster;

            foreach (var affectedPawn in data.Affected) {
                var condition = IsPermanent ? affectedPawn.gameObject.AddComponent<PawnCondition>() : affectedPawn.gameObject.AddComponent<TransientPawnCondition>();

                condition.EndTrigger = EndTrigger?.ShallowCopy();
                condition.ApplyTrigger = ApplyTrigger?.ShallowCopy();
                condition.Origin = origin;

                condition.Context = new EffectContext() {
                    AreaOfEffect = areaOfEffect,
                    Caster = affectedPawn,
                    EffectLocation = affectedPawn,
                    EffectsToTrigger = effectsToTrigger
                };

                condition.Apply();
            }
        }
    }

}
