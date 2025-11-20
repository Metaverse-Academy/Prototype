using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float lookSensitivity = 15f;
    [SerializeField] private float maxLookAngle = 80f;
    private Vector2 lookInput;
    private Vector2 smoothLook;
    private Vector2 currentLook;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleLook();
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void HandleLook()
    {
        Vector2 rawDelta = lookInput * lookSensitivity * Time.deltaTime;
        smoothLook = Vector2.Lerp(smoothLook, rawDelta, 0.7f);
        currentLook += smoothLook;

        currentLook.y = Mathf.Clamp(currentLook.y, -maxLookAngle, maxLookAngle);

        cameraRoot.localRotation = Quaternion.Euler(-currentLook.y, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentLook.x, 0);
    }

}
