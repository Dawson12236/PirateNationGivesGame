using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    enum Movement_State { Idle, Walk, Jump, Fall, Crouch, Damage};
    Movement_State current_state;
    Movement_State previous_state;
    public InputActionAsset InputActions;
    private InputAction jump_action;
    private InputAction move_action;
    private InputAction crouch_action;

    public GameObject manager;
    
    private AudioSource peeDeeAudio;
    public AudioClip damageTaken;
    private Rigidbody2D rb;
    public Transform headCheck;
    public CapsuleCollider2D playerCollider;
    public LayerMask groundLayer;

    public Vector2 standingSize;
    public Vector2 crouchingSize;
    public float crouchHeight, standingHeight;
    public Vector2 standingOffset;
    public Vector2 crouchingOffset;
    public float headCheckRadius = 0.2f;
    public float walk_speed = 5f;
    public float w_acceleration = 50f;
    public float crouch_speed = 2f;
    public float c_acceleration = 35f;
    public float jump_strength = 5f;
    private float friction_coefficient = 1f;
    public float recoilHorizontalStrength = 7f;
    public float recoilVerticalStrength = 7f;
    public float standardGravity = 3f;
    public float recoilGravity = 5f;
    bool isGrounded;
    bool tookDamage = false;
    bool playerTookDamageFromRight = true;
    private Vector2 movingInput;

    private void Awake()
    {
        move_action = InputSystem.actions.FindAction("Move");
        jump_action = InputSystem.actions.FindAction("Jump");
        crouch_action = InputSystem.actions.FindAction("crouch");
        peeDeeAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    void Update()
    {
        movingInput = move_action.ReadValue<Vector2>(); // Assigns movingInput to the move_action's vector its retriving from directional inputs (Joysticks, WASD, or Arrow Keys)

        //UpdateGroundCheck();

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
            case Movement_State.Damage:
                UpdateDamageState();
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
                Debug.Log($"Jump Force Applied: {jump_strength}, Mass: {rb.mass}, Resulting Velocity: {rb.linearVelocity.y}");
                break;
            case Movement_State.Fall:
                print("Entered Fall State");
                break;
            case Movement_State.Crouch:
                print("Entered Crouch State");
                playerCollider.size = new Vector2(playerCollider.size.x, crouchHeight);
                playerCollider.offset = new Vector2(0, -0.5f);
                break;
            case Movement_State.Damage:
                print("Entered Damage State");
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
            case Movement_State.Damage:
                print("Exited Crouch State");
                break;
        }
    }

    void UpdateIdleState()
    {
        //print("Idling");

        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, caluculate_fricition() * Time.deltaTime);
        if (tookDamage)
        {
            EnterState(Movement_State.Damage);
        }
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            EnterState(Movement_State.Fall);
        }
        if (movingInput != Vector2.zero)
        {
            EnterState(Movement_State.Walk);
        }
        if (jump_action.WasPressedThisFrame() && isGrounded )
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
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, movingInput.x * walk_speed, w_acceleration * Time.deltaTime);
        if (isGrounded)
        {
            EnterState(Movement_State.Idle);
        }
        if (tookDamage)
        {
            EnterState(Movement_State.Damage);
        }
    }
    void UpdateWalkState()
    {
        print("Walking");
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, movingInput.x * walk_speed, w_acceleration * Time.deltaTime);
        if (tookDamage)
        {
            EnterState(Movement_State.Damage);
        }
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
        //print("Jumping");
        if (tookDamage)
        {
            EnterState(Movement_State.Damage);
        }
        if (rb.linearVelocity.y < 0)
        {
            EnterState(Movement_State.Fall);
        }
    }
    void UpdateCrouchState()
    {
        print("Crouching");
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, movingInput.x * crouch_speed, c_acceleration * Time.deltaTime);
        if (tookDamage)
        {
            EnterState(Movement_State.Damage);
        }
        if (!crouch_action.IsPressed())
        {
            StandAttempt();
        }

    }
    void UpdateDamageState()
    {
        print("Taking Damage");
        if (isGrounded)
        {
            EnterState(Movement_State.Idle);
        }
    }
    public void StandAttempt()
    {
        bool notblocked = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
        if (notblocked && !tookDamage)
        {
            playerCollider.size = new Vector2(playerCollider.size.x, standingHeight);
            playerCollider.offset = standingOffset;
            EnterState(Movement_State.Idle);
        }
    }

    void DamageTaken(Collision2D other) // This is part one of damage recoil where the player gets launched backwards. I wasn't able to implement it into the player state machine yet.
    {
        Debug.Log("Took Damage");
        tookDamage = true; 
        isGrounded = false;
        
        peeDeeAudio.PlayOneShot(damageTaken);
        rb.gravityScale = recoilGravity; // Might need adjustment.
        if (headCheck.position.x >= other.transform.position.x) 
        {
            rb.linearVelocity = new Vector2(recoilHorizontalStrength, recoilVerticalStrength);
            playerTookDamageFromRight = true;
        }
        else
        {
            rb.linearVelocity = new Vector2(recoilHorizontalStrength * -1, recoilVerticalStrength);
            playerTookDamageFromRight = false;
        }
        manager.GetComponent<PlayerTreasure>().RemoveAndScatterCoins(playerTookDamageFromRight);
    }

    void DamageRecovery() // Part 2 of damage recoil, is called when the player hits the ground again.
    {
        if (tookDamage) 
        {
            tookDamage = false;
            
            rb.gravityScale = standardGravity;
            StandAttempt();
        }
    }
    float caluculate_fricition()
    {
        var normal_force = rb.mass * (Physics2D.gravity.y);
        var fricition_force = normal_force * friction_coefficient * -1;
        return fricition_force;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        /*if (other.gameObject.CompareTag("Ground")) 
        {
            Debug.Log("Touching ground");
            isGrounded = true;
            blocks++;
            
            DamageRecovery();
        } */

        if (other.gameObject.CompareTag("Lose Treasure"))
        {
            DamageTaken(other);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Gain Treasure" && !tookDamage)
        {
            TreasureItem treasure = collision.gameObject.GetComponent<TreasureItem>();
            manager.GetComponent<PlayerTreasure>().AddCoins(treasure.treasureAmount);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Vector3 normal = other.GetContact(0).normal;
        print(normal);
        if (normal.y > 0.8f ) // can't go up angles no more than 36 degrees
        {
            isGrounded = true;
            DamageRecovery();
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