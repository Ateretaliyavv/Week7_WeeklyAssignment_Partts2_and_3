using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Player hit! HP = " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player dead!");
            // פה אפשר לעבור ל-GameOver / Restart וכו'
        }
    }
}
