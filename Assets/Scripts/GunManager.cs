using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject bulletPrefab;

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
        print("Shot");
        GameObject bulletObject = Instantiate(bulletPrefab, playerCamera.transform.position + playerCamera.transform.forward, Quaternion.identity);
        bulletObject.transform.forward = playerCamera.transform.forward;
    }
}
