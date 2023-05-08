using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBar : MonoBehaviour
{
    public static StaminaBar Instance { get; set; }

    public Slider staminaSlider;
    public Gradient gradient;
    public Image Fill;
    private float smoothSliderDuration = .5f;


    private void Awake()
    {
        Instance = this;

        staminaSlider = GetComponent<Slider>();

        Fill.color = gradient.Evaluate(.1f);
        staminaSlider.minValue = 0f;
        staminaSlider.maxValue = 5f;
    }

    public void UpdateStaminaSlider(float currentStamina)
    {
        Fill.DOColor(gradient.Evaluate(staminaSlider.normalizedValue), smoothSliderDuration);
        staminaSlider.DOValue(currentStamina, smoothSliderDuration);
    }
}
