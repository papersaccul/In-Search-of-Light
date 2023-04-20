using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yellow : Slime
{
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private float jumpInterval = 2f;
    [SerializeField] private float visionDistance = 50f;
    public Animator animator;

    private float jumpTimer = 0f;

    private void FixedUpdate()
    {
        IsGroundChecker();

        if (isGrounded)
        {
            jumpTimer += Time.fixedDeltaTime;

            if (jumpTimer >= jumpInterval)
            {
                jumpTimer = 0f;
                GetComponent<Rigidbody2D>().AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}