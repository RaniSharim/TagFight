using System;
using TagFighter.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TagFighter
{
    public class UnitActionEventArgs : EventArgs
    {
        public Transform UnitTransform { get; }
        public UnitActionEventArgs(Transform unitTransform) {
            UnitTransform = unitTransform;
        }
    }

    public class UnitMoveEventArgs : EventArgs
    {
        public Vector3 Point { get; }
        public UnitMoveEventArgs(Vector3 point) {
            Point = point;
        }
    }

    public class PlayerKeyActions : MonoBehaviour
    {
        [SerializeField] CameraController _cameraController;
        [SerializeField] LayerMask _unitSelectionLayerMask;
        [SerializeField] LayerMask _unitMovementLayerMask;
        [SerializeField] EventAggregator _eventAggregator;

        bool _anyGameWorldHidden = false;

        protected void Update() {
            if (_anyGameWorldHidden == false) {
                HandleTimeDilation();
                HandleCameraMovement();
                HandleCameraRotation();
                HandleUnitActions();
            }
            HandleAdditionalKeyboadClicks();
            HandleMouseClick();
        }

        void HandleAdditionalKeyboadClicks() {
            if (Input.GetKeyDown(KeyCode.R)) {
                if (Input.GetKey(KeyCode.LeftControl)) {
                    _eventAggregator.OnWeaveBuilderScreenToggleSelected(this, EventArgs.Empty);
                    _anyGameWorldHidden = !_anyGameWorldHidden;
                }
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                _eventAggregator.OnPawnSheetToggleSelected(this, new());
            }
        }
        void HandleUnitActions() {
        }

        void HandleMouseClick() {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out var hit)) {
                        var objectHit = hit.transform;
                        if (_unitSelectionLayerMask.IsLayerInMask(objectHit.gameObject.layer)) {
                            _eventAggregator.OnUnitSelected(this, new(objectHit));
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out var hit)) {
                        var objectHit = hit.transform;
                        if (_unitSelectionLayerMask.IsLayerInMask(objectHit.gameObject.layer)) {
                            _eventAggregator.OnTargetSelected(this, new(objectHit));
                        }
                        else if (_unitMovementLayerMask.IsLayerInMask(objectHit.gameObject.layer)) {
                            if (Input.GetKey(KeyCode.LeftControl)) {
                                _eventAggregator.OnUnitMove(this, new(hit.point));
                            }
                            else {
                                _eventAggregator.OnPlannedActionsClearSelected(this, EventArgs.Empty);
                                _eventAggregator.OnUnitMove(this, new(hit.point));
                            }
                        }
                    }


                }
            }
        }

        void HandleTimeDilation() {
            if (Input.GetKeyDown(KeyCode.Period)) {
                _eventAggregator.OnTimeDilationSpeedUp(this, EventArgs.Empty);
            }
            if (Input.GetKeyDown(KeyCode.Comma)) {
                _eventAggregator.OnTimeDilationSpeedDown(this, EventArgs.Empty);
            }
            if (Input.GetKeyDown(KeyCode.Slash)) {
                _eventAggregator.OnTimeDilationSpeedReset(this, EventArgs.Empty);
            }

        }

        void HandleCameraMovement() {
            if (Input.GetKey(KeyCode.W)) {
                _cameraController.MoveForward();
            }
            if (Input.GetKey(KeyCode.A)) {
                _cameraController.MoveLeft();
            }
            if (Input.GetKey(KeyCode.S)) {
                _cameraController.MoveBack();
            }
            if (Input.GetKey(KeyCode.D)) {
                _cameraController.MoveRight();
            }

        }
        void HandleCameraRotation() {
            if (Input.GetKey(KeyCode.Q)) {
                _cameraController.RotateLeft();
            }
            if (Input.GetKey(KeyCode.E)) {
                _cameraController.RotateRight();
            }
        }
    }

    public static class LayerMaskExtensions
    {
        public static bool IsLayerInMask(this LayerMask layerMask, int layer) {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}
