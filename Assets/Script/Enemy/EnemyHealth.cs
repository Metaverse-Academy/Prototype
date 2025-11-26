using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Health is now " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // يمكنك هنا إضافة تأثيرات الموت، مثل تشغيل حركة (animation) أو تدمير الكائن
        Destroy(gameObject);
    }
}
