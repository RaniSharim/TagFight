using System;
using UnityEngine;

namespace TagFighter.Resources
{
    public interface IWatchableResource
    {
        public event EventHandler<ResourceChangeArgs> ResourceChanged;

        public ResourceChangeArgs Status {
            get;
        }
    }

    public class ResourceChangeArgs : EventArgs
    {
        public int Current { get; }
        public int Capacity { get; }
        public int CurrentBase { get; }
        public int CapacityBase { get; }
        public int CurrentModifier { get; }
        public int CapacityModifier { get; }

        public Transform Pawn { get; }

        public ResourceChangeArgs(int current, int currentBase, int currentModifier, int capacity, int capacityBase, int capacityModifier, Transform pawn) {
            Current = current;
            Capacity = capacity;
            CurrentBase = currentBase;
            CapacityBase = capacityBase;
            CurrentModifier = currentModifier;
            CapacityModifier = capacityModifier;
            Pawn = pawn;
        }
    }
}