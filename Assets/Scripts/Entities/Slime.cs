using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class Slime : Enitity
{
    // A little later, I will transfer half of the methods from the Slime class to the Entity class when I deal with other monsters

    [SerializeField] int attackDamage = 2;
    [SerializeField] private float slimeSpeed = 20f;

    private bool slimeGetDamage = false;
    private Vector3 slimeDirection;
    private SpriteRenderer ObjSprite;
    public Transform ObjAttackPosition;
    public LayerMask player;
    private bool playerInSight = false;



    private void Awake()
    {
        entityHealth = 6f;

        slimeDirection = transform.right;
        ObjSprite = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // SlimeMove();
    }


    private void SlimeMove()
    {
        bool slimeDie = GetComponent<Animator>().GetBool("Die");

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection, 8f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.gameObject != Player.Instance.gameObject)
                slimeDirection *= -1f;

            else if (hit.collider.gameObject == Player.Instance.gameObject)
            {
                playerInSight = true;
                if (!slimeGetDamage && !slimeDie)
                    GetComponent<Animator>().Play("Attack");
            } 
            else playerInSight = false;
        }

        if (!slimeGetDamage && !playerInSight && !slimeDie)
            GetComponent<Rigidbody2D>().AddForce(slimeDirection.normalized * slimeSpeed, ForceMode2D.Impulse);

        ObjSprite.flipX = slimeDirection.x > 0f;
    }

    // Will be called by animator
    public void onAttack()
    {   
        ObjAttackPosition.position = new Vector3(this.GetComponent<Rigidbody2D>().position.x, ObjAttackPosition.position.y, 0f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(ObjAttackPosition.position, 10f, player);
        
        if (playerInSight)
            foreach (Collider2D collider in colliders)
                collider.GetComponent<Player>().PlayerGetDamage(attackDamage);
    }


    
    private void OnDrawGizmosSelected()
    {
        // Debug Wall Detector Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -10f, 0f), slimeDirection * 8f);

        // Debug Attack Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ObjAttackPosition.position, 13f);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage(attackDamage);
        }
    }

    // Will be called by animator
    private void ToggleSlimeGetDamage()
    {
        slimeGetDamage = false;
    }

}