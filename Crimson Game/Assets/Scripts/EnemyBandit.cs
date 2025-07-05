using UnityEngine;

public class EnemyBandit : MonoBehaviour, IDamageable
{
    [Header("Combat Settings")]
    public int maxHealth = 10;
    public int attackDamage = 5;
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float hurtToIdleDelay = 0.5f;
    public float kneelDelay = 0.5f;

    private int currentHealth;
    private bool isDead = false;
    private bool isHurt = false;
    private float lastAttackTime;

    [Header("Components")]
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private Transform player;
    private Vector3 initialScale;

    [Header("For dialogue")]
    public DialogueManager diag;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        initialScale = transform.localScale;

        animator.Play("Idle");
    }

    private void Update()
    {
        if (isDead || isHurt || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Flip to face player
        FacePlayer();

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack());
                lastAttackTime = Time.time;
            }
        }
        else if (distance <= detectionRange)
        {
            MoveTowardPlayer();
            animator.Play("Run");
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }
    }

    private void MoveTowardPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void FacePlayer()
    {
        if (player == null) return;

        bool playerIsLeft = player.position.x < transform.position.x;
        float scaleX = Mathf.Abs(initialScale.x);
        transform.localScale = new Vector3(playerIsLeft ? scaleX : -scaleX, initialScale.y, initialScale.z);
    }

    private System.Collections.IEnumerator Attack()
    {
        rb.linearVelocity = Vector2.zero;
        animator.Play("Attack1");

        yield return new WaitForSeconds(0.5f); // Match animation length

        PlayerControl playerScript = player.GetComponent<PlayerControl>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        StartCoroutine(HurtThenIdle());

        if (currentHealth <= 0)
        {
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            // col.enabled = false;
            StopAllCoroutines();
            StartCoroutine(PlayKneelAfterHurt());
        }
    }

    private System.Collections.IEnumerator HurtThenIdle()
    {
        isHurt = true;
        rb.linearVelocity = Vector2.zero;
        animator.Play("Hurt");

        yield return new WaitForSeconds(hurtToIdleDelay);

        if (!isDead)
        {
            animator.Play("Idle");
            isHurt = false;
        }

    }

    private System.Collections.IEnumerator PlayKneelAfterHurt()
    {
        animator.Play("Hurt");
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(kneelDelay);

        animator.Play("Kneel");

        yield return new WaitForSeconds(kneelDelay);
        PlayerControl.canMove = false;
        diag.ConvoStart();

        yield return new WaitForSeconds(10);
        col.enabled = false;
        this.enabled = false;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
