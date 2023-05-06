using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{

    [SerializeField, Header("Ground")]
                     private bool isGrounded = false;
    [SerializeField] private bool isSlope = false;
    [SerializeField] private bool isWall = false;
    [SerializeField] private bool leftFeet = false;
    [SerializeField] private bool rightFeet = false;
    [SerializeField] private float slopeCheckDistance;

    [SerializeField, Header("Slope Friction")]
                     private PhysicsMaterial2D zeroFriction;
    [SerializeField] private PhysicsMaterial2D mediumFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    private Vector2 colliderSize;
    private Vector2 SlopeNormalPerpendicular;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private void IsGroundChecker()
    {
        Vector2 currentPosition = transform.position;
        Vector2 boxSize = new(2f, 0.1f);

        Collider2D[] collidersRight = Physics2D.OverlapBoxAll(currentPosition - new Vector2(-1.1f, boxSize.y / 2f), boxSize, 0f);
        Collider2D[] collidersLeft = Physics2D.OverlapBoxAll(currentPosition - new Vector2(1.1f, boxSize.y / 2f), boxSize, 0f);

        foreach (Collider2D collider in collidersRight)
            rightFeet = collider.gameObject.CompareTag("Ground");

        foreach (Collider2D collider in collidersLeft)
            leftFeet = collider.gameObject.CompareTag("Ground");

        isGrounded = leftFeet || rightFeet;

        if (!isGrounded)
        {
            if (ObjRigidbody.velocity.y < 0)
                State = States.fall;
            else
                State = States.jump;
        }
    }

    private void SlopeChecker()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, colliderSize.y / 2 - 2f);

        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, Ground);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, Ground);

        // Horizontal
        if (slopeHitFront)
        {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0f;
            isSlope = false;
        }

        // Vertical
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, Ground);

        if (hit)
        {
            SlopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
                isSlope = true;

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, SlopeNormalPerpendicular, Color.magenta);
            Debug.DrawRay(hit.point, hit.normal, Color.cyan);
        }

        if (isGrounded)
        {
            if (isSlope && !Input.GetButton("Horizontal"))
                ObjRigidbody.sharedMaterial = fullFriction;

            else ObjRigidbody.sharedMaterial = Input.GetButton("Horizontal") || isGetDamage ? zeroFriction : mediumFriction;
        }
    }

    private void WallChecker()
    {
        float raycastDistance = 3.5f;
        Vector2 raycastDirection = ObjSprite.flipX ? Vector2.left : Vector2.right;
        Vector2 raycastOrigin = transform.position + new Vector3(0f, 8f, 0f);

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastDistance, Ground);
        isWall = hit.collider;

        Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, isWall ? Color.red : Color.green);
    }

    // Debug Ground Checker
    private void OnDrawGizmos()
    {
        Vector2 currentPosition = transform.position;
        Vector2 boxSize = new(2f, 0.1f);

        if (leftFeet) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(currentPosition - new Vector2(1.1f, boxSize.y / 2f), boxSize);

        if (rightFeet) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(currentPosition - new Vector2(-1.1f, boxSize.y / 2f), boxSize);
    }

}
