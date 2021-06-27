using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using TMPro;

public class GunManager : MonoBehaviour
{
    PhotonView PV;

    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 15f;
    [SerializeField] bool automatic = false;
    [SerializeField] float nextTimeToFire = 0f;

    [SerializeField] int maxAmmo = 20;
    private int currentAmmo;
    [SerializeField] float reloadTime = 1f;
    private bool isReloading = false;
    [SerializeField] TMP_Text ammoText;


    [SerializeField] float verticalIntensity;
    [SerializeField] float horizontalIntensity;
    [SerializeField] float smooth;
    private Quaternion originRotation;

    [SerializeField] PlayerController player;
    [SerializeField] Transform playerCameraHolder;
    [SerializeField] VisualEffect MuzzleFlashVFX;
    [SerializeField] GameObject ProjectileVFX;
    [SerializeField] VisualEffect BulletImpactVFX;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        player = GetComponentInParent<PlayerController>();
    }

    void Start()
    {
        InitializeAmmo();
        originRotation = transform.localRotation;
    }

    void OnEnable()
    {
        isReloading = false;    
    }

    void Update()
    {
        if (!PV.IsMine) { return; }

        // Shooting
        if (isReloading) { return; }

        if ((currentAmmo < maxAmmo && Input.GetKeyDown(KeyCode.R)) || currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        // Swaying
        UpdateSway();
    }

    void Shoot()
    {
        RaycastHit hit;
        currentAmmo--;
        RefreshAmmoUI();
        if (Physics.Raycast(playerCameraHolder.transform.position, playerCameraHolder.transform.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Player"))
            {
                PlayerController victim = hit.collider.gameObject.GetComponent<PlayerController>();
                if (victim.healthUIAnimator.GetFloat("Health_current") - damage <= 0)
                {
                    player.GetKill();
                } 
                victim.TakeDamage(damage);
            }
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPos, Vector3 hitNor)
    {
        MuzzleFlashVFX.Play();
        GameObject projectile = Instantiate(ProjectileVFX, MuzzleFlashVFX.transform.position, Quaternion.identity);
        projectile.transform.forward = playerCameraHolder.forward;
        Instantiate(BulletImpactVFX, hitPos, Quaternion.LookRotation(hitNor));
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        RefreshAmmoUI();
        isReloading = false;
    }

    void UpdateSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion xAdj = Quaternion.AngleAxis(-horizontalIntensity * mouseX, Vector3.up);
        Quaternion yAdj = Quaternion.AngleAxis(verticalIntensity * mouseY, Vector3.right);

        Quaternion targetRotation = originRotation * xAdj * yAdj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    void InitializeAmmo()
    {
        currentAmmo = maxAmmo;
        RefreshAmmoUI();
    }

    void RefreshAmmoUI()
    {
        ammoText.text = currentAmmo.ToString("00") + "/" + maxAmmo.ToString("00");
    }
}
