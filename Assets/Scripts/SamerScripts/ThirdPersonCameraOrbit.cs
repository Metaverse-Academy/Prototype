using UnityEngine;

public class ThirdPersonCameraOrbit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 4f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float rotationSpeed = 200f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -35f, 60f);

        // rotate camera around player
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rot * new Vector3(0f, height, -distance);
        transform.position = player.position + offset;
        transform.LookAt(player.position + Vector3.up * height * 0.5f);
    }
}
