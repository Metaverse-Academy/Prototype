// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    [Header("Loot & Effects")]
    [Tooltip("الـ Prefab القابل للالتقاط الذي يحتوي على الطلقات")]
    public GameObject ammoPickupPrefab;

    // --- **الإضافة الجديدة** ---
    [Tooltip("الـ Prefab القابل للالتقاط الذي يحتوي على الصحة")]
    public GameObject healthPickupPrefab; 
    // -------------------------

    [Tooltip("التأثير البصري الذي يظهر عند موت العدو (اختياري)")]
    public GameObject deathVfxPrefab;

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

    // --- **تم تعديل هذه الدالة** ---
    void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // 1. إنشاء التأثير البصري (إذا كان موجوداً)
        if (deathVfxPrefab != null)
        {
            Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
        }

        // 2. إنشاء غنيمة الطلقات (إذا كانت موجودة)
        if (ammoPickupPrefab != null)
        {
            // إنشاء الغنيمة في مكان العدو
            GameObject droppedAmmo = Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
            
            // ضبط كمية الطلقات في الغنيمة
            AmmoPickup ammoScript = droppedAmmo.GetComponent<AmmoPickup>();
            if (ammoScript != null)
            {
                ammoScript.ammoAmount = ammoAmountToDrop;
            }
        }

        // 3. إنشاء غنيمة الصحة (إذا كانت موجودة) - **الإضافة الجديدة**
        if (healthPickupPrefab != null)
        {
            // إنشاء الغنيمة بجانب غنيمة الطلقات قليلاً لتجنب التداخل
            Vector3 healthDropPosition = transform.position + new Vector3(0.5f, 0, 0.5f);
            Instantiate(healthPickupPrefab, healthDropPosition, Quaternion.identity);
        }
        // ------------------------------------

        // 4. تدمير كائن العدو
        Destroy(gameObject);
    }
}
