using UnityEngine;

public class Slime : Enitity
{
    [SerializeField] private int SlimeHealht = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.PlayerGetDamage();
            SlimeHealht--;
            Debug.Log("slime: " + SlimeHealht);
        }

        if (SlimeHealht <= 0)
            EntityDie();
    }
}