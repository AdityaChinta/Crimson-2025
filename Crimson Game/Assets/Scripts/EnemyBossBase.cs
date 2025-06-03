using UnityEngine;

public abstract class EnemyBossBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP;
    public int defense;
    public float moveSpeed;
    public float detectionRange;
    public float attackRange;

    [Header("Attack Damage")]
    public int attack1Damage;
    public int attack2Damage;
    public int attack3Damage;

    [Header("Cooldowns")]
    public float attack2Cooldown;
    public float attack3Cooldown;

    protected float lastAttack2Time = -Mathf.Infinity;
    protected float lastAttack3Time = -Mathf.Infinity;

    protected int currentHP;
    protected bool isDead = false;

    protected Transform player;
    protected Animator animator;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        if (distance > attackRange)
            MoveTowardPlayer();
        else
            Attack();
    }

    protected void MoveTowardPlayer()
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

    protected virtual void Attack()
    {
        animator.SetBool("isMoving", false);

        bool can2 = Time.time - lastAttack2Time >= attack2Cooldown;
        bool can3 = Time.time - lastAttack3Time >= attack3Cooldown;

        int[] choices = can2 && can3 ? new[] { 1, 2, 3 } :
                        can2 ? new[] { 1, 2 } :
                        can3 ? new[] { 1, 3 } :
                        new[] { 1 };

        int chosen = choices[Random.Range(0, choices.Length)];

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

    protected void DealDamage(int damage)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        hp?.TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        int actual = Mathf.Max(damage - defense, 1);
        currentHP -= actual;

        Debug.Log($"{name} took {actual} damage. HP: {currentHP}");

        if (currentHP <= 0)
            Die();
        else
            animator.SetTrigger("Hurt");
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 4f);
    }
}
