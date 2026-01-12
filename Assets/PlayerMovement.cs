using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{

    enum Movement_State {Idle, Walk, Jump, Fall, Crouch};
    Movement_State current_state;
    Movement_State previous_state;

    public InputActionAsset InputActions;

    private InputAction jump_action;
    private InputAction move_action;
    private InputAction crouch_action;

    private Rigidbody2D rb;
    private Vector2 movingInput;
    public LayerMask groundLayer;


    public Transform headCheck;
    public float headCheckRadius = 0.2f;

    public CapsuleCollider2D playerCollider;
    public Vector2 standingSize;
    public Vector2 crouchingSize;
    public float crouchHeight, standingHeight;

    public float walk_speed = 5f;
    public float crouch_speed = 2f;
    private float jump = 10f;
    bool isGrounded;
    private float blocks = 0f;
    private void Awake()
    {
        move_action = InputSystem.actions.FindAction("Move");
        jump_action = InputSystem.actions.FindAction("Jump");
        crouch_action = InputSystem.actions.FindAction("crouch");
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    void Update()
    {
        movingInput = move_action.ReadValue<Vector2>();
        switch (current_state)
        {
            case Movement_State.Idle:
                UpdateIdleState();
                break;
            case Movement_State.Walk:
                UpdateWalkState(); 
                break;
            case Movement_State.Jump:
                UpdateJumpState();
                break;
            case Movement_State.Fall:
                UpdateFallState();
                break;
            case Movement_State.Crouch:
                UpdateCrouchState();
                break;
        }
    }

    void EnterState(Movement_State new_state)
    {
        ExitState();
        current_state = new_state;
        switch (current_state)
        {
            case Movement_State.Idle:
                print("Entered Idle State");
                break;
            case Movement_State.Walk:
                print("Entered Walk State");
                break;
            case Movement_State.Jump:
                print("Entered Jump State");
                rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
                break;
            case Movement_State.Fall:
                print("Entered Fall State");
                break;
            case Movement_State.Crouch:
                print("Entered Crouch State");
                playerCollider.size = new Vector2(playerCollider.size.x, crouchHeight);
                playerCollider.offset = new Vector2(0, -0.5f);
                break;
        }
    }

    void ExitState()
    {
        previous_state = current_state;
        switch (current_state)
        {
            case Movement_State.Idle:
                print("Exited Idle State");
                break;
            case Movement_State.Walk:
                print("Exited Walk State");
                break;
            case Movement_State.Jump:
                print("Exited Jump State");
                break;
            case Movement_State.Fall:
                print("Exited Fall State");
                break;
            case Movement_State.Crouch:
                print("Exited Crouch State");
                break;
        }

    }

    void UpdateIdleState()
    {
        print("Idling");
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            EnterState(Movement_State.Fall);
        }
        if (movingInput != Vector2.zero)
        {
            EnterState(Movement_State.Walk);
        }
        if (jump_action.WasPressedThisFrame())
        {
            EnterState(Movement_State.Jump);
        }
        if (crouch_action.WasPressedThisFrame())
        {
            EnterState(Movement_State.Crouch);
        }
    }

    void UpdateFallState()
    {
        print("Falling");
        if (isGrounded)
        {
            EnterState(Movement_State.Idle);
        }
    }

    void UpdateWalkState()
    {
        rb.linearVelocity = new Vector2(movingInput.x * walk_speed, rb.linearVelocity.y);
        print("Walking");
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            EnterState(Movement_State.Fall);
        }
        if (movingInput == Vector2.zero)
        {
            EnterState(Movement_State.Idle);
        }
        if (jump_action.WasPressedThisFrame())
        {
            EnterState(Movement_State.Jump);
        }
        if (crouch_action.WasPressedThisFrame())
        {
            EnterState(Movement_State.Crouch);
        }
    }

    void UpdateJumpState()
    {
        print("Jumping");
        if (rb.linearVelocity.y < 0)
        {
            EnterState(Movement_State.Fall);
        }
    }

    void UpdateCrouchState()
    {
        print("Crouching");
        rb.linearVelocity = new Vector2(movingInput.x * crouch_speed, rb.linearVelocity.y);
        if (!crouch_action.IsPressed())
        {
            StandAttempt();
        }

    }

    public void StandAttempt()
    {
        bool notblocked = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        if (notblocked)
        {
            playerCollider.size = new Vector2(playerCollider.size.x, standingHeight);
            playerCollider.offset = new Vector2(0, 0);
            EnterState(Movement_State.Idle);
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
