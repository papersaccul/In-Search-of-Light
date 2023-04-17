using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float minCameraDistance = -10f;
    [SerializeField] private float maxCameraDistance = -20f;
    [SerializeField] private float cameraFollowSpeed = 2f;
    [SerializeField] private float minCameraSize = 30f;
    [SerializeField] private float maxCameraSize = 45f;
    [SerializeField] private float cameraSizeSmoothTime = 1.5f;

    private new Camera camera;
    private Vector3 playerPosition;
    private float currentCameraSizeVelocity;

    private void Awake()
    {
        if (!player)
        {
            player = FindObjectOfType<Player>().transform;
        }

        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        
        playerPosition = player.position;
        playerPosition.z = -10f;
        playerPosition.y += 10f;

        transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * cameraFollowSpeed);


        float newCameraSize = Mathf.Lerp(minCameraSize, maxCameraSize, player.GetComponent<Rigidbody2D>().velocity.magnitude / 10f);
        Debug.Log("newCameraSize: " + newCameraSize + " velocity: " + player.GetComponent<Rigidbody2D>().velocity.magnitude);
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, newCameraSize, ref currentCameraSizeVelocity, cameraSizeSmoothTime);
    }
}
