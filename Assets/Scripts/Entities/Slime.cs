using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class Slime : Enitity
{
    private float slimeSpeed = 20f;
    private bool slimeGetDamage = false;
    private Vector3 slimeDirection;
    private SpriteRenderer ObjSprite;

    private void Awake()
    {
        entityHealth = 6f;

        slimeDirection = transform.right;
        ObjSprite = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        SlimeMove();
    }


    private void SlimeMove()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection, 10f);

        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.gameObject != Player.Instance.gameObject)
                slimeDirection *= -1f;

        if (!slimeGetDamage)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + slimeDirection, Time.deltaTime * slimeSpeed);

        ObjSprite.flipX = slimeDirection.x > 0f;
    }

    // Debug Wall Detector Range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection * 10f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage();
        }
    }

    private void ToggleSlimeGetDamage()
    {
        slimeGetDamage = false;
    }

}