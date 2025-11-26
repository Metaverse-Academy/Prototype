// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // <-- **مهم جداً:** أضف هذا السطر لاستخدام الـ Coroutines

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    [Tooltip("اسحب هنا كائن 'Heart_Fill' الذي يمثل القلب الممتلئ")]
    public Image healthHeartImage;

    [Header("Damage Effect")] // <-- **القسم الجديد الذي أضفناه**
    [Tooltip("اسحب هنا كائن الـ Volume الذي يمثل تأثير الضربة")]
    public GameObject hitEffectVolume;
    [Tooltip("المدة التي سيبقى فيها التأثير ظاهراً (بالثواني)")]
    public float effectDuration = 0.25f;

    private bool isEffectActive = false; // لمنع تشغيل التأثير بشكل متداخل

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // التأكد من أن التأثير معطل في البداية
        if (hitEffectVolume != null)
        {
            hitEffectVolume.SetActive(false);
        }
    }

    // --- **تم تعديل هذه الدالة** ---
    public void TakeDamage(float amount)
    {
        // لا تفعل شيئاً إذا كان اللاعب ميتاً بالفعل
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        // --- **الإضافة الجديدة: تشغيل تأثير الضربة** ---
        if (hitEffectVolume != null && !isEffectActive)
        {
            StartCoroutine(PlayHitEffect());
        }
        // ---------------------------------------------

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthHeartImage != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            healthHeartImage.fillAmount = healthPercentage;
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        gameObject.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //    FindFirstObjectByType<UIManager>().ShowDeathScreen();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    // --- **الدالة الجديدة التي أضفناها لتشغيل التأثير** ---
    private IEnumerator PlayHitEffect()
    {
        isEffectActive = true;
        hitEffectVolume.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        hitEffectVolume.SetActive(false);
        isEffectActive = false;
    }
}
