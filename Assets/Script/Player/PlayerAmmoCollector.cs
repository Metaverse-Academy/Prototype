// PlayerAmmoCollector.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAmmoCollector : MonoBehaviour
{
    // --- **التغيير هنا: اجعل المتغير عاماً** ---
    [Header("Weapon Reference")]
    [Tooltip("اسحب هنا الكائن الذي يحمل سكربت WeaponController")]
    public WeaponController weaponController; // <-- أصبح public

    void Start()
    {
        // لم نعد بحاجة للبحث التلقائي، لكن سنبقي التحقق
        if (weaponController == null)
        {
            Debug.LogError("لم يتم ربط WeaponController يدوياً في الـ Inspector!", this);
        }
    }

    // دالة OnTriggerEnter تبقى كما هي تماماً
    private void OnTriggerEnter(Collider other)
    {
        AmmoPickup pickup = other.GetComponent<AmmoPickup>();
        if (pickup != null)
        {
            if (weaponController != null)
            {
                weaponController.AddReserveAmmo(pickup.ammoAmount);
                Destroy(other.gameObject);
            }
        }
    }
}
