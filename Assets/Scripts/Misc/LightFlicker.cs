using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private bool rotateLight = true;
    [SerializeField] private bool scaleLight = true;
    [SerializeField] private bool flickerIntensity = true;

    [SerializeField] private float minIntensity = .5f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private int smoothing = 5;

    [SerializeField] private float minSize = 1f;
    [SerializeField] private float maxSize = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float sizeSpeed = 5f;


    private Light2D light2D;

    private float[] smoothQueue;
    private int currentIndex = 0;
    private float currentSum = 0f;

    private void Start()
    {
        light2D = GetComponent<Light2D>();
        smoothQueue = new float[smoothing];
    }

    private void FixedUpdate()
    {
        float newIntensity;

        if (flickerIntensity)
            newIntensity = Random.Range(minIntensity, maxIntensity);

        else
            newIntensity = light2D.intensity;

        float newSize;

        if (scaleLight)
        {
            newSize = transform.localScale.x + Random.Range(-sizeSpeed, sizeSpeed) * Time.deltaTime;
            newSize = Mathf.Clamp(newSize, minSize, maxSize);
            transform.localScale = Vector3.one * newSize;
        }
        else
        newSize = light2D.pointLightOuterRadius;

        float angle;

        if (rotateLight)
        {
            angle = transform.eulerAngles.z;
            angle += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        currentSum -= smoothQueue[currentIndex];
        currentSum += newIntensity;
        smoothQueue[currentIndex] = newIntensity;
        currentIndex = (currentIndex + 1) % smoothing;

        float smoothIntensity = currentSum / smoothing;

        if (flickerIntensity)
        light2D.intensity = smoothIntensity;
    }
}
