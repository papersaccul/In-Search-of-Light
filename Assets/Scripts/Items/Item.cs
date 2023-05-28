using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Player player;
    protected bool isCollision;
    private Color hightlightOutlineColor = new Color32(255, 255, 40, 255);

    protected void Start(SpriteRenderer spriteRender)
    {
        player = Player.Instance;
        spriteRenderer = spriteRender;
    }

    protected void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        float brightness = Mathf.Lerp(4, 1, Mathf.Clamp01(distance / 30f));
        if (!isCollision)
            spriteRenderer.material.color = Color.Lerp(hightlightOutlineColor, Color.white, Mathf.Clamp01(distance / 45f)) * brightness;
    }

    protected void PickUp(Shader outline)
    {
        Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), 1f);
        GetComponent<Rigidbody2D>().AddForce(dropDirection * 100f);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        isCollision = true;
        spriteRenderer.material.color = hightlightOutlineColor * 5f;
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        isCollision = false;
    }
}
