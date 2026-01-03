using UnityEngine;

/// <summary>
/// Basic enemy component. Can be extended for more complex enemy behavior.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Called when enemy takes damage from a bullet.
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death effects, score, etc. here
        Destroy(gameObject);
    }
}

