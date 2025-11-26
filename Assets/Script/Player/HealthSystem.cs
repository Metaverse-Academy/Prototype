using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("Health UI Elements")]
    private float currentHealth;
    public Image fullHealth;
    public Image sixityHealth;
    public Image thirtyHealth;
    public Image zeroHealth;
    public TMP_Text healthText;

    [Header("Damage Effect")]
    public GameObject hitEffectVolume;
    public float effectDuration = 0.25f;
    private bool isEffectActive = false;

    [Header("Death Settings")]
    [Tooltip("اسحب هنا الكائن الذي يحمل سكربت UIManager")]
    public UIManager uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (hitEffectVolume != null)
        {
            hitEffectVolume.SetActive(false);
        }
        
        if (uiManager == null)
        {
            Debug.LogError("لم يتم ربط UIManager في PlayerHealth! اسحب الكائن الذي يحمل السكربت إلى الخانة المخصصة.", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthUI();
        if (hitEffectVolume != null && !isEffectActive)
        {
            StartCoroutine(PlayHitEffect());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    private void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString("0") + "%";

        switch (currentHealth)
        {
            case float n when (n > 60):
                fullHealth.enabled = true;
                sixityHealth.enabled = false;
                thirtyHealth.enabled = false;
                zeroHealth.enabled = false;
                break;
            case float n when (n <= 60 && n > 30):
                fullHealth.enabled = false;
                sixityHealth.enabled = true;
                thirtyHealth.enabled = false;
                zeroHealth.enabled = false;
                break;
            case float n when (n <= 30 && n > 0):
                fullHealth.enabled = false;
                sixityHealth.enabled = false;
                thirtyHealth.enabled = true;
                zeroHealth.enabled = false;
                break;
            case float n when (n <= 0):
                fullHealth.enabled = false;
                sixityHealth.enabled = false;
                thirtyHealth.enabled = false;
                zeroHealth.enabled = true;
                break;
        }
    }
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
