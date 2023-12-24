using System.Collections.Generic;
using System.Linq;
using TagFighter.Martial;
using TMPro;
using UnityEngine;

namespace TagFighter.UI
{
    public class MovePredicatePopulator : MonoBehaviour
    {
        [SerializeField] MoveRequirementRuleset _moveRequirementRules;
        [SerializeField] MovePicker _movePicker;
        [SerializeField] MoveFilterClicker _moveFilterPrefab;

        protected void Start() {
            PopulatePredicateFilters(_moveRequirementRules);
        }

        void PopulatePredicateFilters(IEnumerable<MoveRequirementRule> moveRules) {
            foreach (var resource in moveRules.Select(rule => rule.Resource).Distinct()) {
                var filterClicker = Instantiate(_moveFilterPrefab, transform);
                var filterLabel = filterClicker.GetComponentInChildren<TextMeshProUGUI>();
                if (filterLabel != null) {
                    filterLabel.text = resource.ToString();
                }
                filterClicker.MoveDisplay = _movePicker;
                filterClicker.Predicate = resource;
            }
        }
    }

}