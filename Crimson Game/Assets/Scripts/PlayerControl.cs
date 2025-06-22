using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerControl : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    public static bool canMove = true;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;
    Vector2 moveInput;
    //public bool hasEnteredNextScene = false;

    [Header("Animaion")]
    float idleDelay = 0.2f;
    float idleTimer = 0f;

    [Header("Running Detection")]
    float doubleTapTime = 0.3f;
    float lastTapTimeLeft = -1f;
    float lastTapTimeRight = -1f;
    bool isRunning = false;
    bool isDefending = false;

    [Header("Respawn")]
    public float respawnDelay = 2f;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public int basicSlashDamage = 10;
    public int heavySlashDamage = 20;
    public int stabDamage = 15;
    public float attackRange = 1.2f;
    public LayerMask enemyLayer;
    public IDamageable currentTarget; // EnemyAI for Gorgon // EnemyValkyrie for Valkyrie

    GameObject play;

    //[Header("Sprite Commponents")]
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    Animator animator;
    SpriteRenderer mySpriteRenderer;
    public float scaleX;
    public float scaleY;

    //public ObjectHealth playerHealth = new ObjectHealth(100,100);
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("isJumping", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdling", true);
        animator.SetBool("isRunning", false);
        play = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
        isDead = false;
        scaleX = play.transform.localScale.x;
        scaleY = play.transform.localScale.y;
    }

    void Update()
    {
        //if (!canMove) { animator.SetBool("isIdling", true); return;  }
        if (isDead) return;
        Run();
        SpriteDirection();
        UpdateAnimationStates();

        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerHealth.Heal(10);
            Debug.Log(playerHealth.currentHealth);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerHealth.DealDamage(10);
            Debug.Log(playerHealth.currentHealth);
        }*/
    }

    void SpriteDirection()
    {
        if (canMove)
        {
            if (moveInput.x > 0.1f)
                transform.localScale = new Vector3(scaleX, scaleY, 4);
            else if (moveInput.x < -0.1f)
                transform.localScale = new Vector3(-scaleX, scaleY, 4);
        }
    }

    void OnMove(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;
        Vector2 input = inputValue.Get<Vector2>();

        if (input.x < -0.1f)
        {
            float timeSinceLastTap = Time.time - lastTapTimeLeft;
            if (timeSinceLastTap <= doubleTapTime)
                isRunning = true;
            lastTapTimeLeft = Time.time;
        }
        else if (input.x > 0.1f)
        {
            float timeSinceLastTap = Time.time - lastTapTimeRight;
            if (timeSinceLastTap <= doubleTapTime)
                isRunning = true;
            lastTapTimeRight = Time.time;
        }
        else
            isRunning = false;
        if (input == Vector2.zero)
        {
            isRunning = false;
            moveInput = Vector2.zero;
            return;
        }

        if (input != Vector2.zero || moveInput != Vector2.zero)
        {
            moveInput = input;
        }
    }

    void OnJump(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;

        if (IsGrounded() && inputValue.isPressed)
        {
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }
        isRunning = false;
    }


    void Run()
    {
        if(!canMove) { moveInput = Vector2.zero; }
        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector2 playerVelocity = new Vector2(moveInput.x * currentSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
    }

    void UpdateAnimationStates()
    {
        bool isWalkingNow = Mathf.Abs(moveInput.x) > 0.1f;
        bool isOnGround = IsGrounded();

        if (!canMove)
        {
            animator.Play("Idle");
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
        }

        else
        {
            if (!isOnGround)
            {
                // In air: jump animation active, running and walking off
                idleTimer = 0f;
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
            }
            else
            {
                animator.SetBool("isJumping", false);
                if (isRunning && isWalkingNow)
                {
                    idleTimer = 0f;
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);
                }
                else if (isWalkingNow)
                {
                    idleTimer = 0f;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    idleTimer += Time.deltaTime;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", idleTimer < idleDelay);
                }
            }
        }
    }


    bool IsGrounded()
    {
        return myCollider.IsTouchingLayers(groundLayer);
    }

    void OnBasicSlash(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;
        animator.SetTrigger("basicSlash");
        DealDamageToEnemy(basicSlashDamage);
    }

    void OnHeavySlash(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;
        animator.SetTrigger("heavySlash");
        DealDamageToEnemy(heavySlashDamage);
    }

    void OnStab(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;
        animator.SetTrigger("stab");
        DealDamageToEnemy(stabDamage);
    }

    void OnDefend(InputValue inputValue)
    {
        if (!canMove) {  return;  }
        if (isDead) return;
        animator.SetTrigger("defend");
        isDefending = inputValue.isPressed;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isDefending)
            damage = Mathf.RoundToInt(damage * 0.3f);

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Clamp at 0

        // Optional: trigger hurt animation
        animator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    void DealDamageToEnemy(int damage)
    {
        Vector2 attackOrigin = transform.position + new Vector3(transform.localScale.x * 0.8f, 0f); // In front of the player
        Collider2D hit = Physics2D.OverlapCircle(attackOrigin, attackRange, enemyLayer);

        if (hit != null)
        {
            IDamageable enemy = hit.GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }


    /*void DealDamageToEnemy(int damage)
    {
        // Detect enemies in front of the player within attack range
        Vector2 attackOrigin = transform.position + new Vector3(transform.localScale.x * 0.8f, 0f); // Slightly in front
        Collider2D hit = Physics2D.OverlapCircle(attackOrigin, attackRange, enemyLayer);

        if (hit != null)
        {
            EnemyMage enemy = hit.GetComponent<EnemyMage>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }*/




    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop player movement
        moveInput = Vector2.zero;
        myRigidbody.linearVelocity = Vector2.zero;

        // Play death animation
        animator.SetTrigger("die");
        this.enabled = false;
        StartCoroutine(RespawnAfterDelay());

        // Disable further input or controls if needed
        // You can disable this script or input system here
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ResetPlayer()
    {
        isDead = false;
        currentHealth = maxHealth;
        animator.ResetTrigger("die");
        animator.Play("Idle");
        this.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackOrigin = transform.position + new Vector3(transform.localScale.x * 0.5f, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin, attackRange);
    }

}
