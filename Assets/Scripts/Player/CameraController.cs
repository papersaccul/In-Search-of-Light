using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(1, 3), Description("1 - orthographic, 2 - transform, 3 - rect")] private int type = 1;
    [SerializeField] private Transform player;
    [SerializeField] private float cameraFollowSpeed = 3f;
    [SerializeField] private float minCameraSize = 35f;
    [SerializeField] private float maxCameraSize = 55f;
    [SerializeField] private float cameraSizeSmoothTime = 1.5f;
    [SerializeField] private float positionZ = -15f;
    [SerializeField] private float offsetY = 6f;
    [SerializeField] private float offsetX = 0f;
    
    private new Camera camera;
    private Vector3 playerPosition;
    private float currentCameraSizeVelocity;

    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<Player>().transform;

        camera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        playerPosition = player.position;
        playerPosition.z = positionZ;
        playerPosition.y += offsetY;
        playerPosition.x += offsetX;

        transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * cameraFollowSpeed);
         
        float newCameraSize = Mathf.Lerp(minCameraSize, maxCameraSize, player.GetComponent<Rigidbody2D>().velocity.magnitude / 10f);
        
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, newCameraSize, ref currentCameraSizeVelocity, cameraSizeSmoothTime);
           
        
    }
}
