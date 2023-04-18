using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraFollowSpeed = 3f;
    [SerializeField] private float minCameraSize = 35f;
    [SerializeField] private float maxCameraSize = 55f;
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
        playerPosition.z = -15f;
        playerPosition.y += 6f;

        transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * cameraFollowSpeed);
         

        float newCameraSize = Mathf.Lerp(minCameraSize, maxCameraSize, player.GetComponent<Rigidbody2D>().velocity.magnitude / 10f);
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, newCameraSize, ref currentCameraSizeVelocity, cameraSizeSmoothTime);
    }
}
