using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_1 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpSpeed = 3.5f;
    [SerializeField] private float doubleJumpMultiplier = 0.5f;
    [SerializeField] public float mouseSensitivity = 100;

    private CharacterController controller;
    private float directionY;
    private bool canDoubleJump = false;

    private Camera mainCamera;
    private float mouseX;
    private float mouseY;
    public float maxUpAngle = 80;
    public float maxDownAngle = -80;
    private float rotX = 0.0f, rotY = 0.0f, rotZ = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = (transform.forward * verticalInput) + (transform.right * horizontalInput);

        if (controller.isGrounded)
        {
            canDoubleJump = true;

            if (Input.GetButtonDown("Jump"))
            {
                directionY = jumpSpeed;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && canDoubleJump)
            {
                directionY = jumpSpeed * doubleJumpMultiplier;
                canDoubleJump = false;
            }
        }

        directionY -= gravity * Time.deltaTime;

        direction.y = directionY;

        controller.Move(direction * moveSpeed * Time.deltaTime);

     //mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
     //mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

     //rotX -= mouseY;
     //rotX = Mathf.Clamp(rotX, maxDownAngle, maxUpAngle);
     //rotY += mouseX;

     //mainCamera.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
     //transform.rotation *= Quaternion.Euler(0, mouseX, 0);
    }
}