using UnityEngine;

public class Slime : Enitity
{
    private void Start()
    {
        entityHealth = 5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage();
            entityHealth--;
            Debug.Log("slime: " + entityHealth);
        }

        if (entityHealth <= 0)
            EntityDie();
    }
}