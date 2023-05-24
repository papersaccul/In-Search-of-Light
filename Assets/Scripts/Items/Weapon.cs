using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapons : Item
{
    [SerializeField] private WeaponType weaponType;

    private bool isEquipAnim = false;
    private float distance;
    private Player player;
    private Vector3 direction;
    private Shader outline;
    private SpriteRenderer weaponSprite;
    private string weaponPath;


    private void Start()
    {
        isEquipAnim = false;
        weaponSprite = GetComponentInChildren<SpriteRenderer>();

        player = Player.Instance;
        weaponPath = $"Weapons/{weaponType}";

        Sprite sprite = Resources.Load<Sprite>(weaponPath);
        weaponSprite.sprite = sprite;

        base.Start(weaponSprite);
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < 30f && Input.GetKey(KeyCode.E) && isCollision && !isEquipAnim)
                EquipWeapon();   
        }

        base.Update();
    }

    private void EquipWeapon()
    {
        isEquipAnim = true;
        transform.DOMove(player.transform.position + new Vector3(0f, 8f), .3f)
            .SetEase(Ease.Flash)
            .OnComplete(OnEquipWeapon);
    }

    private void OnEquipWeapon()
    {
        PickUp(outline);

        WeaponType prevWeaponType = weaponType;
        weaponType = (WeaponType)player.MainHand;
        player.MainHand = (MainHand)prevWeaponType;
        
        Start();
    }
}
 
public enum WeaponType
{
    none,
    defaultSword,
    spear,
    bow
}