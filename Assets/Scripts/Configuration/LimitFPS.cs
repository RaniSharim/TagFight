using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    [ContextMenuItem("Apply Target Framerate", "ApplyTargetFrameRate")]
    [SerializeField] int _targetFrameRate;

    protected void Start() {
        ApplyTargetFrameRate();
    }

    void ApplyTargetFrameRate() {
        Application.targetFrameRate = _targetFrameRate;
    }
}
