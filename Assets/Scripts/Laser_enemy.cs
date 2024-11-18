using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_enemy : MonoBehaviour
{
    public int damage = 10;  // Amount of damage the laser deals

    // This function will be called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that collided with the laser has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerLife script attached to the player
            PlayerLife playerLife = collision.GetComponent<PlayerLife>();

            if (playerLife != null)
            {
                // Call the TakeDamage method from the PlayerLife script to apply damage
                playerLife.TakeDamage(damage);
            }
        }
    }
}
