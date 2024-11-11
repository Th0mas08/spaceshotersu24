using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public int maxHealth = 100;
    private int currentHealth;
    public float invincibilityTime = 1f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            TakeDamage(20);
        }
    }

    private void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            animator.SetTrigger("Hurt");

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Invincibility());
            }
        }
    }

    private IEnumerator Invincibility()
    {
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(invincibilityTime);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Die()
    {
        animator.SetTrigger("Death");
        rb.bodyType = RigidbodyType2D.Static;
        StartCoroutine(RestartLevelAfterDelay(2f));
    }

    private IEnumerator RestartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }
}
