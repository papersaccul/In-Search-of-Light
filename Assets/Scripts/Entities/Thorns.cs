using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorns : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
       /* if (Player.Instance != null && collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage(2, GetComponent<Rigidbody2D>().position, this);
        }*/
    }
}
