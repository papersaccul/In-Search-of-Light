using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D ObjRigidbody;

    protected float entityHealth;
    protected bool isGrounded = false;

    public virtual void EntityGetDamage()
    {
        animator = GetComponent<Animator>();
        entityHealth -= Player.Instance.playerDamage;

        if (entityHealth <= 0)

            if (isGrounded)
                animator.SetBool("Die", true);
            else animator.SetBool("AirDie", true);

        else animator.SetBool("Hurt", true);
    }

    public virtual void ToggleGetDamage()
    {
        GetComponent<Animator>().SetBool("Hurt", false);
    }

    protected virtual void IsGroundChecker()
    {
        animator = GetComponent<Animator>();
        ObjRigidbody = GetComponent<Rigidbody2D>();

        Vector2 currentPosition = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 16f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, direction, distance);

        foreach (RaycastHit2D hit in hits)
            isGrounded = (hit.collider != null && hit.collider.gameObject != gameObject);

        if (!isGrounded && !(entityHealth <= 0) )
        {
            if (ObjRigidbody.velocity.y < 0)
            {
                animator.SetBool("Fall", true);
                animator.SetBool("Jump", false);
            }
            else
            {
                animator.SetBool("Fall", false);
                animator.SetBool("Jump", true);
            }

            animator.SetBool("isGrounded", false);
        }
        else
        {
            animator.SetBool("Fall", false);
            animator.SetBool("Jump", false);
            animator.SetBool("isGrounded", true);
        }
    }

    protected virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }

}
