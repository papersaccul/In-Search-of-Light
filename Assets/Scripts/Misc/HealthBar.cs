using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthbarSlider;
    private Quaternion startRotation;
    public Gradient gradient;
    public Image Fill;
    private float smoothSliderDuration = 0.1f;

    private void Awake()
    {
        startRotation = transform.rotation;
        healthbarSlider = GetComponent<Slider>();

        Fill.color = gradient.Evaluate(1f);
    }

    private void LateUpdate()
    {
        transform.rotation = startRotation;
    }

    public void UpdateHealthBar(int maxHealth, int currentHealth)
    {
        healthbarSlider.maxValue = maxHealth;
        healthbarSlider.minValue = 0;

        StartCoroutine(UpdateHealthBarCoroutine(currentHealth));
    }

    private IEnumerator UpdateHealthBarCoroutine(int currentHealth)
    {
        float startValue = healthbarSlider.value;
        float endValue = currentHealth;

        while (healthbarSlider.value != endValue)
        {
            float newValue = Mathf.MoveTowards(healthbarSlider.value, endValue, Time.deltaTime / smoothSliderDuration);

            healthbarSlider.value = newValue;
            Fill.color = gradient.Evaluate(healthbarSlider.normalizedValue);

            yield return null;
        }
    }
}
