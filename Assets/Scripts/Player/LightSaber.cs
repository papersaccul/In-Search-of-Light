using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSaber : MonoBehaviour
{
    public Light2D lightSaber;
    public Light2D lightSword;

    private void Start()
    {
        lightSaber = GetComponentInChildren<Light2D>();

        lightSaber.intensity = 0f;
        DOTween.To(() => lightSaber.intensity, x => lightSaber.intensity = x, 36f, .3f);

        StartCoroutine(Lifetime());
    }

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

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(.5f);

        DOTween.To(() => lightSaber.intensity, x => lightSaber.intensity = x, 0f, .5f);

        yield return new WaitForSeconds(.5f);

        Destroy(this.gameObject);
    }

}