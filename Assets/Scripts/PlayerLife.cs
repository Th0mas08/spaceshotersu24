using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    public Animator animator; // Reference to the animator
    public Rigidbody2D rb; // Reference to the Rigidbody2D
    public int maxHealth = 100; // Max health of the player
    private int currentHealth; // Current health of the player

    public float invincibilityTime = 1f; // Time during which player can't take damage after getting hurt

    public int playerDamage = 20; // The amount of damage the player deals to enemies

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health at the start
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap")) // If we collide with something tagged "Trap", take damage
        {
            TakeDamage(20); // Call TakeDamage() with an amount of 20 damage (can be modified)
        }
        else if (collision.gameObject.CompareTag("Enemy")) // If we collide with an enemy, deal damage to the enemy
        {
            DealDamageToEnemy(collision.gameObject); // Call the method to deal damage to the enemy
        }
    }

    private void TakeDamage(int damage) // Reduce health when taking damage
    {
        if (currentHealth > 0) // If player has health left
        {
            currentHealth -= damage; // Subtract damage from current health
            animator.SetTrigger("Hurt"); // Trigger a hurt animation (optional)

            if (currentHealth <= 0) // If health reaches 0, trigger death
            {
                Die();
            }
            else
            {
                StartCoroutine(Invincibility()); // Start invincibility period after taking damage
            }
        }
    }

    private IEnumerator Invincibility() // Make player invincible for a short period after taking damage
    {
        rb.bodyType = RigidbodyType2D.Static; // Freeze player movement temporarily (optional)
        yield return new WaitForSeconds(invincibilityTime); // Wait for the invincibility duration
        rb.bodyType = RigidbodyType2D.Dynamic; // Resume player movement after invincibility ends
    }

    private void Die() // Handle player death
    {
        animator.SetTrigger("Death"); // Play death animation
        rb.bodyType = RigidbodyType2D.Static; // Stop player movement completely
        StartCoroutine(RestartLevelAfterDelay(2f)); // Restart the level after 2 seconds
    }

    private IEnumerator RestartLevelAfterDelay(float delay) // Restart the level after a delay
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0); // Reload the scene (assuming scene index 0)
    }

    // Method to deal damage to the enemy
    private void DealDamageToEnemy(GameObject enemyObject)
    {
        // Get the Enemy script attached to the collided object (enemy)
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Apply the player's damage to the enemy
            enemy.TakeDamage(playerDamage);
        }
    }
}
