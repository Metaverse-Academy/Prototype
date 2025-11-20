using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimplePatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("اسحب هنا الكائن الرئيسي الذي يحتوي على نقاط المسار")]
    public Transform pathParent;

    [Tooltip("سرعة حركة الشخصية")]
    //public float moveSpeed = 2.5f;

    private Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator; // للتحكم في حركة المشي
    public Transform modelRoot;

    // New: store a randomized order of points for this agent
    private Transform[] randomizedPatrolPoints;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // احصل على مكون Animator

        // تحقق من وجود المسار
        if (pathParent == null)
        {
            Debug.LogError("لم يتم تحديد مسار الدورية (Path Parent)!", this);
            enabled = false;
            return;
        }

        // احصل على كل نقاط الدورية من الكائن الأب
        patrolPoints = new Transform[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            patrolPoints[i] = pathParent.GetChild(i);
        }

        // تأكد من وجود نقاط
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("المسار لا يحتوي على أي نقاط!", this);
            enabled = false;
            return;
        }

        agent.updateRotation = false; // <--- Add this line
        agent.angularSpeed = 0;       // <--- Optional: disables agent's own turning

        // Randomize the patrol points for this agent
        randomizedPatrolPoints = (Transform[])patrolPoints.Clone();
        ShufflePatrolPoints(randomizedPatrolPoints);

        GoToNextPoint();
    }

    void Update()
    {
        // إذا وصلت الشخصية إلى وجهتها تقريباً، اذهب إلى النقطة التالية
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }

        Vector3 velocity = agent.velocity;
        velocity.y = 0; // تجاهل المحور الرأسي
        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    void GoToNextPoint()
    {
        // اختر النقطة التالية في المسار (randomized)
        agent.SetDestination(randomizedPatrolPoints[currentPointIndex].position);

        // جهز المؤشر للنقطة التي تليها
        currentPointIndex = (currentPointIndex + 1) % randomizedPatrolPoints.Length;
    }

    // Fisher-Yates shuffle
    void ShufflePatrolPoints(Transform[] points)
    {
        for (int i = points.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Transform temp = points[i];
            points[i] = points[j];
            points[j] = temp;
        }
    }
}