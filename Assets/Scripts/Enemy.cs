using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float chaseRadius = 10f;
    public float attackRadius = 2f; // Range within which the player can damage the enemy
    public float obstacleCheckDistance = 1f;
    public LayerMask obstacleLayerMask;
    public Animator animator;
    private Transform target;
    private bool isChasing = false;
    public float moveSpeed = 5f;
    public int damageAmount = 10; // Amount of damage dealt when M is pressed

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Check if the player is close enough and presses M to deal damage
        if (Vector3.Distance(target.position, transform.position) <= attackRadius && Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(damageAmount);
        }

        // Chasing logic
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            // Obstacle checking
            Vector2 direction = (target.position - transform.position).normalized;
            Vector2 boxSize = new Vector2(1f, 1f);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, direction, obstacleCheckDistance, obstacleLayerMask);
            if (hit.collider == null)
            {
                Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    // Call this method to reduce the enemy's health
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health); // Debug to show current health

        if (health <= 0)
        {
            Die();
        }
    }

    // Die method to handle death behavior
    void Die()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        if (animator != null)
        {
            animator.Play("Possum_Death"); // Play death animation
        }
        yield return new WaitForSeconds(0.5f); // Adjust duration to match the animation length
        Destroy(gameObject); // Destroy enemy object
    }
}
