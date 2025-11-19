using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimplePatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("اسحب هنا الكائن الرئيسي الذي يحتوي على نقاط المسار")]
    public Transform pathParent;

    [Tooltip("سرعة حركة الشخصية")]
    public float moveSpeed = 2.5f;

    private Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator; // للتحكم في حركة المشي

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

        // اضبط السرعة وابدأ الحركة نحو النقطة الأولى
        agent.speed = moveSpeed;
        GoToNextPoint();
    }

    void Update()
    {
        // إذا وصلت الشخصية إلى وجهتها تقريباً، اذهب إلى النقطة التالية
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }

        // تحديث حركة المشي في Animator
        if (animator != null)
        {
            // اجعل قيمة "Speed" في الـ Animator تساوي سرعة الـ Agent الحالية
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void GoToNextPoint()
    {
        // اختر النقطة التالية في المسار
        agent.SetDestination(patrolPoints[currentPointIndex].position);

        // جهز المؤشر للنقطة التي تليها
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }
}
