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
    }

    public void UpdateHealthBar(int currentHealth)
    {
        Fill.color = gradient.Evaluate(healthbarSlider.normalizedValue);
        healthbarSlider.DOValue(currentHealth, smoothSliderDuration);
    }
}