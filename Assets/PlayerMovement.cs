using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{

    enum Movement_State { Idle, Walk, Jump, Fall, Crouch};
    Movement_State current_state;
    Movement_State previous_state;

    public InputActionAsset InputActions;
    private InputAction jump_action;
    private InputAction move_action;
    private InputAction crouch_action;

    private Rigidbody2D rb;
    public Transform headCheck;
    public CapsuleCollider2D playerCollider;
    public LayerMask groundLayer;


    public Vector2 standingSize;
    public Vector2 crouchingSize;
    public float crouchHeight, standingHeight;
    public float headCheckRadius = 0.2f;

    public float walk_speed = 5f;
    public float w_acceleration = 50f;
    public float crouch_speed = 2f;
    public float c_acceleration = 35f;
    public float jump_strength = 125f;
    private float friction_coefficient = 1f;

    private Vector2 movingInput;

    bool isGrounded;
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
        movingInput = move_action.ReadValue<Vector2>(); // Assigns movingInput to the move_action's vector its retriving from directional inputs (Joysticks, WASD, or Arrow Keys)

        switch (current_state) // This switch statement switches to the corresponding update function depending on the current state
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
        switch (current_state) // This switch statement executes the corresponding lines once entering a state
        {
            case Movement_State.Idle:
                print("Entered Idle State");
                break;
            case Movement_State.Walk:
                print("Entered Walk State");
                break;
            case Movement_State.Jump:
                print("Entered Jump State");
                rb.AddForce(Vector2.up * jump_strength, ForceMode2D.Impulse);
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
        switch (current_state) // This switch statement executes the corresponding lines once exiting a state
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

        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, caluculate_fricition() * Time.deltaTime);

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
        print("Walking");
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, movingInput.x * walk_speed, w_acceleration * Time.deltaTime);
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
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, movingInput.x * crouch_speed, c_acceleration * Time.deltaTime);
        if (!crouch_action.IsPressed())
        {
            StandAttempt();
        }

    }

    void StandAttempt()
    {
        bool notblocked = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        if (notblocked)
        {
            playerCollider.size = new Vector2(playerCollider.size.x, standingHeight);
            playerCollider.offset = new Vector2(0, 0);
            EnterState(Movement_State.Idle);
        }
    }
    float caluculate_fricition()
    {
        var normal_force = rb.mass * (Physics2D.gravity.y);
        var fricition_force = normal_force * friction_coefficient * -1;
        return fricition_force;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Vector3 normal = other.GetContact(0).normal;
        print(normal);
        if (normal.y > 0.8f ) // can't go up angles no more than 36 degrees
        {
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