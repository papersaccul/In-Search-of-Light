using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Slime : Enitity
{
    private Animator animator;

    private void Start()
    {
        entityHealth = 5f;
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage();
        }
    }

}