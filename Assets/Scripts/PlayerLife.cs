using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    public int maxHealth = 100;  // The player's max health
    private int currentHealth;    // The player's current health
    public float invincibilityTime = 1f;  // Time the player is invincible after taking damage

    private void Start()
    {
        // Initialize the player's health
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with something tagged as "Trap"
        if (collision.gameObject.CompareTag("Trap"))
        {
            TakeDamage(20);  // Deal 20 damage when hitting a trap
        }
    }

    // Method to deal damage to the player
    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;  // Decrease the player's health
            Debug.Log("Player Health: " + currentHealth);  // Log the player's health

            if (currentHealth <= 0)
            {
                Die();  // Call the Die method if health reaches 0
            }
            else
            {
                StartCoroutine(Invincibility());  // Start invincibility period
            }
        }
    }

    // Coroutine to make the player temporarily invincible after taking damage
    private IEnumerator Invincibility()
    {
        // You can add logic here to prevent further damage during invincibility, like disabling collisions or rendering effects

        // Wait for the invincibility period
        yield return new WaitForSeconds(invincibilityTime);
        
        // After the invincibility period, the player can take damage again
    }

    // Method to handle the player's death
    private void Die()
    {
        Debug.Log("Player has died!");

        // Start the coroutine to load the next scene after a delay (2 seconds for the death animation)
        StartCoroutine(LoadNextSceneAfterDelay(2f));  // 2 seconds delay for the death effect
    }

    // Coroutine to load the next scene after a delay
    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Get the current scene index and load the next scene in the build settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next scene in the build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene found in the build settings.");
            // Optionally, load the first scene or handle the case where there's no next scene.
            SceneManager.LoadScene(0);  // Load the first scene if no next scene exists
        }
    }
}
