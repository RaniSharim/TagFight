using System;
using System.Collections.Generic;
using UnityEngine;

namespace TagFighter.Martial
{
    [Serializable]
    public class CombatMoveSet
    {
        [UnityEngine.Serialization.FormerlySerializedAs("combatMoves")]
        [SerializeField] List<CombatMoveRef> _combatMoves = new();

        public IEnumerable<CombatMoveRef> CombatMoves() {
            return _combatMoves;
        }
        public CombatMoveSet AddMoves(CombatMoveSet additionalMoveSet) {
            return AddMoves(additionalMoveSet.CombatMoves());
        }
        public CombatMoveSet AddMoves(IEnumerable<CombatMoveRef> additionalMoveSet) {
            _combatMoves.AddRange(additionalMoveSet);
            return this;
        }
    }
}