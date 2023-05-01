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
        healthbarSlider.value = currentHealth;

        Fill.color = gradient.Evaluate(healthbarSlider.normalizedValue);
    }



}
