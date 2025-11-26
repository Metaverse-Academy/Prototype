// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    [Header("Loot & Effects")]
    [Tooltip("الـ Prefab القابل للالتقاط الذي يحتوي على الطلقات")]
    public GameObject ammoPickupPrefab; // <-- هذا هو المهم

    [Tooltip("التأثير البصري الذي يظهر عند موت العدو (اختياري)")]
    public GameObject deathVfxPrefab; // <-- هذا هو التأثير البصري

    [Tooltip("عدد الطلقات التي سيسقطها هذا العدو")]
    public int ammoAmountToDrop = 6;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // --- **المنطق الجديد والمعدل** ---

        // 1. إنشاء التأثير البصري (إذا كان موجوداً)
        if (deathVfxPrefab != null)
        {
            Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
        }

        // 2. إنشاء غنيمة الطلقات القابلة للالتقاط (إذا كانت موجودة)
        if (ammoPickupPrefab != null)
        {
            GameObject droppedLoot = Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
            
            AmmoPickup pickupScript = droppedLoot.GetComponent<AmmoPickup>();
            if (pickupScript != null)
            {
                pickupScript.ammoAmount = ammoAmountToDrop;
            }
        }
        // ------------------------------------

        // تدمير كائن العدو
        Destroy(gameObject);
    }
}
