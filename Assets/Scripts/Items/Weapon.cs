using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;

    // Start is called before the first frame update
    void Start()
    {
        switch (weaponType)
        {
            case WeaponType.defaultSword:

                break;
            case WeaponType.spear:

                break;
            case WeaponType.bow:

                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
    }

    void Update()
    {
        
    }
}

public enum WeaponType
{
    defaultSword,
    spear,
    bow
}