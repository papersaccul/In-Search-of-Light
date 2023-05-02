using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public static HealthBar Instance { get; set; }

    public Slider healthbarSlider;
    public Gradient gradient;
    public Image Fill;
    public Slider lightSlider;
    private float smoothSliderDuration = 0.3f;

    private void Awake()
    {
        Instance = this;

        healthbarSlider = GetComponent<Slider>();

        Fill.color = gradient.Evaluate(1f);
        healthbarSlider.minValue = 0;
        lightSlider.maxValue = 10;
        healthbarSlider.maxValue = 10;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        Fill.color = gradient.Evaluate(healthbarSlider.normalizedValue);
        healthbarSlider.DOValue(currentHealth, smoothSliderDuration);

        if (healthbarSlider.value >= 10 && currentHealth > 10)
        {
            lightSlider.gameObject.SetActive(true);
            float lightValue = currentHealth - 10;
            lightSlider.DOValue(lightValue, smoothSliderDuration);
        }
        else
            lightSlider.gameObject.SetActive(false);
    }
}