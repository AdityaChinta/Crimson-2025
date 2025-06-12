using UnityEngine;

public class UndeadZombie : MonoBehaviour, IDamageable
{
    [Header("Combat Settings")]
    public int maxHealth = 50;
    public int attackDamage = 25;
    public float attackRange = 1.2f;
    public float detectionRange = 5f;
    public float moveSpeed = 1f;
    public float destroyDelay = 2f;

    private int currentHealth;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool playerDetected = false;

    [Header("Components")]
    private Transform player;
    private Animator animator;
    private Rigidbody2D zombieRigidbody;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        zombieRigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            playerDetected = true;
        }

        if (playerDetected)
        {
            if (distance > attackRange)
            {
                MoveTowardsPlayer();
                animator.Play("Walk");
            }
            else
            {
                zombieRigidbody.linearVelocity = Vector2.zero;
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            zombieRigidbody.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }

        FacePlayer();
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        zombieRigidbody.linearVelocity = new Vector2(direction * moveSpeed, zombieRigidbody.linearVelocity.y);
    }

    private void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.Play("Attack");
        yield return new WaitForSeconds(0.8f); // Sync with attack animation
        DealDamageToPlayer(attackDamage);
        yield return new WaitForSeconds(0.5f); // Cooldown
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
        animator.Play("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        zombieRigidbody.linearVelocity = Vector2.zero;
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
