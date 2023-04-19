using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Enitity : MonoBehaviour
{
    private Animator animator;
    protected float entityHealth;
    public virtual void EntityGetDamage()
    {
        animator = GetComponent<Animator>();
        entityHealth -= Player.Instance.playerDamage;

        if (entityHealth <= 0)
            animator.SetTrigger("Die");

        else animator.Play("Hurt");
    }

    public virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }
}
