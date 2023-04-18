using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Enitity : MonoBehaviour
{
    public virtual void EntityGetDamage()
    {

    }

    public virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }
}
