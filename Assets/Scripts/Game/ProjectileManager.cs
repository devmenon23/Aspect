using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] float speed = 100f;

    void Update() 
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        Destroy(gameObject, 2f);
    }
}
