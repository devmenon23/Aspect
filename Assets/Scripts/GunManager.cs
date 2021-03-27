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

    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] VisualEffect MuzzleFlashVFX;
    [SerializeField] VisualEffect BulletImpactVFX;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {

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
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Instantiate(BulletImpactVFX, hit.point, Quaternion.LookRotation(hit.normal));
            print("hit");
        }
        MuzzleFlashVFX.Play();
        currentAmmo--;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

}
