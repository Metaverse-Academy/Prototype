using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 3.5f;

    [Header("Detection Settings")]
    public float viewRadius = 10f;      // مدى الرؤية
    [Range(0, 360)]
    public float viewAngle = 90f;       // زاوية مجال الرؤية (مخروط الرؤية)
    public Transform player;            // اسحب هنا كائن اللاعب
    public LayerMask obstacleMask;      // طبقة العوائق (مثل الجدران)

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;       // سرعة العدو عند المطاردة
    public bool isPlayerSpotted = false;

    private int currentPointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!", this);
            this.enabled = false;
            return;
        }
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    void Update()
    {
        // ابحث عن اللاعب باستمرار
        FindPlayer();

        if (isPlayerSpotted)
        {
            // --- وضع المطاردة ---
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position + Vector3.forward * 5f); // طارد اللاعب
        }
        else
        {
            // --- وضع الدورية ---
            agent.speed = patrolSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPoint();
            }
        }
    }

    void FindPlayer()
    {
        // تحقق مما إذا كان اللاعب ضمن مدى الرؤية
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > viewRadius)
        {
            isPlayerSpotted = false;
            return; // اللاعب بعيد جداً
        }

        // تحقق مما إذا كان اللاعب ضمن زاوية الرؤية
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToPlayer) > viewAngle / 2)
        {
            isPlayerSpotted = false;
            return; // اللاعب خارج مخروط الرؤية
        }

        // تحقق من عدم وجود عوائق (جدران) بين العدو واللاعب
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            isPlayerSpotted = false;
            return; // هناك جدار في الطريق
        }

        // إذا نجحت كل الاختبارات، فقد تم رصد اللاعب!
        if (!isPlayerSpotted) // اطبع الرسالة مرة واحدة فقط عند أول رصد
        {
            Debug.Log("PLAYER SPOTTED!");
        }
        isPlayerSpotted = true;
    }

    void GoToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    // دالة لرسم مجال الرؤية في نافذة المشهد (للتصحيح)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        if (isPlayerSpotted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
