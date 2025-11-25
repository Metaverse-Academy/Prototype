// PlayerAmmoCollector.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAmmoCollector : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("اسحب هنا الكائن الذي يحمل سكربت WeaponController")]
    public WeaponController weaponController;

    // --- **الإضافة الجديدة** ---
    [Tooltip("اسحب هنا الكائن الذي يحمل سكربت PlayerHealth (غالباً هو اللاعب نفسه)")]
    public PlayerHealth playerHealth;
    // -------------------------

    void Start()
    {
        // التحقق من أن المكونات تم ربطها في الـ Inspector
        if (weaponController == null)
        {
            Debug.LogError("لم يتم ربط WeaponController في PlayerAmmoCollector!", this);
        }
        if (playerHealth == null)
        {
            Debug.LogError("لم يتم ربط PlayerHealth في PlayerAmmoCollector!", this);
        }
    }

    // --- **تم تعديل هذه الدالة بالكامل** ---
    private void OnTriggerEnter(Collider other)
    {
        // أولاً، تحقق مما إذا كان الكائن هو غنيمة ذخيرة
        AmmoPickup ammoPickup = other.GetComponent<AmmoPickup>();
        if (ammoPickup != null)
        {
            // تأكد من وجود weaponController قبل استخدامه
            if (weaponController != null)
            {
                Debug.Log("Ammo picked up: " + ammoPickup.ammoAmount);
                weaponController.AddReserveAmmo(ammoPickup.ammoAmount);
                Destroy(other.gameObject); // تدمير الغنيمة بعد التقاطها
            }
            return; // الخروج من الدالة لأننا وجدنا ما نبحث عنه
        }

        // إذا لم يكن غنيمة ذخيرة، تحقق مما إذا كان غنيمة صحة
        HealthPickup healthPickup = other.GetComponent<HealthPickup>();
        if (healthPickup != null)
        {
            // تأكد من وجود playerHealth قبل استخدامه
            if (playerHealth != null)
            {
                Debug.Log("Health picked up: " + healthPickup.healthAmount);
                playerHealth.Heal(healthPickup.healthAmount); // استدعاء دالة الشفاء
                Destroy(other.gameObject); // تدمير الغنيمة بعد التقاطها
            }
        }
    }
}
