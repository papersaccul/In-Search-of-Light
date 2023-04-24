using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private Transform rayCastOrigin;
    [SerializeField] private Transform entityFeet;
    [SerializeField] private LayerMask layerMask;

    private RaycastHit2D hit;

    private void Update()
    {
        isGroundChecker();
    }

    private void isGroundChecker()
    {
        hit = Physics2D.Raycast(rayCastOrigin.position, -Vector2.up, 100f, layerMask);

        if (!hit)
        {
            Vector2 temp = entityFeet.position;
            temp.y = hit.point.y;
            entityFeet.position = temp;
        }
    }
}
