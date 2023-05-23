using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;

    private bool isCollision = false;
    private bool isEquipAnim = false;
    private float distance;
    private Player player;
    private Vector3 direction;
    private SpriteRenderer weaponSprite;
    private string weaponPath;


    private void Start()
    {
        isEquipAnim = false;

        player = Player.Instance;
        weaponSprite = GetComponentInChildren<SpriteRenderer>();
        weaponPath = $"Weapons/{weaponType}";

        Sprite sprite = Resources.Load<Sprite>(weaponPath);
        weaponSprite.sprite = sprite;
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
    }

    private void EquipWeapon()
    {
        isEquipAnim = true;
        Debug.Log("anim");
        transform.DOMove(player.transform.position + new Vector3(0f, 8f), .5f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(OnEquipWeapon);
    }

    private void OnEquipWeapon()
    {
        Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), 1f);

        WeaponType prevWeaponType = weaponType;
        weaponType = (WeaponType)player.MainHand;
        player.MainHand = (MainHand)prevWeaponType;
        GetComponent<Rigidbody2D>().AddForce(dropDirection * 100f);

        Start();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isCollision = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isCollision = false;
    }
}
 
public enum WeaponType
{
    none,
    defaultSword,
    spear,
    bow
}