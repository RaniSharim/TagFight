using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TagFighter.Events;
using TagFighter.Actions;

namespace TagFighter.UnitControl
{
    public class TestAIController : MonoBehaviour
    {

        [SerializeField] UnitControllerType _controllingUnitsOfType;
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] float _decisionFrequency = 0.2f;

        List<Transform> _alertedPawns = new();
        List<Transform> _allControlledPawns = new();

        LayerMask _unitsLayerMask;

        protected void Awake() {
            _eventAggregator.UnitControllerStarted += OnUnitControllerStarted;
            _unitsLayerMask = LayerMask.NameToLayer("units");

            if (_controllingUnitsOfType == null) {
                Debug.LogWarning($"{transform.name} missing controllingUnitsOfType. Did you forget to set?");
            }

            if (_eventAggregator == null) {
                Debug.LogWarning($"{transform.name} missing eventAggregator. Did you forget to set?");
            }
        }
        protected void Start() {
            StartCoroutine(DecideActionForAllAlertedUnits());
        }
        protected void OnDestroy() {
            _eventAggregator.UnitControllerStarted -= OnUnitControllerStarted;
            StopCoroutine(DecideActionForAllAlertedUnits());
        }

        void OnUnitControllerStarted(object sender, UnitControllerTargetStartedEventArgs e) {
            if (_controllingUnitsOfType != e.ControlledBy) {
                return;
            }
            _alertedPawns.Add(e.UnitTransform);
            _allControlledPawns.Add(e.UnitTransform);
            Debug.Log($"<color=green>{e.UnitTransform.name} : Added</color>");
        }

        IEnumerator DecideActionForAllAlertedUnits() {
            // Waiting for a couple of frames before starting so all Start() events are called
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //var a = ProduceMoveLocations().GetEnumerator();
            while (true) {
                foreach (var unit in _alertedPawns) {
                    if (unit.TryGetComponent(out ActionPlan actionPlan)) {
                        var currentAction = actionPlan.FirstOrDefault();
                        if (currentAction == default) {
                            //a.MoveNext();
                            //actionPlan.TryAddActionToPlan(new MoveToAction(unit.GetComponent<Weaver>(), a.Current
                            var sightedEnemy = GetEnemiesInRange(unit).FirstOrDefault();
                            //Debug.Log($"Sighted: {sightedEnemy}");
                            if (sightedEnemy != default) {
                                actionPlan.TryAddActionToPlan(new FollowAction(unit.GetComponent<Weaver>(), sightedEnemy, 1f));
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(_decisionFrequency);
                //Debug.Log("DecideActionForAllAlertedUnits");
            }
        }
        IEnumerable<Vector3> ProduceMoveLocations() {
            while (true) {
                yield return new(55, 0, 60);
                yield return new(55, 0, 65);
                yield return new(50, 0, 65);
                yield return new(50, 0, 60);
            }
        }

        IEnumerable<Transform> GetEnemiesInRange(Transform actingUnit) {
            var hits = Physics.SphereCastAll(actingUnit.position, 5f, actingUnit.forward, 0, _unitsLayerMask);

            return hits.Select(hit => hit.transform).
                    Where(unit => unit.TryGetComponent(out UnitControllerTarget unitControllerTarget) && unitControllerTarget.ControlledBy != _controllingUnitsOfType);
        }
        //protected void OnDrawGizmos() {
        //    LayerMask _unitsLayerMask = LayerMask.NameToLayer("units");
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawWireSphere(transform.position, 5f);
        //    var hits = Physics.SphereCastAll(transform.position, 5f, transform.forward, 0, _unitsLayerMask);
        //    foreach (var hit in hits.Where(hit => hit.transform != transform)) {
        //        Gizmos.DrawLine(transform.position, hit.transform.position);
        //    }
        //}
    }
}
