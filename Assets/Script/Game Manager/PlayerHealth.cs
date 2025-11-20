// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // <-- مهم جداً: أضف هذا السطر للتحكم بالصور
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    [Tooltip("اسحب هنا كائن 'Heart_Fill' الذي يمثل القلب الممتلئ")]
    public Image healthHeartImage; // <-- إضافة جديدة: هذا هو المتغير الخاص بالقلب



    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // --- تم تعديل هذه الدالة بالكامل ---
    void UpdateHealthUI()
    {
        if (healthHeartImage != null)
        {
            // حساب النسبة المئوية للصحة
            float healthPercentage = currentHealth / maxHealth;
            // تحديث قيمة Fill Amount للقلب
            healthHeartImage.fillAmount = healthPercentage;
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // يمكنك إضافة منطق الموت هنا
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
