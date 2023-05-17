using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public partial class Player : MonoBehaviour
{
    private MainHand CurrentWeapon;

    private void ChangeWeapon()
    {
        MainHand[] weapons = (MainHand[])Enum.GetValues(typeof(MainHand));
        int currentIndex = (int)CurrentWeapon;
        currentIndex = (currentIndex + 1) % weapons.Length;
        CurrentWeapon = weapons[currentIndex];
        MainHand = CurrentWeapon;
    }

}
