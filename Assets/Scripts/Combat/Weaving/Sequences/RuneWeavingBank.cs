using System;
using System.Collections.Generic;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter
{

    [CreateAssetMenu(fileName = "RuneWeavingBank", menuName = "Game/Combat/RuneWeavingBank")]
    public class RuneWeavingBank : ScriptableObject
    {
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] List<RuneWeavingContainer> _runeWeavings = new();

        public void AddWeave(RuneWeaving runeWeaving, ValidWeaveName weaveName) {
            _runeWeavings.Add(new RuneWeavingContainer(weaveName, runeWeaving));
            _eventAggregator.OnRuneWeavingBankChanged(this, new EventAggregator.RuneWeavingBankChangedEventArgs(this));
        }

        public bool TryDeleteWeaveByName(ValidWeaveName weaveName) {
            var found = false;
            var weaveIndex = _runeWeavings.FindIndex(weave => weave.WeaveName == weaveName);
            found = weaveIndex != -1;
            if (found == true) {
                _runeWeavings.RemoveAt(weaveIndex);
                _eventAggregator.OnRuneWeavingBankChanged(this, new(this));
            }
            return found;
        }

        public bool TryGetWeaveByName(ValidWeaveName weaveName, out RuneWeavingContainer runeWeavingContainer) {
            var found = false;
            var weaveIndex = _runeWeavings.FindIndex(weave => weave.WeaveName == weaveName);
            found = weaveIndex != -1;
            if (found == true) {
                runeWeavingContainer = _runeWeavings[weaveIndex];
            }
            else {
                runeWeavingContainer = null;
            }
            return found;
        }

        public IEnumerable<RuneWeavingContainer> GetWeaves() => _runeWeavings;
    }

    [Serializable]
    public class RuneWeavingContainer
    {
        public RuneWeaving RuneWeaving;
        public ValidWeaveName WeaveName;
        public RuneWeavingContainer(ValidWeaveName weaveName, RuneWeaving runeWeaving) {
            WeaveName = weaveName;
            RuneWeaving = runeWeaving;
        }
        public RuneWeavingContainer(RuneWeavingContainer toCopy) {
            RuneWeaving = new RuneWeaving(toCopy.RuneWeaving.MartialSequence.GetSequence(), toCopy.RuneWeaving.RuneBindingSequence.GetSequence());
            WeaveName = toCopy.WeaveName;
        }
    }

    [Serializable]
    public record ValidWeaveName
    {
        [SerializeField] string _weaveName;
        ValidWeaveName(string weaveName) {
            _weaveName = weaveName;
        }
        public bool TrySetName(string weaveName) {
            var validName = IsWeaveNameValid(weaveName);
            if (validName) {
                _weaveName = weaveName;
            }
            return validName;
        }

        public override string ToString() {
            return _weaveName;
        }

        public static bool TryParse(string weaveName, out ValidWeaveName validWeaveName) {
            var validName = IsWeaveNameValid(weaveName);
            if (validName) {
                validWeaveName = new ValidWeaveName(weaveName);
            }
            else {
                validWeaveName = null;
            }

            return validName;
        }
        public static bool IsWeaveNameValid(string weaveName) {
            return weaveName.Length > 0;
        }

        public static implicit operator string(ValidWeaveName validWeaveName) => validWeaveName._weaveName;

    }
}
