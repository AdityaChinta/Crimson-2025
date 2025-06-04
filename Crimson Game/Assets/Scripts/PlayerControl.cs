using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerControl : MonoBehaviour
{
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

    [Header("Ground Check")]
    public LayerMask groundLayer;
    
    //[Header("Sprite Commponents")]
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    Animator animator;
    SpriteRenderer mySpriteRenderer;
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
    }

    void Update()
    {
        Run();
        SpriteDirection();
        UpdateAnimationStates();
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
        animator.SetTrigger("basicSlash");
    }

    void OnHeavySlash(InputValue inputValue)
    {
        animator.SetTrigger("heavySlash");
    }

    void OnStab(InputValue inputValue)
    {
        animator.SetTrigger("stab");
    }

}
