using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    protected float entityHealth;
    protected bool isGrounded = false;

    public virtual void EntityGetDamage()
    {
        animator = GetComponent<Animator>();
        entityHealth -= Player.Instance.playerDamage;

        if (entityHealth <= 0)
            animator.SetTrigger("Die");

        else if (isGrounded) animator.Play("Hurt");
    }

    public virtual void IsGroundChecker()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Vector2 currentPosition = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 16f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, direction, distance);

        foreach (RaycastHit2D hit in hits)
            isGrounded = (hit.collider != null && hit.collider.gameObject != gameObject);

        if (!isGrounded && !(entityHealth <= 0) && !animator.GetBool("Hurt") )
        {
            if (rb.velocity.y < 0)
                animator.SetTrigger("Fall");
            else
                animator.SetTrigger("Jump");

            animator.SetBool("isGrounded", false);
        }
        else
        {
            animator.ResetTrigger("Fall");
            animator.ResetTrigger("Jump");
            animator.SetBool("isGrounded", true);
        }
    }
   
    public virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }
}
