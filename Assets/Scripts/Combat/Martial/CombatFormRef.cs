using UnityEngine;

namespace TagFighter.Martial
{
    [CreateAssetMenu(fileName = "CombatForm", menuName = "Game/Combat/CombatForm")]
    public class CombatFormRef : ScriptableObject
    {
        [SerializeField] CombatForm _combatForm;
        public static implicit operator CombatForm(CombatFormRef combatFormRef) => combatFormRef._combatForm;

        protected void OnEnable() {
            _combatForm?.Populate();
        }

        protected void OnDisable() {
            _combatForm?.Clear();
        }

        public CombatForm Value => _combatForm;
    }
}