using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class Slime : Enitity
{
    // A little later, I will transfer half of the methods from the Slime class to the Entity class when I deal with other monsters

    [SerializeField] int attackDamage = 2;
    [SerializeField] private float slimeSpeed = 25f;

    private bool slimeGetDamage = false;
    private Vector3 slimeDirection;
    private SpriteRenderer ObjSprite;
    public Transform ObjAttackPosition;
    public LayerMask player;
    private bool isPlayerInSight = false;



    private void Awake()
    {
        entityHealth = 6f;

        slimeDirection = transform.right;
        ObjSprite = this.GetComponentInChildren<SpriteRenderer>();
    }

    // Will be called by animator
    private void SlimeMove()
    {
        bool slimeDie = GetComponent<Animator>().GetBool("Die");

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection, 9f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.gameObject != Player.Instance.gameObject)
                slimeDirection *= -1f;

            else if (hit.collider.gameObject == Player.Instance.gameObject)
            {
                isPlayerInSight = true;
                if (!slimeGetDamage && !slimeDie)
                    GetComponent<Animator>().Play("Attack");
            } 
            else isPlayerInSight = false;
        }

        if (!slimeGetDamage && !isPlayerInSight && !slimeDie)
            GetComponent<Rigidbody2D>().AddForce(slimeDirection.normalized * slimeSpeed, ForceMode2D.Impulse);

        ObjSprite.flipX = slimeDirection.x > 0f;
    }

    // Will be called by animator
    private void onAttack()
    {
        if (isPlayerInSight)
        {
            ObjAttackPosition.position = new Vector3(this.GetComponent<Rigidbody2D>().position.x, ObjAttackPosition.position.y, 0f);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(ObjAttackPosition.position, 10f, player);


            foreach (Collider2D collider in colliders)
                collider.GetComponent<Player>().PlayerGetDamage(attackDamage, GetComponent<Rigidbody2D>().position);
        }
    }


    
    private void OnDrawGizmosSelected()
    {
        // Debug Wall Detector Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection * 9f);

        // Debug Attack Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ObjAttackPosition.position, 10f);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage(attackDamage, GetComponent<Rigidbody2D>().position);
        }
    }

    // Will be called by animator
    private void ToggleSlimeGetDamage()
    {
        slimeGetDamage = false;
    }

}