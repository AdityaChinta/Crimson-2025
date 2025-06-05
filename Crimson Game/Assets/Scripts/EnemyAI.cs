/*using UnityEngine;

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
        if (direction.x > 0) transform.localScale = new Vector3(3, 3, 3);
        else transform.localScale = new Vector3(-3, 3, 3);
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
*/

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Combat Settings")]
    public int maxHealth = 20;
    public int attack1Damage = 10;
    public int attack2Damage = 20;
    public float attackRange = 10f;
    public float detectionRange = 6f;
    public float moveSpeed = 2f;
    public float destroyDelay = 2f;

    private int currentHealth;
    private bool isDead = false;
    private bool playerDetected = false;
    private bool isAttacking = false;

    [Header("Components")]
    private Transform player;
    private Animator animator;
    private Rigidbody2D gorgonRigidbody;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        gorgonRigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Start tracking if player enters detection range (once)
        if (distance <= detectionRange)
        {
            playerDetected = true;
        }

        if (playerDetected)
        {
            if (distance > attackRange)
            {
                // Move toward the player
                MoveTowardsPlayer();
                animator.Play("Run");
            }
            else
            {
                // Stop and attack when in range
                gorgonRigidbody.linearVelocity = Vector2.zero;
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // Before detection, idle
            gorgonRigidbody.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }

        FacePlayer();
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        gorgonRigidbody.linearVelocity = new Vector2(direction * moveSpeed, gorgonRigidbody.linearVelocity.y);
    }

    private void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }



    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        int attackType = Random.Range(0, 2); // 0 or 1

        if (attackType == 0)
        {
            animator.Play("Attack2");
            yield return new WaitForSeconds(0.5f); // match Attack1 animation
            DealDamageToPlayer(attack1Damage);
        }
        else
        {
            animator.Play("Attack3");
            yield return new WaitForSeconds(0.7f); // match Attack2 animation
            DealDamageToPlayer(attack2Damage);
        }

        yield return new WaitForSeconds(0.3f); // cooldown
        isAttacking = false;
    }

    private void DealDamageToPlayer(int damage)
    {
        PlayerControl playerScript = player.GetComponent<PlayerControl>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        //animator.Play("Hurt"); - No hurt animation for Gorgon

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        gorgonRigidbody.linearVelocity = Vector2.zero;
        animator.Play("Dead");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
