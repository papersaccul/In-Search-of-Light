using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Enitity : MonoBehaviour
{
    protected float entityHealth;
    public virtual void EntityGetDamage()
    {
        entityHealth -= Player.Instance.playerDamage;

        if (entityHealth <= 0)
            EntityDie();
    }

    public virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }
}
