using System;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects.ResourceLocationAccessors.ContextRegisters;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects
{
    public class ReleaseMultiplier
    {
        public Type MatchingAoe { get; set; }
        public float MatchingAoeReleaseMultiplier { get; set; } = 1f;
        public float NonMatchingAoeReleaseMultiplier { get; set; } = 1f;
    }

    public record RegistryKey
    {
        public Type ContextRegisterType;
        public Type ResourceType;
        public RegistryKey(Type contextRegisterType, Type resourceType) {
            ContextRegisterType = contextRegisterType;
            ResourceType = resourceType;
        }
    }

    public class EffectContext
    {
        Dictionary<RegistryKey, IUnit> _resourceRegistry = new();

        public Transform Caster { get; set; }
        public Transform EffectLocation { get; set; }

        public IAreaOfEffect AreaOfEffect;

        public List<IDelayedEffect> EffectsToTrigger { get; set; } = new();

        public IEffectSystem EffectSystem { get; private set; }

        public ReleaseMultiplier ReleaseMultiplier { get; set; }

        public EffectContext() {
            EffectSystem = SystemsHandler.EffectSystem;
            ReleaseMultiplier = new();
        }

        public Unit<TUnit> GetResource<TResource, TUnit, TContextRegister>()
            where TResource : Resource<TUnit>
            where TUnit : IUnitType
            where TContextRegister : IContextRegister {
            var key = new RegistryKey(typeof(TContextRegister), typeof(TResource));
            var value = (Unit<TUnit>)_resourceRegistry.GetValueOrDefault(key, (Unit<TUnit>)0);
            Debug.Log($"{nameof(GetResource)} ({typeof(TResource).Name},{typeof(TContextRegister).Name}) = {value}");
            return value;
        }

        public Unit<TUnit> SetResource<TResource, TUnit, TContextRegister>(Unit<TUnit> value)
            where TResource : Resource<TUnit>
            where TUnit : IUnitType
            where TContextRegister : IContextRegister {
            var key = new RegistryKey(typeof(TContextRegister), typeof(TResource));
            var newValue = (Unit<TUnit>)(_resourceRegistry[key] = value);
            Debug.Log($"{nameof(SetResource)} ({typeof(TResource).Name},{typeof(TContextRegister).Name})  = {newValue}");
            return newValue;
        }

        public IEnumerable<(Type, IUnit)> GetAllResourcesInRegister<TContextRegister>() where TContextRegister : IContextRegister {
            return _resourceRegistry.Where(x => x.Key.ContextRegisterType == typeof(TContextRegister)).Select(x => (x.Key.ResourceType, x.Value));
        }

        public IEnumerable<Transform> GetAffectedUnits() {
            if (AreaOfEffect == null) {
                return Enumerable.Empty<Transform>();
            }

            var directionVector = (EffectLocation.position - Caster.position).normalized;
            directionVector.y = 0;
            var direction = Quaternion.LookRotation(directionVector, Vector3.up);

            GameObject tmpGo = new();
            tmpGo.transform.position = EffectLocation.position;
            tmpGo.transform.rotation = direction;

            var affectedUnits = AreaOfEffect.GetAffectedUnits(EffectLocation);

            GameObject.Destroy(tmpGo);

            return affectedUnits;
        }

        void ResetEffects() {
            EffectsToTrigger.Clear();
        }

        public void Reset() {
            ResetEffects();
        }
    }
}
