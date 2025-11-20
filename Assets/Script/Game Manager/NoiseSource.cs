using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NoiseSource : MonoBehaviour
{
    [SerializeField] private float impactThreshold = 1.5f;
    [SerializeField] private float loudnessMultiplier = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude;
        if (impact >= impactThreshold)
        {
            float loudness = impact * loudnessMultiplier;

            Collider[] listeners = Physics.OverlapSphere(transform.position, loudness);
            foreach (Collider listener in listeners)
            {
                if (listener.CompareTag("Mother"))
                {
                    listener.GetComponent<MotherAI>()?.OnHeardNoise(transform.position, loudness);
                }
            }

            Debug.DrawRay(transform.position, Vector3.up * 2, Color.red, 1f);
            
        }
    }
}
