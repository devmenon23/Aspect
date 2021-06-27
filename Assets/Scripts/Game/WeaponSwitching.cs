using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] int selectedWeapon = 0;
    [SerializeField] PhotonView PV;

    [SerializeField] Image primary;
    [SerializeField] Image secondary;
    [SerializeField] float bright = 0.6f;
    [SerializeField] float dim = 0.2f;

    void Start()
    {
        primary = primary.GetComponent<Image>();
        secondary = secondary.GetComponent<Image>();
        SelectWeapon();
    }

    void Update()
    {
        if (!PV.IsMine) { return; }

        // Switching weapon
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }

        // Changing active weapon UI
        if (selectedWeapon == 0)
        {
            var dimColor = secondary.color;
            var brightColor = primary.color;
            dimColor.a = dim;
            brightColor.a = bright;
            secondary.color = dimColor;
            primary.color = brightColor;
        }

        else if (selectedWeapon == 1)
        {
            var dimColor = primary.color;
            var brightColor = secondary.color;
            dimColor.a = dim;
            brightColor.a = bright;
            primary.color = dimColor;
            secondary.color = brightColor;
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform) 
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
