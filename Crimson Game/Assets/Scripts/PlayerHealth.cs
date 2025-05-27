using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;

    [Header("Combat Settings")]
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    [Header("Detection Settings")]
    public LayerMask playerLayer;

    private int currentHealth;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private bool movingRight = true;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
        CheckEdgeOrWall();
    }

    private void Move()
    {
        float direction = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void CheckEdgeOrWall()
    {
        // Check for ground ahead
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // Check for wall in front
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(transform.position, direction, 0.5f, groundLayer);

        if (!groundInfo.collider || wallInfo.collider)
        {
            Flip();
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            AttackPlayer(collision.gameObject);
        }
    }

    private void AttackPlayer(GameObject player)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Example: Assume player has a PlayerHealth script
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: death animation, sound, etc.
        Destroy(gameObject);
    }

    // Debugging helper
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
