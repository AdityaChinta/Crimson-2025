using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    public int damage = 10;
    public float moveSpeed = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;

    [Header("Detection")]
    public Transform player;
    public LayerMask playerLayer;

    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;

    private int currentHealth;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else
        {
            MoveTowardPlayer();
        }
    }

    void MoveTowardPlayer()
    {
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;

            // Do damage (example assumes PlayerHealth exists)
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (hit)
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("dead");
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        // Optional: Destroy(gameObject, delay);
    }

    // Visualize attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
