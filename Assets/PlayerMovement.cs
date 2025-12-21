using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour 
{
    private InputSystem_Actions controls;
    private Vector2 movingInput;
    private Rigidbody2D rb;
    private readonly float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Enable(); // makes sure is enabled 
    }

    void Start()
    {
        controls.Player.Move.performed += OnMove;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = movingInput * speed;
    }

    private void FixedUpdate()
    {
        // moving cahracter based on where they currently are, interpolates to look smoother
        rb.MovePosition(rb.position + movingInput * speed * Time.fixedDeltaTime);

    }

    //signature used to read 3d
    public void OnMove(InputAction.CallbackContext context)
    {
        movingInput = context.ReadValue<Vector2>();
    }
}
