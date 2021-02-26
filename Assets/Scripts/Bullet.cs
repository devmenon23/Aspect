using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float flightDuration = 2f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        flightDuration -= Time.deltaTime;
        if (flightDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
