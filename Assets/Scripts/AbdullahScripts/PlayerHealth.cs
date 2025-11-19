// PlayerHealth.cs
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + Mathf.Clamp(currentHealth, 0, maxHealth).ToString("F0");
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Add respawn, game over, etc.
    }
}