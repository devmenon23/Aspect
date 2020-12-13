using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController playerController;
    [SerializeField] float speed = 10f;
    [SerializeField] Vector3 jumpForce = new Vector3(0f, 10f, 0f);
    [SerializeField] bool playerOnGround = true;
    Rigidbody rb;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 moveDistance = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerOnGround = playerController.isGrounded;

        playerController.Move(moveDistance * speed * Time.deltaTime);

        if (moveDistance != Vector3.zero) // if player is moving
        {
            gameObject.transform.forward = moveDistance; // change forward of player
        }

        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(jumpForce);
        }
    }
}
