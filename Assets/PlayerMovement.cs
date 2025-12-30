using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Rigidbody2D rb;
    private Vector2 movingInput;
    public LayerMask groundLayer;


    public Transform headCheck;
    public float headCheckRadius = 0.2f;

    public CapsuleCollider2D playerCollider;
    public Vector2 standingSize;
    public Vector2 crouchingSize;
    public float crouchHeight, standingHeight;
    private bool wantsCrouch;

    public float speed = 5f;
    private float jump = 10f;
    bool isGrounded;
    private float blocks = 0f;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Enable(); 
    }

    void Start()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
        controls.Player.Crouch.performed += OnCrouch;
        controls.Player.Crouch.canceled += OnCrouch;
        rb = GetComponent<Rigidbody2D>(); 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        //rb.linearVelocity = movingInput * speed;
    }

    private void FixedUpdate()
    {
        if (movingInput != Vector2.zero) 
        {
            rb.linearVelocity = new Vector2(movingInput.x * speed, rb.linearVelocity.y);
        }

        if(wantsCrouch && isGrounded)
        {
            Crouch();
        }
        else
        {
            StandAttempt();
        }
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movingInput = context.ReadValue<Vector2>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            wantsCrouch = true;
        }
        else
        {
            wantsCrouch = false;
        }
    }

    public void Crouch()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, crouchHeight);
        playerCollider.offset = new Vector2(0, -0.5f);
        speed = 4f;
    }
    public void StandAttempt()
    {
        bool notblocked = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        if (notblocked)
        {
            Debug.Log("uncrouched");
            playerCollider.size = new Vector2(playerCollider.size.x, standingHeight);
            playerCollider.offset = new Vector2(0, 0);
            speed = 5f;
        }
    }

   

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            Debug.Log("Jumping");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            Debug.Log("Touching ground");
            isGrounded = true;
            blocks++;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            blocks--;
            if(blocks == 0)
            {
                isGrounded = false;
            }
        }
    }
}
