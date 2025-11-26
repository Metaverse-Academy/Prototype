// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    public Image healthHeartImage;

    [Header("Damage Effect")]
    public GameObject hitEffectVolume;
    public float effectDuration = 0.25f;
    private bool isEffectActive = false;

    // --- **القسم الجديد الذي أضفناه** ---
    [Header("Death Settings")]
    [Tooltip("اسحب هنا الكائن الذي يحمل سكربت UIManager")]
    public UIManager uiManager;
    // ------------------------------------

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (hitEffectVolume != null)
        {
            hitEffectVolume.SetActive(false);
        }

        // (اختياري ولكن موصى به) التأكد من ربط UIManager
        if (uiManager == null)
        {
            Debug.LogError("لم يتم ربط UIManager في PlayerHealth! اسحب الكائن الذي يحمل السكربت إلى الخانة المخصصة.", this);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        if (hitEffectVolume != null && !isEffectActive)
        {
            StartCoroutine(PlayHitEffect());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthHeartImage != null)
        {
            healthHeartImage.fillAmount = currentHealth / maxHealth;
        }
    }

    // --- **تم تعديل هذه الدالة بالكامل** ---
    void Die()
    {
        Debug.Log("Player Died!");

        // 1. تحقق من وجود UIManager ثم قم بإظهار شاشة الموت
        if (uiManager != null)
        {
            uiManager.ShowDeathScreen();
        }
        else
        {
            // إذا لم يتم ربط UIManager، قم بإعادة تحميل المشهد كحل بديل
            Debug.LogWarning("UIManager غير مربوط، سيتم إعادة تحميل المشهد مباشرة.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return; // الخروج من الدالة
        }

        // 2. إيقاف اللاعب (يمكنك تعطيل سكربت الحركة أو الكائن بأكمله)
        // تعطيل الكائن بأكمله هو الأسهل
        gameObject.SetActive(false);

        // 3. (مهم) إظهار مؤشر الفأرة لكي يتمكن اللاعب من الضغط على الزر
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // ------------------------------------

    private IEnumerator PlayHitEffect()
    {
        isEffectActive = true;
        hitEffectVolume.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        hitEffectVolume.SetActive(false);
        isEffectActive = false;
    }
}
