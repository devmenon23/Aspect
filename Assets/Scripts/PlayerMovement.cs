using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    public PlayerState currentState; 

    [Header("Speed")]
    [SerializeField] float sprintingSpeed = 7f;
    [SerializeField] float walkingSpeed = 5f;
    [SerializeField] float crouchingSpeed = 3f;
    float speed;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    [Header("Crouch")]
    [SerializeField] float reducedHeight;
    float originalHeight;

    [Header("Headbob")]
    [SerializeField] Animator cameraAnimator;
    float originalFOV;
    float sprintFOVmodifier = 1.4f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.4f;
    bool isGrounded;

    void Start()
    {
        currentState = PlayerState.idle;
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
        if (Input.GetKey(KeyCode.LeftShift) && currentState == PlayerState.walking)
        {
            Sprint();
        }

        // Crouch
        else if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            Crouch();
        }

        // Resetting Player
        else
        {
            ResetPlayer();
        }

        // Jump
        if (Input.GetButtonDown("Jump") && currentState != PlayerState.crouching && isGrounded)
        {
            Jump();
        }

        velocity.y += gravity * Time.deltaTime; // this is for going back to the ground when you are in the air

        controller.Move(velocity * Time.deltaTime);

        // Headbob
        if (currentState == PlayerState.idle)
        {
            cameraAnimator.SetTrigger("Idle");
        }
        else if (currentState == PlayerState.walking & isGrounded)
        {
            cameraAnimator.SetTrigger("Walk_Headbob");
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x == 0 && z == 0)
        {
            currentState = PlayerState.idle;
        }

        else if (speed == walkingSpeed  && (x > 0 || z > 0))
        {
            currentState = PlayerState.walking;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void Sprint()
    {
        currentState = PlayerState.sprinting;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFOV * sprintFOVmodifier, Time.deltaTime * 8f);
        speed = sprintingSpeed;
    }

    void Crouch()
    {
        currentState = PlayerState.crouching;
        speed = crouchingSpeed;
        controller.height = Mathf.Lerp(controller.height, reducedHeight, Time.deltaTime * 5);
    }

    void Jump()
    {
        currentState = PlayerState.jumping;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ResetPlayer()
    {
        controller.height = Mathf.Lerp(controller.height, originalHeight, Time.deltaTime * 5);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFOV, Time.deltaTime * 8f);
        speed = walkingSpeed;
    }
}
  
public enum PlayerState { idle, walking, sprinting, jumping, crouching }