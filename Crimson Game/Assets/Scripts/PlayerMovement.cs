using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 7.5f;
    private Rigidbody2D myRigidbody;
    private CapsuleCollider2D myBodyCollider;
    private Vector2 moveInput;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        Run();
    }
    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if(value.isPressed)
            myRigidbody.linearVelocity += new Vector2 (0f, jumpSpeed);
    }
    void Run()
    {
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
    }
}
