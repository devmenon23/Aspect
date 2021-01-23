using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    public PlayerState currentState; 

    [Header("Speed")]
    float speed;
    [SerializeField] float sprintingSpeed = 7f;
    [SerializeField] float walkingSpeed = 5f;
    [SerializeField] float crouchingSpeed = 3f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 3f;
    Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    [Header("Crouch")]
    float originalHeight;
    [SerializeField] float reducedHeight;
    bool isCrouching = false;

    [Header("Headbob")]
    float originalFOV;
    float sprintFOVmodifier = 1.3f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    void Start()
    {
        currentState = PlayerState.standing;
        speed = walkingSpeed;
        originalHeight = controller.height;
        originalFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Move
        Move();

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && currentState != PlayerState.crouching)
        {
            currentState = PlayerState.sprinting;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFOV * sprintFOVmodifier, Time.deltaTime * 8f);
            speed = sprintingSpeed;
        }

        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFOV, Time.deltaTime * 8f);
            if (currentState == PlayerState.crouching)
            {
                speed = crouchingSpeed;
            }
            else
            {
                speed = walkingSpeed;
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            if (isCrouching)
            {
                Jump();
                Crouch();
            }
            else
            {
                Jump();
            }
        }

        velocity.y += gravity * Time.deltaTime; // this is for going back to the ground when you are in the air

        controller.Move(velocity * Time.deltaTime);

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            Crouch();
        }

        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Stand();
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Crouch()
    {
        isCrouching = true;
        speed = crouchingSpeed;
        controller.height = reducedHeight;
    }

    void Stand()
    {
        isCrouching = false;
        speed = walkingSpeed;
        controller.height = originalHeight;
    }

}

public enum PlayerState { standing, walking, sprinting, crouching}