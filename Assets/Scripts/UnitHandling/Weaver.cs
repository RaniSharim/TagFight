using System.Linq;
using TagFighter.Martial;
using UnityEngine;

namespace TagFighter
{
    public class Weaver : MonoBehaviour
    {
        [SerializeField] MoveRequirementRuleset _moveSetRules;
        public bool CanUseWeave(RuneWeaving weave) {
            if (weave == null) {
                return false;
            }

            var canUseWeave = true;
            var forbiddenMove = weave.MartialSequence.GetSequence().FirstOrDefault(move => !_moveSetRules.CanUseCombatMove(move, transform));
            if (forbiddenMove != null) {
                // Debug.Log($"<color=red>{transform} cannot use {forbiddenMove}</color> on weave {weave}");
                canUseWeave = false;
            }

            return canUseWeave;
        }

    }
}