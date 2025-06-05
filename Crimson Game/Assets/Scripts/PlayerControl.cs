using System.Collections;
using UnityEngine;
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

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;
    Vector2 moveInput;

    [Header("Animaion")]
    float idleDelay = 0.2f;
    float idleTimer = 0f;

    [Header("Running Detection")]
    float doubleTapTime = 0.3f;
    float lastTapTimeLeft = -1f;
    float lastTapTimeRight = -1f;
    bool isRunning = false;

    [Header("Respawn")]
    public float respawnDelay = 2f;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    //[Header("Sprite Commponents")]
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    Animator animator;
    SpriteRenderer mySpriteRenderer;

    public ObjectHealth playerHealth = new ObjectHealth(100,100);
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
        currentHealth = maxHealth;
        isDead = false;
    }

    void Update()
    {
        if (isDead) return;
        Run();
        SpriteDirection();
        UpdateAnimationStates();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerHealth.Heal(10);
            Debug.Log(playerHealth.currentHealth);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerHealth.DealDamage(10);
            Debug.Log(playerHealth.currentHealth);
        }
    }

    void SpriteDirection()
    {
        if (moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnMove(InputValue inputValue)
    {
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
        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector2 playerVelocity = new Vector2(moveInput.x * currentSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
    }

    void UpdateAnimationStates()
    {
        bool isWalkingNow = Mathf.Abs(moveInput.x) > 0.1f;
        bool isOnGround = IsGrounded();

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


    bool IsGrounded()
    {
        return myCollider.IsTouchingLayers(groundLayer);
    }

    void OnBasicSlash(InputValue inputValue)
    {
        if (isDead) return;
        animator.SetTrigger("basicSlash");
    }

    void OnHeavySlash(InputValue inputValue)
    {
        if (isDead) return;
        animator.SetTrigger("heavySlash");
    }

    void OnStab(InputValue inputValue)
    {
        if (isDead) return;
        animator.SetTrigger("stab");
    }

    void OnDefend(InputValue inputValue)
    {
        if (isDead) return;
        animator.SetTrigger("defend");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Clamp at 0

        // Optional: trigger hurt animation
        animator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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

}
