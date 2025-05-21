using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] float runSpeed = 9f;
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;

    [Header("Crouch Colliders")]
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    private Rigidbody2D myRigidbody;
    private CapsuleCollider2D myBodyCollider;
    private BoxCollider2D myFeetCollider;
    private Vector2 moveInput;
    private bool isCrouching, canJump;

    public void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.gravityScale = 2.5f;
        myBodyCollider = GetComponent<CapsuleCollider2D>();
    }

    public void FixedUpdate()
    {
        float speed = isCrouching ? runSpeed * crouchSpeedMultiplier : runSpeed;
        myRigidbody.linearVelocity = new Vector2 (moveInput.x * speed, myRigidbody.linearVelocity.y);
    }

    public void Update()
    {
        if(moveInput.x != 0)
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), 1f);   
    }

    public void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if(canJump && !isCrouching)
        {
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
            canJump = false;
        }
        /*if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if(value.isPressed)
            myRigidbody.linearVelocity += new Vector2 (0f, jumpForce);*/
    }
    public void OnCrouch(InputValue value)
    {
        bool pressed = value.isPressed;
        isCrouching = pressed;
        if(standingCollider)
            standingCollider.enabled = !pressed;
        if(crouchingCollider)
            crouchingCollider.enabled = pressed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            canJump = true;
    }

    private void OCollisionExit2D(Collision2D collision)
    {
        canJump = false;
    }

}
