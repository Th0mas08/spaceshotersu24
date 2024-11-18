using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_shoot : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireRate = 0.2f;  // Time in seconds between each shot
    private float nextFireTime = 0f;  // Time when the next shot is allowed
    private bool isAutoShooting = true;  // To toggle auto-shooting

    void Update()
    {
        // Check if enough time has passed to shoot again
        if (isAutoShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;  // Set the next allowed fire time
        }
    }

    void Shoot()
    {
        // Instantiate the bullet and store the reference
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Schedule the bullet to be destroyed after 1 seconds
        Destroy(bullet, 1f);
    }
}