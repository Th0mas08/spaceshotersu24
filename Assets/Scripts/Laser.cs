using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public int laserDamage = 20; // Damage dealt by the laser

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Check if the laser collides with an "Enemy"
        {
            Enemy enemy = collision.GetComponent<Enemy>(); // Get the Enemy script
            if (enemy != null)
            {
                enemy.TakeDamage(laserDamage); // Deal damage to the enemy
            }
            Destroy(gameObject); // Destroy the laser after hitting the enemy
        }
    }
}

