using System;
using System.Collections;
using System.Collections.Generic;
using CareBoo.Serially;
using TagFighter.Effects;
using TagFighter.Effects.ResourceLocationAccessors.PawnProperties;
using UnityEngine;

namespace TagFighter.Martial
{
    [CreateAssetMenu(fileName = "NewMoveRequirementRuleset", menuName = "Game/Combat/MoveRequirementRuleset")]
    public class MoveRequirementRuleset : ScriptableObject, IEnumerable<MoveRequirementRule>, ISerializationCallbackReceiver
    {
        [SerializeField] List<MoveRequirementRule> _rules;
        Dictionary<CombatMoveRef, int> _moveToIndex;

        public IEnumerator<MoveRequirementRule> GetEnumerator() {
            return ((IEnumerable<MoveRequirementRule>)_rules).GetEnumerator();
        }

        public void OnAfterDeserialize() {
            _moveToIndex = new();
            for (var ruleIndex = 0; ruleIndex < _rules.Count; ruleIndex++) {
                var currentRule = _rules[ruleIndex];
                if (currentRule != null && currentRule.Move != null) {
                    _moveToIndex.TryAdd(currentRule.Move, ruleIndex);
                }
            }
        }
        public MoveRequirementRule this[int i] => _rules[i];
        public MoveRequirementRule this[CombatMoveRef combatMoveRef] => _rules[_moveToIndex[combatMoveRef]];

        public bool CanUseCombatMove(CombatMoveRef combatMove, Transform transform) {
            var rule = _rules[_moveToIndex[combatMove]];
            var statValue = rule.Resource.GetStat(transform, rule.Stat);

            return statValue >= rule.Threshold;
        }

        public void OnBeforeSerialize() {
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_rules).GetEnumerator();
        }
    }

    [Serializable]
    public class MoveRequirementRule
    {
        public CombatMoveRef Move;

        [SerializeReference, ShowSerializeReference]
        public IResourceGetter Resource;

        [SerializeReference, ShowSerializeReference]
        public IPawnResourceProperty Stat;

        public int Threshold;
    }
}