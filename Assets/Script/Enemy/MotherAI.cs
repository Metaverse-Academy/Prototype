using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MotherAI : MonoBehaviour
{
    private enum State { Idle, Investigating, Alerted }

    [Header("Behavior Settings")]
    [SerializeField] private float investigateTime = 3f;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask visionMask;

    [Header("Vision Visualization")]
    [SerializeField] private bool drawVisionCone = true;
    [SerializeField] private Color visionColor = new Color(1f, 0f, 0f, 0.25f);

    private NavMeshAgent agent;
    private Vector3 lastHeardPosition;
    private float investigateTimer;
    private State currentState = State.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Investigating:
                Investigate();
                break;
            case State.Alerted:
                ChasePlayer();
                break;
        }

        if (player && CanSeePlayer())
        {
            currentState = State.Alerted;
        }
    }

    public void OnHeardNoise(Vector3 source, float loudness)
    {
        if (currentState == State.Alerted) return;

        lastHeardPosition = source;
        agent.SetDestination(source);
        currentState = State.Investigating;
        investigateTimer = investigateTime;
    }

    private void Investigate()
    {
        investigateTimer -= Time.deltaTime;
        if (investigateTimer <= 0f)
            currentState = State.Idle;
    }

    private void ChasePlayer()
    {
        if (player)
            agent.SetDestination(player.position);
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        float dist = dir.magnitude;
        if (dist > visionRange) return false;

        dir.Normalize();
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > visionAngle * 0.5f) return false;

        // Raycast check
        if (Physics.Raycast(transform.position + Vector3.up * 1.6f, dir, out RaycastHit hit, visionRange, visionMask))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!drawVisionCone) return;

        Gizmos.color = visionColor;
        Vector3 origin = transform.position + Vector3.up * 1.6f;

        // Draw cone edges
        Quaternion leftRayRotation = Quaternion.Euler(0, -visionAngle / 2, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, visionAngle / 2, 0);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(origin, leftRayDirection * visionRange);
        Gizmos.DrawRay(origin, rightRayDirection * visionRange);

        // Draw arc (optional for clarity)
        int segments = 20;
        Vector3 lastPoint = origin + leftRayDirection * visionRange;
        for (int i = 1; i <= segments; i++)
        {
            float step = (-visionAngle / 2) + (visionAngle / segments) * i;
            Vector3 nextDir = Quaternion.Euler(0, step, 0) * transform.forward;
            Vector3 nextPoint = origin + nextDir * visionRange;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
