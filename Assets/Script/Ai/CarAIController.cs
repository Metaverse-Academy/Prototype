using UnityEngine;
using System.Collections.Generic;

public class CarAIController : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform pathParent;

    [Header("Movement Settings")]
    public float speed = 11f; // سرعة 40 كم/س
    public float rotationSpeed = 5.0f;
    public float reachDistance = 2.0f;

    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    
    // --- **الإضافة الجديدة: متغير لتتبع اتجاه الحركة** ---
    private bool movingForward = true;

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

        if (waypoints.Count < 2) // يجب أن يكون هناك نقطتان على الأقل للحركة
        {
            Debug.LogError("المسار يجب أن يحتوي على نقطتين على الأقل!", this);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // --- **تم تعديل هذا الجزء بالكامل** ---

        // الحصول على النقطة الحالية المستهدفة
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // حساب الاتجاه نحو النقطة المستهدفة
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0;

        // الدوران
        if (direction != Vector3.zero) // تجنب الدوران إذا كنا في نفس المكان
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // الحركة
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // التحقق من الوصول
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance < reachDistance)
        {
            // إذا وصلنا إلى النقطة، قرر ما هي النقطة التالية
            if (movingForward)
            {
                // كنا نتحرك للأمام
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    // وصلنا إلى نهاية المسار، ابدأ بالعودة
                    movingForward = false;
                    // اضبط العداد للنقطة قبل الأخيرة
                    currentWaypointIndex = waypoints.Count - 2;
                }
            }
            else
            {
                // كنا نتحرك للخلف
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    // وصلنا إلى بداية المسار، ابدأ بالتحرك للأمام مجدداً
                    movingForward = true;
                    // اضبط العداد للنقطة الثانية
                    currentWaypointIndex = 1;
                }
            }
        }
    }
}
