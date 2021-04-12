using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    float moveX;
    float moveZ;
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
    [SerializeField] Transform weaponHolder;
    float originalFOV;
    Vector3 weaponHolderOrigin;
    float movementCounter;
    float idleCounter;
    Vector3 targetWeaponBobPos;

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
        weaponHolderOrigin = weaponHolder.localPosition;
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
        if (currentState == PlayerState.idle || currentState == PlayerState.crouching)
        {
            cameraAnimator.Rebind();
            cameraAnimator.SetBool("Idle", true);
            Headbob(idleCounter, 0.01f, 0.01f);
            idleCounter += Time.deltaTime;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetWeaponBobPos, Time.deltaTime * 1f);
        }

        else if (currentState == PlayerState.walking)
        {
            if (isGrounded)
            {
                cameraAnimator.SetBool("Idle", false);
                cameraAnimator.SetBool("Sprint_Headbob", false);
                cameraAnimator.SetBool("Walk_Headbob", true);
            }
            Headbob(movementCounter, 0.015f, 0.015f);
            movementCounter += Time.deltaTime * 3f;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetWeaponBobPos, Time.deltaTime * 6f);
        }

        else if (currentState == PlayerState.sprinting)
        {
            if (isGrounded)
            {
                cameraAnimator.SetBool("Idle", false);
                cameraAnimator.SetBool("Walk_Headbob", false);
                cameraAnimator.SetBool("Sprint_Headbob", true);
            }
            Headbob(movementCounter, 0.02f, 0.02f);
            movementCounter += Time.deltaTime * 5f;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetWeaponBobPos, Time.deltaTime * 10f);
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (moveX == 0 && moveZ == 0)
        {
            currentState = PlayerState.idle;
        }

        else if (speed == walkingSpeed  && (moveX > 0 || moveZ > 0))
        {
            currentState = PlayerState.walking;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);
    }

    void Sprint()
    {
        currentState = PlayerState.sprinting;
        speed = sprintingSpeed;
    }

    void Crouch()
    {
        if (moveX == 0 & moveZ  == 0)
        {
            currentState = PlayerState.crouching;
        }

        else
        {
            currentState = PlayerState.crouchWalking;
        }
        speed = crouchingSpeed;
        controller.height = Mathf.Lerp(controller.height, reducedHeight, Time.deltaTime * 5);
    }

    void Jump()
    {
        currentState = PlayerState.jumping;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Headbob(float z, float xIntensity, float yIntensity)
    {
        targetWeaponBobPos = weaponHolderOrigin + new Vector3 (Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }

    void ResetPlayer()
    {
        controller.height = Mathf.Lerp(controller.height, originalHeight, Time.deltaTime * 5);
        speed = walkingSpeed;
    }
}
  
public enum PlayerState { idle, walking, sprinting, jumping, crouching, crouchWalking }