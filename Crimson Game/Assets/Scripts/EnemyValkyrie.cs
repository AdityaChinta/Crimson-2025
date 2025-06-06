using UnityEngine;

public class EnemyValkyrie : MonoBehaviour, IDamageable
{
    [Header("Combat Settings")]
    public int maxHealth = 20;
    public int attack1Damage = 10;
    public int attack2Damage = 20;
    public int attack3Damage = 25;
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
    private Rigidbody2D valkyrieRigidbody;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        valkyrieRigidbody = GetComponent<Rigidbody2D>();
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
                valkyrieRigidbody.linearVelocity = Vector2.zero;
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // Before detection, idle
            valkyrieRigidbody.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }

        FacePlayer();
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        valkyrieRigidbody.linearVelocity = new Vector2(direction * moveSpeed, valkyrieRigidbody.linearVelocity.y);
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
            animator.Play("Attack1");
            yield return new WaitForSeconds(0.5f); // match Attack1 animation
            DealDamageToPlayer(attack1Damage);
        }
        else if (attackType == 1)
        {
            animator.Play("Attack2");
            yield return new WaitForSeconds(0.7f); // match Attack2 animation
            DealDamageToPlayer(attack2Damage);
        }
        else
        {
            animator.Play("Attack3");
            yield return new WaitForSeconds(0.7f);
            DealDamageToPlayer(attack3Damage);
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
        animator.Play("BeingHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        valkyrieRigidbody.linearVelocity = Vector2.zero;
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

