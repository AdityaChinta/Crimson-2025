/*using UnityEngine;
using System.Collections;

public class SoldierBoss : MonoBehaviour
{
    [Header("Combat Settings")]
    public int maxHealth = 200;
    public float attackRange = 1.5f;
    public float detectionRange = 8f;
    public float moveSpeed = 3f;

    [Header("Attack Damage")]
    public int slashDamage = 10;
    public int hellSlashDamage = 20;
    public int stabDamage = 15;
    public int dimensionalSlashDamage = 25;
    public int jabDamage = 8;

    private int currentHealth;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool playerDetected = false;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        animator.speed = 1.5f;
    }

    void Update()
    {
        if (isDead || isAttacking || player == null) return;

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            playerDetected = true;

        if (!playerDetected)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // stop X, keep gravity
            return;
        }

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StartCoroutine(AttackRoutine());
        }
        else
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        animator.Play("Dash");
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y); // preserve falling speed
    }

    void FacePlayer()
    {
        sprite.flipX = player.position.x < transform.position.x;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        int attackIndex = Random.Range(0, 5);
        string attackName = "";
        int damage = 0;
        float delay = 0.5f;

        switch (attackIndex)
        {
            case 0: attackName = "Slash"; damage = slashDamage; delay = 0.4f; break;
            case 1: attackName = "HellSlash"; damage = hellSlashDamage; delay = 0.5f; break;
            case 2: attackName = "Stab"; damage = stabDamage; delay = 0.45f; break;
            case 3: attackName = "DimentionalSlash"; damage = dimensionalSlashDamage; delay = 0.6f; break;
            case 4: attackName = "Jab"; damage = jabDamage; delay = 0.35f; break;
        }

        animator.Play(attackName);
        yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            PlayerControl pc = hit.GetComponent<PlayerControl>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.Play("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.Play("Dead");
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}*/
using UnityEngine;

public class SoldierBoss : MonoBehaviour,  IDamageable
{
    [Header("Attack Damage")]
    public int slashDamage = 10;
    public int hellSlashDamage = 20;
    public int stabDamage = 15;
    public int dimensionalSlashDamage = 25;
    public int jabDamage = 8;

    [Header("Combat Settings")]
    public int maxHealth = 200;
    public float attackRange = 1.5f;
    public float detectionRange = 8f;
    public float moveSpeed = 3f;
    public float destroyDelay = 2f;
    
    private int currentHealth;
    private bool isDead = false;
    private bool playerDetected = false;
    private bool isAttacking = false;

    [Header("Components")]
    private Transform player;
    private PlayerControl playerControl;
    private Animator animator;
    private Rigidbody2D soldierRigidbody;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        soldierRigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (player != null)
            playerControl = player.GetComponent<PlayerControl>();
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
                animator.Play("Dash");
            }
            else
            {
                // Stop and attack when in range
                soldierRigidbody.linearVelocity = Vector2.zero;
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // Before detection, idle
            soldierRigidbody.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }

        FacePlayer();
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        soldierRigidbody.linearVelocity = new Vector2(direction * moveSpeed, soldierRigidbody.linearVelocity.y);
    }

    private void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }



    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        int attackType = Random.Range(0, 5);

        switch (attackType)
        {
            case 0:
                animator.Play("Slash");
                yield return new WaitForSeconds(0.25f);
                DealDamageToPlayer(slashDamage);
                break;
            case 1:
                animator.Play("HellSlash");
                yield return new WaitForSeconds(0.35f);
                DealDamageToPlayer(hellSlashDamage);
                break;
            case 2:
                animator.Play("Stab");
                yield return new WaitForSeconds(0.3f);
                DealDamageToPlayer(stabDamage);
                break;
            case 3:
                animator.Play("DimensionalSlash");
                yield return new WaitForSeconds(0.24f);
                DealDamageToPlayer(dimensionalSlashDamage);
                break;
            case 4:
                animator.Play("Jab");
                yield return new WaitForSeconds(0.3f);
                DealDamageToPlayer(jabDamage);
                break;
        }

        yield return new WaitForSeconds(0.2f); // cooldown
        isAttacking = false;
    }

    private void DealDamageToPlayer(int damage)
    {
        
        if (playerControl != null)
        {
            playerControl.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        animator.Play("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        soldierRigidbody.linearVelocity = Vector2.zero;
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

