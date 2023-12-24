using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace TagFighter.Martial
{
    [Serializable]
    public class CombatForm
    {
        [FormerlySerializedAs("_name")]
        public string Name;
        [SerializeField] CombatMoveSetRef _combatMoveSetRef;
        CombatMoveSet _combatMoveSet;
        public CombatForm() {
            Populate();
        }

        public void Populate() {
            if (_combatMoveSetRef != null) {
                _combatMoveSet = _combatMoveSetRef;
            }
        }
        public void Clear() {
            _combatMoveSet = null;
        }
        public CombatMoveSet FormedMoveSet(CombatMoveSet moveSet) {
            CombatMoveSet formedMoveSet = new();
            return formedMoveSet.AddMoves(moveSet.CombatMoves().Select(move => FormMove(move))).AddMoves(_combatMoveSet);
        }

        CombatMoveRef FormMove(CombatMoveRef basicMove) {
            return basicMove;
        }
    }
}