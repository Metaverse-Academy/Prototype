using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerNoise : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float walkNoise = 2f;
    [SerializeField] private float sprintNoise = 5f;
    [SerializeField] private float crouchNoise = 0.5f;
    [SerializeField] private float noiseInterval = 0.4f;

    private PlayerMovement movement;
    private Rigidbody rb;
    private float noiseTimer;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Only make noise if the player is moving
        Vector3 horizontalVel = rb.linearVelocity; horizontalVel.y = 0f;
        if (horizontalVel.magnitude > 0.1f && movementIsGrounded())
        {
            noiseTimer -= Time.deltaTime;
            if (noiseTimer <= 0f)
            {
                float loudness = GetNoiseLevel();
                MakeNoise(loudness);
                noiseTimer = noiseInterval;
            }
        }
    }

    private bool movementIsGrounded()
    {
        // Access the grounded check from PlayerMovement if you expose it publicly later.
        // For now, assume always grounded if on flat ground.
        return true;
    }

    private float GetNoiseLevel()
    {
        // Approximate the movement type
        if (IsSprinting()) return sprintNoise;
        if (IsCrouching()) return crouchNoise;
        return walkNoise;
    }

    private bool IsSprinting() => GetPrivateBool("isSprinting");
    private bool IsCrouching() => GetPrivateBool("isCrouching");

    private bool GetPrivateBool(string field)
    {
        var f = typeof(PlayerMovement).GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return f != null && (bool)f.GetValue(movement);
    }

    private void MakeNoise(float loudness)
    {
        Collider[] listeners = Physics.OverlapSphere(transform.position, loudness);
        foreach (Collider listener in listeners)
        {
            if (listener.CompareTag("Mother"))
            {
                listener.GetComponent<MotherAI>()?.OnHeardNoise(transform.position, loudness);
            }
        }

        // Debug visualization
        Debug.DrawRay(transform.position, Vector3.up * 2, Color.yellow, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, walkNoise);
    }
}
