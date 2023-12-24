using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using UnityEngine;

/// <summary>
/// 
/// When Getting or Setting a resource, three attributes must by chosen
/// 1. Which resource should be manipulated
/// 2. What is the location of the resource (Pawn / Context) to be manipulated
/// 3. Which property of the location should be manipulated
///
/// <example>
/// 1. Get (1)Pain resource from (2)Context (3)Current Register
/// 2. Get (1)RedTag (3)Capacity from all (2)Pawns
/// </example>
/// 
/// In Order to allow Serialization in the editor, all three attributes have a corresponding accessor classes.
/// These are all consolidated and expected under
/// <see cref="TagFighter.Effects.ResourceInfoGet{TTypeAccessor, TLocationAccessor}"/>
/// <see cref="TagFighter.Effects.ResourceInfoSet{TTypeAccessor, TLocationAccessor}"/>
/// 
/// Resources are accessed through <see cref="TagFighter.Effects.ResourceTypeAccessor{TResource, TUnit}"/> with specific types encoded per resource.
/// For example: <see cref="TagFighter.Resources.Pain"/> is accessed through <see cref="TagFighter.Effects.ResourceTypeAccessors.Pain"/>
/// 
/// Locations are accessed through <see cref=TagFighter.Effects.IResourceLocationGet"/> and <see cref="TagFighter.Effects.IResourceLocationSet"/>
/// <example>
/// <see cref="TagFighter.Effects.EffectContext"/> is accessed through <see cref="TagFighter.Effects.ResourceLocationAccessors.Get.Context"/> and <see cref="TagFighter.Effects.ResourceLocationAccessors.Set.Context"/>
/// Pawn (Any Gameobject) is accessed through <see cref="TagFighter.Effects.ResourceLocationAccessors.Get.Pawn"/> and <see cref="TagFighter.Effects.ResourceLocationAccessors.Set.Pawn"/>
/// </example>
/// 
/// As each Location has different properties - Location properties have specialized accessors:
/// 1. Context - <see cref="TagFighter.Effects.ResourceLocationAccessors.ContextRegisters.IContextRegister"/>
/// 2. Pawn - <see cref="TagFighter.Effects.ResourceLocationAccessors.PawnProperties.IPawnResourceProperty"/>
/// <example>
/// Context <see cref="TagFighter.Effects.EffectContext.GetResource{TResource, TUnit, TContextRegister}"/> is accessed through <see cref="TagFighter.Effects.ResourceLocationAccessors.ContextRegisters.ContextRegister{TRegisterType}"/>
/// with specialized registers such as <see cref="TagFighter.Effects.ResourceLocationAccessors.ContextRegisters.Current"/>, <see cref="TagFighter.Effects.ResourceLocationAccessors.ContextRegisters.Removed"/>
/// or <see cref="TagFighter.Effects.ResourceLocationAccessors.ContextRegisters.Added"/>
/// Pawn <see cref="TagFighter.Resources.Resource{TUnitType}.GetCapacity"/> is accessed through <see cref="TagFighter.Effects.ResourceLocationAccessors.PawnProperties.Capacity"/>
/// and Pawn <see cref="TagFighter.Resources.Resource{TUnitType}.GetCurrent"/> is accessed through <see cref="TagFighter.Effects.ResourceLocationAccessors.PawnProperties.Current"/>
/// </example>
/// 
/// In addition, Context and Pawn are different in regards to amount of instances. There is always a single weave Context while there can be 0 or more pawns affected.
/// As a result, when Setting to Context, an aggregation operation method must be chosen from <see cref="TagFighter.Effects.IResourceOperator"/>
/// <example>
/// <see cref="TagFighter.Effects.Operators.Sum"/>,  <see cref="TagFighter.Effects.Operators.Multiply"/>
/// </example>
/// 
/// Serializing Pawn Resource Accessor in a MonoBehavior without a Context:
/// [SerializeReference, ShowSerializeReference]
/// public <see cref="TagFighter.Effects.IResourceGetter"/>  Resource;
/// [SerializeReference, ShowSerializeReference]
/// public <see cref="TagFighter.Effects.ResourceLocationAccessors.PawnProperties.IPawnResourceProperty"/> Stat;
/// Resource.GetStat(transform, Stat);
///
/// </summary>

namespace TagFighter.Effects
{
    [Serializable]
    public class ResourceInfoGet<TTypeAccessor, TLocationAccessor> where TTypeAccessor : IResourceTypeAccessor where TLocationAccessor : IResourceLocationGet
    {
        [SerializeReference, ShowSerializeReference]
        public TTypeAccessor Resource;

        [SerializeReference, ShowSerializeReference]
        public TLocationAccessor Location;

        [TypeFilter(derivedFrom: typeof(Resources.Resource<>))]
        [SerializeField]
        public SerializableType ResourceType;

        public double Multiplier = 1;

        public double Addend = 0;
        public bool IsInit {
            get {
                return (Resource != null) && (Location != null);
            }
        }

        public IEnumerable<double> Get(EffectInput data) {
            return IsInit ? Resource.Get(data, Location).Select(resource => (Multiplier * resource) + Addend) :
                            Enumerable.Repeat(Addend, 1);
        }

        public override string ToString() => $"{Resource}.{Location}";
    }

    [Serializable]
    public class ResourceInfoSet<TTypeAccessor, TLocationAccessor> where TTypeAccessor : IResourceTypeAccessor where TLocationAccessor : IResourceLocationSet
    {
        [SerializeReference, ShowSerializeReference]
        public TTypeAccessor Resource;

        [SerializeReference, ShowSerializeReference]
        public TLocationAccessor Location;

        public double Multiplier = 1;

        public double Addend = 0;
        public bool IsInit {
            get {
                return (Resource != null) && (Location != null);
            }
        }
        public void Set(EffectInput data, IEnumerable<double> value) {
            if (!IsInit) {
                Debug.Log("Set Resource missing Type or Location, skipping");
                return;
            }

            var manipulatedValue = value.Select(resource => (int)((Multiplier * resource) + Addend));
            Resource.Set(data, Location, manipulatedValue);
        }

        public override string ToString() => $"{Resource}.{Location}";
    }
}
