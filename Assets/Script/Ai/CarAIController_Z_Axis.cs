using UnityEngine;
using System.Collections.Generic;

// تأكد من أن اسم الكلاس يطابق اسم الملف
public class CarAIController_Z_Axis : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform pathParent;

    [Header("Movement Settings")]
    public float speed = 11f;
    public float rotationSpeed = 5.0f;
    public float reachDistance = 2.0f;

    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private bool movingForwardInPath = true; // تم تغيير الاسم ليكون أوضح

    void Start()
    {
        if (pathParent == null)
        {
            Debug.LogError("لم يتم تحديد مسار للسيارة!", this);
            enabled = false;
            return;
        }

        waypoints = new List<Transform>();
        foreach (Transform child in pathParent)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count < 2)
        {
            Debug.LogError("المسار يجب أن يحتوي على نقطتين على الأقل!", this);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // --- **التعديل الأول: حساب الاتجاه على محور Z فقط** ---
        Vector3 fullDirection = targetWaypoint.position - transform.position;
        // تصفير المحاور التي لا نريدها (X و Y)
        Vector3 direction = new Vector3(0, 0, fullDirection.z);

        // --- **التعديل الثاني: الدوران** ---
        // نجعل السيارة تنظر دائماً إما للأمام أو للخلف
        if (direction.z > 0 || direction.z < 0)
        {
            // حدد الاتجاه المطلوب (أمام أو خلف)
            Vector3 targetForward = direction.z > 0 ? Vector3.forward : Vector3.back;
            Quaternion targetRot = Quaternion.LookRotation(targetForward);

            // احتفظ بزوايا X و Z الحالية، وادمج فقط زاوية Y (yaw)
            Vector3 currentEuler = transform.rotation.eulerAngles;
            float targetYaw = targetRot.eulerAngles.y;
            float newYaw = Mathf.LerpAngle(currentEuler.y, targetYaw, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(currentEuler.x, newYaw, currentEuler.z);
        }

        // --- **التعديل الثالث: الحركة** ---
        // تحريك السيارة نحو موضع Z الخاص بالنقطة المستهدفة
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetWaypoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // --- **التعديل الرابع: التحقق من الوصول على محور Z فقط** ---
        float distance = Mathf.Abs(transform.position.z - targetWaypoint.position.z);
        if (distance < reachDistance)
        {
            // (منطق تبديل النقاط يبقى كما هو)
            if (movingForwardInPath)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    movingForwardInPath = false;
                    currentWaypointIndex = waypoints.Count - 2;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    movingForwardInPath = true;
                    currentWaypointIndex = 1;
                }
            }
        }
    }
}
