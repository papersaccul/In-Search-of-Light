using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthbarSlider;
    private Quaternion startRotation;

    private void Awake()
    {
        startRotation = transform.rotation;
        healthbarSlider = GetComponent<Slider>();
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
    }



}
