using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpSpeed = 3.5f;
    [SerializeField] private float doubleJumpMultiplier = 0.5f;
    [SerializeField] public float mouseSensitivity = 3;

    private CharacterController controller;
    private float directionY;
    private bool canDoubleJump = false;

    private Camera mainCamera;
    private float mouseX;
    private float mouseY;
    public float maxUpAngle = 80;
    public float maxDownAngle = -80;
    private float rotX = 0.0f, rotY = 0.0f;
    private float rotZ = 0.0f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

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

        controller.Move(direction.z * Vector3.forward * moveSpeed * Time.deltaTime);
        controller.Move(direction.x * Vector3.right * moveSpeed * Time.deltaTime);

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, maxDownAngle, maxUpAngle);
        rotY += mouseX;

        mainCamera.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        transform.Rotate(Vector3.up * mouseX);
    }
}