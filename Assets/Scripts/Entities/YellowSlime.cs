using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yellow : Slime
{
    [SerializeField] private float jumpForce = 60f;
    [SerializeField] private float jumpInterval = 1f;

    private float jumpTimer = 0f;

    private void Update()
    {
        if (isGrounded)
        {
            jumpTimer += Time.fixedDeltaTime;

            if (jumpTimer >= jumpInterval)
            {
                jumpTimer = 0f;
                GetComponent<Rigidbody2D>().AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        IsGroundChecker();
    }
}