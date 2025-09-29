using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (_mainCameraTransform == null) return;
        transform.LookAt(transform.position + _mainCameraTransform.forward, Vector3.up);
    }
}
