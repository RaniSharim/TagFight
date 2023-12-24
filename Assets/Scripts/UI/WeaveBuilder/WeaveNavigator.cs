using System.Collections.Generic;
using System.Linq;
using TagFighter.Martial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class WeaveNavigator : MonoBehaviour
    {
        [SerializeField] GameObject _timeCounterNodeTemplate;
        [SerializeField] RectTransform _timeCounterTransform;
        [SerializeField] GameObject _martialSequenceNodeTemplate;
        [SerializeField] RectTransform _martialSequenceTransform;
        [SerializeField] GameObject _runeBindingSequenceNodeTemplate;
        [SerializeField] RectTransform _runeBindingSequenceTransform;

        readonly List<GameObject> _timeSequenceNodes = new();
        readonly List<MartialSequenceClicker> _martialSequence = new();
        float _martialSequenceTime;

        readonly List<RuneBindingSequenceClicker> _runeBindingSequence = new();
        float _runeBindingSequenceTime;
        VerticalLayoutGroup _scrollLayoutGroup;

        protected void Awake() {
            _scrollLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();
        }

        void UpdateScrollLayoutGroup() {
            _scrollLayoutGroup.spacing += 0.01f;
            if (_scrollLayoutGroup.spacing > 0.01f) {
                _scrollLayoutGroup.spacing = 0f;
            }
        }

        void UpdateTimeSequenceNodes() {
            while (_martialSequenceTime + 1 <= _timeSequenceNodes.Count) {
                var timeNodeToRemove = _timeSequenceNodes[^1];
                _timeSequenceNodes.RemoveAt(_timeSequenceNodes.Count - 1);
                Destroy(timeNodeToRemove.gameObject);
            }

            while (_martialSequenceTime > _timeSequenceNodes.Count) {
                var timeNode = Instantiate(_timeCounterNodeTemplate, _timeCounterTransform);
                var secLabel = timeNode.GetComponentInChildren<TextMeshProUGUI>();

                secLabel.text = (_timeSequenceNodes.Count + 1).ToString();
                _timeSequenceNodes.Add(timeNode);
                timeNode.SetActive(true);
            }
        }
        void AddMoveToSequence(CombatMoveRef moveRef) {
            var combatMoveNode = Instantiate(_martialSequenceNodeTemplate, _martialSequenceTransform);
            var nameLabel = combatMoveNode.GetComponentInChildren<TextMeshProUGUI>();
            var martialSequenceClicker = combatMoveNode.GetComponent<MartialSequenceClicker>();
            var combatMoveNodeNodeTransform = combatMoveNode.GetComponent<RectTransform>();

            martialSequenceClicker.CombatMoveRef = moveRef;
            _martialSequence.Add(martialSequenceClicker);
            _martialSequenceTime += moveRef.CombatMove.Speed;

            combatMoveNodeNodeTransform.sizeDelta *= moveRef.CombatMove.Speed;
            nameLabel.text = moveRef.CombatMove.MoveName;
            combatMoveNode.SetActive(true);

            UpdateTimeSequenceNodes();
        }

        void RemoveMoveFromSequence(int positionInSequence) {
            var martialSequenceClicker = _martialSequence[positionInSequence];
            _martialSequenceTime -= martialSequenceClicker.CombatMoveRef.CombatMove.Speed;

            _martialSequence.RemoveAt(positionInSequence);

            martialSequenceClicker.gameObject.SetActive(false);
            Destroy(martialSequenceClicker.gameObject);

            UpdateTimeSequenceNodes();
        }

        void AddRuneToSequence(RuneRef runeRef) {
            var runeBindingNode = Instantiate(_runeBindingSequenceNodeTemplate, _runeBindingSequenceTransform);
            var nameLabel = runeBindingNode.GetComponentInChildren<TextMeshProUGUI>();
            var runeBindingSequenceClicker = runeBindingNode.GetComponent<RuneBindingSequenceClicker>();
            var runeBindingNodeTransform = runeBindingNode.GetComponent<RectTransform>();

            runeBindingSequenceClicker.RuneRef = runeRef;
            _runeBindingSequence.Add(runeBindingSequenceClicker);
            _runeBindingSequenceTime += runeRef.Rune.Speed;

            runeBindingNodeTransform.sizeDelta *= runeRef.Rune.Speed;
            nameLabel.text = runeRef.name;
            runeBindingNode.SetActive(true);
        }

        void RemoveRuneFromSequence(int positionInSequence) {
            var runeBindingSequenceClicker = _runeBindingSequence[positionInSequence];
            _runeBindingSequenceTime -= runeBindingSequenceClicker.RuneRef.Rune.Speed;

            _runeBindingSequence.RemoveAt(positionInSequence);

            Destroy(runeBindingSequenceClicker.gameObject);
        }

        public bool CanAddMove(CombatMove move) {
            return move != null;
        }
        public bool CanRemoveMove(int positionInSequence) {
            return (positionInSequence < _martialSequence.Count) &&
                (_runeBindingSequenceTime <= _martialSequenceTime - _martialSequence[positionInSequence].CombatMoveRef.CombatMove.Speed + 0.01f);

        }

        public bool TryRemoveMove(MartialSequenceClicker clicker) {
            var positionInSequence = _martialSequence.IndexOf(clicker);
            var moveRemovedFromSequence = false;
            if (CanRemoveMove(positionInSequence)) {
                RemoveMoveFromSequence(positionInSequence);
                UpdateScrollLayoutGroup();
                moveRemovedFromSequence = true;
            }

            return moveRemovedFromSequence;
        }

        public bool TryAddMove(CombatMoveRef moveRef) {
            var moveAddedToSequence = false;
            if (CanAddMove(moveRef)) {
                AddMoveToSequence(moveRef);
                UpdateScrollLayoutGroup();
                moveAddedToSequence = true;
            }
            return moveAddedToSequence;
        }

        public bool CanAddRune(Rune rune) {
            return rune != null && _runeBindingSequenceTime + rune.Speed <= _martialSequenceTime + 0.01f;
        }

        public bool CanRemoveRune(int positionInSequence) {
            return positionInSequence < _runeBindingSequence.Count;
        }

        public bool TryRemoveRune(RuneBindingSequenceClicker clicker) {
            var positionInSequence = _runeBindingSequence.IndexOf(clicker);
            var runeRemovedFromSequence = false;
            if (CanRemoveRune(positionInSequence)) {
                RemoveRuneFromSequence(positionInSequence);
                UpdateScrollLayoutGroup();
                runeRemovedFromSequence = true;
            }

            return runeRemovedFromSequence;
        }
        public bool TryAddRune(RuneRef runeRef) {
            var runeAddedToSequence = false;
            if (CanAddRune(runeRef)) {
                AddRuneToSequence(runeRef);
                UpdateScrollLayoutGroup();
                runeAddedToSequence = true;
            }
            return runeAddedToSequence;
        }

        public void Clear() {
            for (var i = _runeBindingSequence.Count - 1; i >= 0; i--) {
                RemoveRuneFromSequence(i);
            }
            for (var i = _martialSequence.Count - 1; i >= 0; i--) {
                RemoveMoveFromSequence(i);
            }
            UpdateScrollLayoutGroup();
        }

        public void Reload(RuneWeavingContainer weaveToLoad) {
            Clear();
            foreach (var combatMove in weaveToLoad.RuneWeaving.MartialSequence.GetSequence()) {
                AddMoveToSequence(combatMove);
            }

            foreach (var rune in weaveToLoad.RuneWeaving.RuneBindingSequence.GetSequence()) {
                AddRuneToSequence(rune);
            }
            UpdateScrollLayoutGroup();
        }

        public RuneWeaving GenerateWeave() {
            return new RuneWeaving(_martialSequence.Select(clicker => clicker.CombatMoveRef),
                                   _runeBindingSequence.Select(clicker => clicker.RuneRef));
        }
    }
}