using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public float spawnInterval = 5f; // Time in seconds between enemy spawns
    public int maxEnemies = 10; // The maximum number of enemies on the field at once
    public int enemiesPerSpawn = 3; // The number of enemies to spawn at once
    public Vector3 spawnLocation = new Vector3(0f, 0f, 0f); // Fixed spawn location

    private int currentEnemyCount = 0; // Keeps track of the current number of enemies

    void Start()
    {
        // Start spawning enemies at regular intervals
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (currentEnemyCount < maxEnemies)
        {
            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Spawn multiple enemies at the fixed spawn location
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                // Instantiate the enemy at the fixed spawn location
                Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
                
                // Increase the current enemy count
                currentEnemyCount++;
            }
        }
    }

    // Call this function to decrease the current enemy count when an enemy is destroyed
    public void EnemyDestroyed()
    {
        currentEnemyCount--;
    }
}

