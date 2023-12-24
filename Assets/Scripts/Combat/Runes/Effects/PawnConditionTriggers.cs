using System;
using System.Collections;
using CareBoo.Serially;
using TagFighter.Effects.ResourceLocationAccessors.PawnProperties;
using TagFighter.Equipment;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.Effects.Triggers
{

    public class ConditionTriggerArgs : EventArgs
    {
        public ConditionTriggerArgs() { }
    }

    public interface ITrigger
    {
        public ITrigger ShallowCopy();
        public void Register(PawnCondition condition);
        public void Unregister();
        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;
    }

    [Serializable]
    public abstract class OnResourceChange : ITrigger
    {
        [SerializeReference, ShowSerializeReference]
        public IResourceGetter Resource;

        [SerializeReference, ShowSerializeReference]
        public IPawnResourceProperty Stat;

        public int Threshold;

        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;

        protected PawnCondition Condition;
        IWatchableResource _resource;
        int _beforeValue;

        public void Register(PawnCondition condition) {
            if (Condition != null) {
                Unregister();
            }
            Condition = condition;
            _resource = Resource.GetWatchableResource(condition.transform);

            _beforeValue = Resource.GetStat(condition.transform, Stat);
            _resource.ResourceChanged += OnResourceChanged;
        }

        public ITrigger ShallowCopy() {
            return (ITrigger)MemberwiseClone();
        }

        public void Unregister() {
            _resource.ResourceChanged -= OnResourceChanged;
            _resource = null;
            Condition = null;
            _beforeValue = 0;
        }

        protected void OnResourceChanged(object sender, ResourceChangeArgs e) {
            var current = Resource.GetStat(e.Pawn, Stat);
            if (ShouldTrigger(current, _beforeValue, Threshold)) {
                OnTriggerConditionMet(new());
            }
            _beforeValue = current;
        }

        protected void OnTriggerConditionMet(ConditionTriggerArgs e) {
            TriggerConditionMet?.Invoke(this, e);
        }

        protected abstract bool ShouldTrigger(int current, int before, int threshold);
    }

    [Serializable]
    public class OnResourceIncrease : OnResourceChange
    {
        protected override bool ShouldTrigger(int current, int before, int threshold) {
            return current > before + threshold;
        }
    }

    [Serializable]
    public class OnResourceDecrease : OnResourceChange
    {
        protected override bool ShouldTrigger(int current, int before, int threshold) {
            return current < before - threshold;
        }
    }

    [Serializable]
    public class OnTimePass : ITrigger
    {
        PawnCondition _condition;

        [SerializeField] float _frequency;

        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;

        // public ResourceInfoGet<IResourceTypeAccessor, ResourceLocationAccessors.Get.Context> Frequency;

        public OnTimePass() { }

        public OnTimePass(float frequency) {
            _frequency = frequency;
        }

        public void Register(PawnCondition condition) {
            if (_condition != null) {
                Unregister();
            }
            _condition = condition;
            _condition.StartCoroutine(Tick());
        }

        public ITrigger ShallowCopy() {
            return (ITrigger)MemberwiseClone();
        }

        public void Unregister() {
            _condition.StopCoroutine(Tick());
        }

        protected void OnTriggerConditionMet(ConditionTriggerArgs e) {
            TriggerConditionMet?.Invoke(this, e);
        }

        IEnumerator Tick() {
            while (true) {
                yield return new WaitForSeconds(_frequency);
                OnTriggerConditionMet(new());
                Debug.Log("Tick");
            }
        }
    }

    [Serializable]
    public class OnApply : ITrigger
    {
        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;

        public void Register(PawnCondition condition) {
            OnTriggerConditionMet(new());
        }

        public ITrigger ShallowCopy() {
            return (ITrigger)MemberwiseClone();
        }

        public void Unregister() {
        }
        protected void OnTriggerConditionMet(ConditionTriggerArgs e) {
            TriggerConditionMet?.Invoke(this, e);
        }
    }

    [Serializable]
    public class OnEquip : ITrigger
    {
        public Item WatchedItem;
        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;

        Outfit _watchedOutfit;

        public void Register(PawnCondition condition) {
            if (condition.TryGetComponent(out _watchedOutfit)) {
                if (WatchedItem != null) {
                    _watchedOutfit.OutfitChanging += OnOutfitChanging;
                }
            }
        }

        void OnOutfitChanging(object sender, OutfitChangingEventArgs e) {
            if (ReferenceEquals(WatchedItem, e.ChangingTo)) {
                OnTriggerConditionMet(new());
            }
        }

        public ITrigger ShallowCopy() {
            return (ITrigger)MemberwiseClone();
        }

        public void Unregister() {
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanging -= OnOutfitChanging;
                _watchedOutfit = null;
                WatchedItem = null;
            }
        }
        protected void OnTriggerConditionMet(ConditionTriggerArgs e) {
            TriggerConditionMet?.Invoke(this, e);
        }
    }

    [Serializable]
    public class OnUnequip : ITrigger
    {
        public Item WatchedItem;
        public event EventHandler<ConditionTriggerArgs> TriggerConditionMet;

        Outfit _watchedOutfit;

        public void Register(PawnCondition condition) {

            if (condition.TryGetComponent(out _watchedOutfit)) {
                if (WatchedItem != null) {
                    _watchedOutfit.OutfitChanging += OnOutfitChanging;
                }
            }
        }

        void OnOutfitChanging(object sender, OutfitChangingEventArgs e) {
            if (ReferenceEquals(WatchedItem, e.ChangingFrom)) {
                OnTriggerConditionMet(new());
            }
        }

        public ITrigger ShallowCopy() {
            return (ITrigger)MemberwiseClone();
        }

        public void Unregister() {
            if (_watchedOutfit != null) {
                _watchedOutfit.OutfitChanging -= OnOutfitChanging;
                _watchedOutfit = null;
                WatchedItem = null;
            }
        }
        protected void OnTriggerConditionMet(ConditionTriggerArgs e) {
            TriggerConditionMet?.Invoke(this, e);
        }
    }
}

