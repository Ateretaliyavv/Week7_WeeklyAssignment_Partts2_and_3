using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("UI Text Settings")]
    [SerializeField] private string healthPrefixText = "";
    [SerializeField] private string gameOverText = "GAME OVER";
    [SerializeField] private Color gameOverColor = Color.red;

    [Header("Game Over Logic")]
    [SerializeField] private bool freezeTimeOnDeath = true;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        // מוודאים שהחיים לא יורדים מתחת לאפס
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player hit! HP = " + currentHealth);
        UpdateUI();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            // שימוש במשתנה במקום לכתוב "Health: " ידנית
            healthText.text = healthPrefixText + currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player dead!");

        if (freezeTimeOnDeath)
        {
            Time.timeScale = 0f;
        }

        if (healthText != null)
        {
            healthText.text = gameOverText; // שימוש במשתנה
            healthText.color = gameOverColor; // שימוש במשתנה
        }
    }
}
