using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Rigidbody2D rb;
    private Vector2 movingInput;

    public float speed = 5f;
    public float jump = 10f;

    public LayerMask groundMask;
    bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Enable(); // makes sure is enabled 

    }

    void Start()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
        rb = GetComponent<Rigidbody2D>(); // enables physics forces applied ot the X and Y axes
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.linearVelocity = movingInput * speed;
    }

    private void FixedUpdate()
    {
        // moving cahracter based on where they currently are, interpolates to look smoother
        if (movingInput != Vector2.zero)
        {
            rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movingInput);
        }

    }

    //signature used to read 3d
    public void OnMove(InputAction.CallbackContext context)
    {
        movingInput = context.ReadValue<Vector2>();
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
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
