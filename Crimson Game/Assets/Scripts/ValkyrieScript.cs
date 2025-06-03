using UnityEngine;

public class ValkyrieScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 7f;
    public float attackRange = 1.5f;

    [Header("Stats")]
    public int maxHP = 100;
    public int defense = 5;

    [Header("Attack")]
    public int attack1Damage = 10; // No cooldown
    public int attack2Damage = 15;
    public int attack3Damage = 20;
    public float attack2Cooldown = 4f;
    public float attack3Cooldown = 6f;

    private float lastAttack2Time = -Mathf.Infinity;
    private float lastAttack3Time = -Mathf.Infinity;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private int currentHP;
    private bool isDead = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        if (distance > attackRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            Attack();
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        animator.SetBool("isMoving", true);
    }

    private void Attack()
    {
        animator.SetBool("isMoving", false);

        bool canAttack2 = Time.time - lastAttack2Time >= attack2Cooldown;
        bool canAttack3 = Time.time - lastAttack3Time >= attack3Cooldown;

        int[] availableAttacks = canAttack2 && canAttack3 ? new[] { 1, 2, 3 } :
                                 canAttack2 ? new[] { 1, 2 } :
                                 canAttack3 ? new[] { 1, 3 } :
                                 new[] { 1 };

        int chosen = availableAttacks[Random.Range(0, availableAttacks.Length)];

        switch (chosen)
        {
            case 1:
                animator.SetTrigger("Attack1");
                DealDamage(attack1Damage);
                break;
            case 2:
                animator.SetTrigger("Attack2");
                DealDamage(attack2Damage);
                lastAttack2Time = Time.time;
                break;
            case 3:
                animator.SetTrigger("Attack3");
                DealDamage(attack3Damage);
                lastAttack3Time = Time.time;
                break;
        }
    }

    private void DealDamage(int damage)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int actualDamage = Mathf.Max(damage - defense, 1);
        currentHP -= actualDamage;

        Debug.Log($"Enemy took {actualDamage} damage. Remaining HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 3f);
    }
}
