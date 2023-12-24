using UnityEngine;

namespace TagFighter.Martial
{
    [CreateAssetMenu(fileName = "CombatMoveSet", menuName = "Game/Combat/CombatMoveSet")]
    public class CombatMoveSetRef : ScriptableObject
    {
        [UnityEngine.Serialization.FormerlySerializedAs("combatMoveSet")]
        public CombatMoveSet CombatMoveSet;
        public static implicit operator CombatMoveSet(CombatMoveSetRef combatMoveSetRef) => combatMoveSetRef.CombatMoveSet;

    }
}