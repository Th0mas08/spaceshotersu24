using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
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
