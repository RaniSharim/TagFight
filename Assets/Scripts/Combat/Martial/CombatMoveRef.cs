using UnityEngine;

namespace TagFighter.Martial
{
    [CreateAssetMenu(fileName = "CombatMove", menuName = "Game/Combat/CombatMove")]
    public class CombatMoveRef : ScriptableObject
    {
        [UnityEngine.Serialization.FormerlySerializedAs("combatMove")]
        public CombatMove CombatMove;
        public static implicit operator CombatMove(CombatMoveRef combatMoveRef) => combatMoveRef.CombatMove;

        public CombatMove Value => CombatMove;
    }
}