using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering;


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
    public Vector2 standingOffset;
    public Vector2 crouchingOffset;

    public float speed = 5f;
    private float jump = 10f;
    public float recoilHorizontalStrength = 7f;
    public float recoilVerticalStrength = 7f;
    public float standardGravity = 2f;
    public float recoilGravity = 3f;
    bool isGrounded;
    bool tookDamage = false;
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

        if(wantsCrouch )
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
        if (context.performed)
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
        playerCollider.offset = crouchingOffset;
        speed = 4f;
    }

    public void StandAttempt()
    {
        bool notblocked = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        if (notblocked && !tookDamage)
        {
            Debug.Log("uncrouched");
            playerCollider.size = new Vector2(playerCollider.size.x, standingHeight);
            playerCollider.offset = standingOffset;
            speed = 5f;
        }
    }

    public void DamageTaken(Collision2D other) // This is part one of damage recoil where the player gets launched backwards. When there is a ceiling 
    {
        Debug.Log("Took Damage");
        tookDamage = true; 
        isGrounded = false;
        controls.Player.Disable();
        playerCollider.size = new Vector2(playerCollider.size.x, crouchHeight); // Did this mainly for when you take damage in a small space.
        playerCollider.offset = crouchingOffset;
        rb.gravityScale = recoilGravity; // Might need adjustment.
        if (headCheck.position.x >= other.transform.position.x) 
        {
            rb.linearVelocity = new Vector2(recoilHorizontalStrength, recoilVerticalStrength);
        }
        else
        {
            rb.linearVelocity = new Vector2(recoilHorizontalStrength * -1, recoilVerticalStrength);
        }
    }

    public void DamageRecovery() // Part 2 of damage recoil, is called when the player hits the ground again.
    {
        if (tookDamage) 
        {
            tookDamage = false;
            controls.Player.Enable();
            rb.gravityScale = standardGravity; 
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
            DamageRecovery();
        }

        if (other.gameObject.CompareTag("Lose Treasure"))
        {
            DamageTaken(other);
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
