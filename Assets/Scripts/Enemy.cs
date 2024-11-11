using System.Collections;
using UnityEngine;
using TMPro; // Add this to use TextMeshProUGUI

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float chaseRadius = 10f;
    public float obstacleCheckDistance = 1f;
    public LayerMask obstacleLayerMask;
    public Animator animator;
    private Transform target;
    private bool isChasing = false;
    public float moveSpeed = 5f;

    private int pointsCount = 0; // Use gemCount instead of Gem for clarity

    [SerializeField] private TextMeshProUGUI gemText; // Make sure this is set to TextMeshProUGUI

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateGemText(); // Initialize the text display
    }

    private void UpdateGemText()
    {
        gemText.text = "Points: " + pointsCount; // Update the UI text
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
        if (animator != null)
        {
            animator.Play("Possum_Death");
        }
        yield return new WaitForSeconds(0.5f); // Adjust duration to match the animation length
        Debug.Log("Points collected!"); // Added missing semicolon
        Destroy(gameObject);
        pointsCount++;
        UpdateGemText();
    }
}