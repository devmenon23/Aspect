using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;

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

    [SerializeField] float verticalIntensity;
    [SerializeField] float horizontalIntensity;
    [SerializeField] float smooth;
    private Quaternion originRotation;

    [SerializeField] Transform playerCameraHolder;
    [SerializeField] VisualEffect MuzzleFlashVFX;
    [SerializeField] VisualEffect BulletImpactVFX;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
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
        if (Physics.Raycast(playerCameraHolder.transform.position, playerCameraHolder.transform.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            }
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPos, Vector3 hitNor)
    {
        MuzzleFlashVFX.Play();
        Instantiate(BulletImpactVFX, hitPos, Quaternion.LookRotation(hitNor));
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
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
}
