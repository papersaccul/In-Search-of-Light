using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOfLight : Entity
{
    private CircleCollider2D triggerCollider;

    private Player player;


    void Start()
    {
        player = Player.Instance;
        triggerCollider = gameObject.GetComponentInChildren<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.playerHealth += 0.5f;
            HealthBar.Instance.UpdateHealthBar(player.playerHealth);
            EntityDie();
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < 30f)
                transform.position += direction.normalized * Time.deltaTime * 50f;
        }
    }
}