using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class Slime : Entity
{
    // A little later, I will transfer half of the methods from the Slime class to the Entity class when I deal with other monsters

    [SerializeField] int attackDamage = 2;
    [SerializeField] private float slimeSpeed = 25f;

    private bool slimeGetDamage = false;

    private Vector3 slimeDirection;
    private SpriteRenderer ObjSprite;
    public LayerMask player;
    public Transform attackPosition;
    

    private void Awake()
    {
        entityHealth = 6f;
        slimeDirection = transform.right;
        ObjSprite = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        IsGroundChecker();
    }

    // Will be called by animator
    protected void SlimeMove()
    {
        bool slimeDie = GetComponent<Animator>().GetBool("Die");

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -15f), slimeDirection, 9f);

        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.gameObject != Player.Instance.gameObject)
                slimeDirection *= -1f;
            
            else MeleeAttack(slimeDirection, slimeGetDamage, slimeDie);
        }

        if (!slimeGetDamage && !isPlayerInSight && !slimeDie)
            GetComponent<Rigidbody2D>().AddForce(slimeDirection.normalized * slimeSpeed, ForceMode2D.Impulse);

        ObjSprite.flipX = slimeDirection.x > 0f;
    }

    // Will be called by animator
    protected void onAttack()
    {
        if (isPlayerInSight)
        {
            attackPosition.position = new Vector3(GetComponent<Rigidbody2D>().position.x, attackPosition.position.y, 0f);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition.position, 10f, player);


            foreach (Collider2D collider in colliders)
                collider.GetComponent<Player>().PlayerGetDamage(attackDamage, GetComponent<Rigidbody2D>().position);
        }
    }


    
    private void OnDrawGizmosSelected()
    {
        // Debug Wall Detector Range
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + transform.up + transform.right * slimeDirection.x + new Vector3(0f, -15f, 0f), slimeDirection * 9f);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage(attackDamage, GetComponent<Rigidbody2D>().position);
        }
    }
}