using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private bool isFall;

    public Collider2D entityCollider;
    public LayerMask Ground;

    public bool isGrounded = false;

    // debug
    [Range(-2f, 2f)] public float boxCastYOffset = -0.1f;
    [Range(-2f, 2f)] public float boxCastXOffset = -0.1f;
    [Range(0, 2)] public float boxCastWidth = 1, boxCastHeight = 1;
    public Color gizmoColorNotGrounded = Color.red, gizmoColorIsGrounded = Color.green;


    private void Awake()
    {
        if (entityCollider != null)
            entityCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        isGroundChecker();
    }

    private void isGroundChecker()
    {
        if (isFall)
        {
            isGrounded = true;
            return;
        }

        RaycastHit2D raycastHit2D = Physics2D.BoxCast(entityCollider.bounds.center + new Vector3(boxCastXOffset, boxCastYOffset, 0), new Vector2(boxCastWidth, boxCastHeight), 45f, new Vector2(0f, 0f)); 
    }
}
