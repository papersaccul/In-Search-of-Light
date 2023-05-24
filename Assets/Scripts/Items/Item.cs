using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected void PickUp()
    {
        Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), 1f);
        GetComponent<Rigidbody2D>().AddForce(dropDirection * 100f);
    }
}
