using System;
using System.Collections;
using System.Collections.Generic;
using TagFighter.Events;
using TagFighter.UnitControl;
using UnityEngine;


namespace TagFighter.Events
{
    public class UnitControllerTargetStartedEventArgs : EventArgs
    {
        public Transform UnitTransform { get; }
        public UnitControllerType ControlledBy { get; }

        public UnitControllerTargetStartedEventArgs(Transform unitTransform, UnitControllerType controlledBy) {
            UnitTransform = unitTransform;
            ControlledBy = controlledBy;
        }
    }
}
namespace TagFighter.UnitControl
{
    public class UnitControllerTarget : MonoBehaviour
    {
        public UnitControllerType ControlledBy;
        [SerializeField] EventAggregator _eventAggregator;

        protected void Start() {
            _eventAggregator.OnUnitControllerStarted(this, new(transform, ControlledBy));
        }

    }

}
