using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    VisualEffect vfx;
    void Start()
    {
        vfx = gameObject.GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (vfx.aliveParticleCount == 0)
        {
            Destroy(gameObject, 0.5f);
        }
    }
}
