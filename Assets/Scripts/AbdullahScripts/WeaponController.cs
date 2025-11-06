using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 25f;          // مقدار الضرر لكل طلقة
    public float range = 100f;          // مدى إطلاق النار
    public Camera playerCamera;         // كاميرا اللاعب لتحديد اتجاه الإطلاق

    [Header("Effects")]
    public ParticleSystem muzzleFlash;  // تأثير وميض الفوهة (اختياري)
    public GameObject impactEffect;     // تأثير الارتطام بالأسطح (اختياري)

   

    public void Shoot()
    {
        // --- تشغيل التأثيرات البصرية ---
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // --- الجزء الأهم: إطلاق الشعاع (Raycast) ---
        RaycastHit hitInfo; // متغير لتخزين معلومات الاصطدام

        // Physics.Raycast يطلق شعاعاً غير مرئي من الكاميرا إلى الأمام
        // إذا اصطدم الشعاع بشيء ضمن المدى المحدد (range)...
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, range))
        {
            Debug.Log("Hit: " + hitInfo.transform.name); // لطباعة اسم الكائن المصاب في الـ Console

            // --- تطبيق الضرر ---
            // ابحث عن سكربت الصحة (مثلاً EnemyHealth) على الكائن المصاب
            EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
            if (enemy != null) // إذا كان الكائن المصاب لديه سكربت صحة...
            {
                enemy.TakeDamage(damage); // ...استدعِ دالة إلحاق الضرر
            }

            // --- إنشاء تأثير الارتطام ---
            if (impactEffect != null)
            {
                // أنشئ نسخة من تأثير الارتطام في مكان الاصطدام وبالزاوية الصحيحة
                Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }
        }
    }
}
