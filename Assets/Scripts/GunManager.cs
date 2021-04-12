using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunManager : MonoBehaviour
{
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
    private Vector3 originPosition;

    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] VisualEffect MuzzleFlashVFX;
    [SerializeField] VisualEffect BulletImpactVFX;
    GameObject BulletImpactVFXGO;

    void Start()
    {
        currentAmmo = maxAmmo;
        originRotation = transform.localRotation;
        originPosition = transform.localPosition;
    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
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
        MuzzleFlashVFX.Play();
        currentAmmo--;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Instantiate(BulletImpactVFX, hit.point, Quaternion.LookRotation(hit.normal));
        }
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
