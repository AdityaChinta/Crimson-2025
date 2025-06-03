using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Combat")]
    public int maxHealth = 100;
    public int attackDamage = 25;
    public float attackCooldown = 1f;
    public float attackRange = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("UI & Effects")]
    public Slider healthBar;
    public ParticleSystem damageEffect;
    public ParticleSystem deathEffect;

    [Header("Crouch Colliders")]
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int currentHealth;
    private bool isAlive = true;
    private bool isCrouching = false;
    private bool canJump = false;
    private float lastAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3.0f;
        currentHealth = maxHealth;

        if (healthBar)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Update()
    {
        if (!isAlive) return;
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!isAlive || !canJump || isCrouching) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canJump = false;
    }

    public void OnCrouch(InputValue value)
    {
        if (!isAlive) return;

        isCrouching = value.isPressed;

        if (standingCollider) standingCollider.enabled = !isCrouching;
        if (crouchingCollider) crouchingCollider.enabled = isCrouching;
    }

    public void OnAttack(InputValue value)
    {
        if (!isAlive || Time.time < lastAttackTime + attackCooldown) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            var enemyHealth = enemy.GetComponent<EnemyBossBase>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }

        lastAttackTime = Time.time;
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (healthBar) healthBar.value = currentHealth;

        if (damageEffect)
            Instantiate(damageEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canJump = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canJump = false;
    }

    private void FlipSprite()
    {
        if (moveInput.x != 0)
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
