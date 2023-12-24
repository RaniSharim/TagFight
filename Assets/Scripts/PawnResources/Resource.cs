using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TagFighter.Resources
{
    public interface IAccessibleResource<TUnitType> where TUnitType : IUnitType
    {
        public Unit<TUnitType> GetCapacity();
        public Unit<TUnitType> GetCurrent();
    }


    [Serializable]
    public class Resource<TUnitType> : MonoBehaviour, IWatchableResource, IAccessibleResource<TUnitType> where TUnitType : IUnitType
    {
        [SerializeField] Stat<Current> _current;
        [SerializeField] Stat<Capacity> _capacity;

        readonly Unit<TUnitType> _minimumCapacity = (Unit<TUnitType>)0;

        public ResourceChangeArgs Status => (ResourceChangeArgs)this;

        public Unit<TUnitType> GetCapacity() {
            var value = _capacity.Total;
            if (value < _minimumCapacity) {
                value = _minimumCapacity;
            }
            return value;
        }

        public void AddCapacityModifier(StatModifier<TUnitType> modifier, CancellationToken cancelToken) {
            AddModifier(modifier, _capacity, cancelToken);
        }

        public Unit<TUnitType> GetCurrent() {
            var value = _current.Total;
            value = Unit<TUnitType>.Clamp(value, _minimumCapacity, GetCapacity());
            return value;
        }

        public void AddCurrentModifier(StatModifier<TUnitType> modifier, CancellationToken cancelToken) {
            AddModifier(modifier, _current, cancelToken);
        }

        void AddModifier<TStatType>(StatModifier<TUnitType> modifier, Stat<TStatType> stat, CancellationToken cancelToken)
            where TStatType : IStatType {
            cancelToken.Register(() => {
                OnResourceChanged((ResourceChangeArgs)this);
            });

            stat.AddModifier(modifier, cancelToken);

            Debug.Log($"AddModifier {transform.name}.{GetType().Name}.{typeof(TStatType).Name}");
            OnResourceChanged((ResourceChangeArgs)this);
        }

        public event EventHandler<ResourceChangeArgs> ResourceChanged;
        public static explicit operator ResourceChangeArgs(Resource<TUnitType> resource) {
            return new((int)resource.GetCurrent(), (int)resource._current.Base, (int)resource._current.Modified,
                       (int)resource.GetCapacity(), (int)resource._capacity.Base, (int)resource._capacity.Modified,
                       resource.transform);
        }

        void OnResourceChanged(ResourceChangeArgs e) {
            ResourceChanged?.Invoke(this, e);
        }

        public interface IStatType { }
        class Current : IStatType { }
        class Capacity : IStatType { }

        [Serializable]
        public class Stat<TStatType> where TStatType : IStatType
        {
            [SerializeField] Unit<TUnitType> _base;
            LinkedList<StatModifier<TUnitType>> _modifiers = new();

            public Unit<TUnitType> Base {
                get {
                    return _base;
                }
                private set {
                    _base = value;
                }
            }

            public Unit<TUnitType> Modified {
                get;
                private set;
            }

            public Unit<TUnitType> Total => Base + Modified;

            public void AddModifier(StatModifier<TUnitType> modifier, CancellationToken cancelToken) {
                if (cancelToken == CancellationToken.None) {
                    Base += modifier.Amount;
                }
                else {
                    AddTransientModifier(modifier, cancelToken);
                }
            }

            void AddTransientModifier(StatModifier<TUnitType> modifier, CancellationToken cancelToken) {
                LinkedListNode<StatModifier<TUnitType>> node = new(modifier);

                _modifiers.AddFirst(node);
                Modified += modifier.Amount;

                cancelToken.Register(() => {
                    _modifiers.Remove(node);
                    Modified -= modifier.Amount;
                });
            }
        }
    }
}
