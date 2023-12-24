using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _rotateSpeed = 100f;


    void MoveTransform(Vector3 moveVector) {
        transform.position += ((transform.forward * moveVector.z) + (transform.right * moveVector.x)) * Time.unscaledDeltaTime * _moveSpeed;
    }
    public void MoveForward() {
        MoveTransform(Vector3.forward);
    }
    public void MoveBack() {
        MoveTransform(Vector3.back);
    }
    public void MoveRight() {
        MoveTransform(Vector3.right);
    }
    public void MoveLeft() {
        MoveTransform(Vector3.left);
    }

    void RotateTransform(Vector3 rotationVector) {
        transform.eulerAngles += rotationVector * Time.deltaTime * _rotateSpeed;
    }
    public void RotateLeft() {
        RotateTransform(Vector3.up);
    }
    public void RotateRight() {
        RotateTransform(Vector3.down);
    }
}
