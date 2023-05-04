using UnityEngine;

public class LightSaber : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Entity entity = other.GetComponent<Entity>();

            if (entity != null)
            {
                entity.EntityGetDamage(10);
            }
        }
    }

}