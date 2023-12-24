using UnityEngine;

namespace TagFighter.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        Transform _camera;

        protected void Awake() {
            _camera = Camera.main.transform;
        }

        protected void LateUpdate() {
            var dirToCamera = (_camera.position - transform.position).normalized;
            transform.LookAt(transform.position + dirToCamera * -1);
        }
    }
}