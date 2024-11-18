using System.Collections;
using UnityEngine;
using TMPro; // Add this to use TextMeshProUGUI

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float chaseRadius = 10f;
    public float obstacleCheckDistance = 1f;
    public LayerMask obstacleLayerMask;
    private Transform target;
    private bool isChasing = false;
    public float moveSpeed = 5f;

    [SerializeField] private TextMeshProUGUI gemText; // Make sure this is set to TextMeshProUGUI

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Check if the player is within the chase radius
        isChasing = Vector3.Distance(target.position, transform.position) <= chaseRadius;

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

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        // Here you can add a death animation, sound effect, or other actions if desired.
        Debug.Log("Enemy defeated, adding points!");

        // Add points to the GameManager when the enemy dies
        GameManager.Instance.AddPoints(1);  // Adds 1 point for each defeated enemy

        // Destroy the enemy object after a short delay (optional)
        yield return new WaitForSeconds(0.5f); // Adjust duration as needed
        
        Destroy(gameObject);
    }
}
