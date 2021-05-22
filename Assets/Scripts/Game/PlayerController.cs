using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject camHolder;
    PhotonView PV;
    PlayerManager playerManager;
    float moveX;
    float moveZ;
    public PlayerState currentState;

    [Header("Health")]
    [SerializeField] Image healthBarImg;
    [SerializeField] GameObject ui;
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;

    [Header("KillCounter")]
    [SerializeField] TMP_Text killCounterText;
    int kills = 0;

    [Header("Look")]
    public float mouseSensitivity = 100f;
    private float verticalRotation = 0f;

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
    Vector3 weaponHolderOrigin;
    float movementCounter;
    float idleCounter;
    Vector3 targetWeaponBobPos;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.4f;
    bool isGrounded;

    [Header("Player Info UI")]
    [SerializeField] TMP_Text usernameText;
    public Animator healthUIAnimator;
    [SerializeField] Image otherHealthBarImg;
    [SerializeField] GameObject otherUI;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        healthUIAnimator.SetFloat("Health_current", currentHealth);
    }

    void Start()
    {
        if (!PV.IsMine) 
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(ui);
        }
        else
        {
            otherUI.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        currentState = PlayerState.idle;
        speed = walkingSpeed;
        originalHeight = controller.height;
        weaponHolderOrigin = weaponHolder.localPosition;
        killCounterText.text = kills.ToString();
        usernameText.text = PhotonNetwork.NickName;
    }

    void Update()
    {
        otherHealthBarImg.fillAmount = healthUIAnimator.GetFloat("Health_current") / maxHealth;
        if (!PV.IsMine) { return; }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Look
        Look();

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

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        camHolder.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
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

    void ResetPlayer()
    {
        controller.height = Mathf.Lerp(controller.height, originalHeight, Time.deltaTime * 5);
        speed = walkingSpeed;
    }

    void Headbob(float z, float xIntensity, float yIntensity)
    {
        targetWeaponBobPos = weaponHolderOrigin + new Vector3 (Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }

    public void GetKill()
    {
        kills += 1;
        killCounterText.text = kills.ToString();
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine) { return; }

        currentHealth -= damage;
        healthBarImg.fillAmount = currentHealth / maxHealth;
        healthUIAnimator.SetFloat("Health_current", currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
  
public enum PlayerState { idle, walking, sprinting, jumping, crouching, crouchWalking }