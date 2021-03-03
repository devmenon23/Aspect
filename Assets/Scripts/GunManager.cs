using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunManager : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] VisualEffect MuzzleFlashVFX;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, playerCamera.transform.position + playerCamera.transform.forward, Quaternion.identity);
        bulletObject.transform.forward = playerCamera.transform.forward;
        MuzzleFlashVFX.Play();
    }
}
