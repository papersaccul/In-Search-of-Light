using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [SerializeField, Header("Movement")]
                     private float playerSpeedMultiplier = 55f;
    [SerializeField] private float playerJumpForce = 50f;
    [SerializeField] private float playerMaxJumpHeight = 30f;

    private void PlayerRun()
    {
        float targetVelocityX = Input.GetAxis("Horizontal") * playerSpeedMultiplier;
        float currentVelocityX = ObjRigidbody.velocity.x;
        float smoothTime = 3f;
        float newVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, smoothTime * Time.deltaTime);

        // anim
        if (Mathf.Abs(currentVelocityX) >= 0f && Mathf.Abs(currentVelocityX) < 2f && ObjSprite.flipX != targetVelocityX < 0f)
            State = States.rotate;
        else if ((Mathf.Abs(currentVelocityX) > 5f)) State = States.run;
        ObjSprite.flipX = targetVelocityX < 0f;

        // Ground
        if (isGrounded && !isSlope && !isWall)
            ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);

        // Slope
        else if (isGrounded && isSlope && !isWall)
        {
            if (Input.GetButton("Jump"))
                ObjRigidbody.velocity = new Vector2(SlopeNormalPerpendicular.x * -targetVelocityX, playerJumpForce);
            else
                ObjRigidbody.velocity = new Vector2(SlopeNormalPerpendicular.x * -targetVelocityX, SlopeNormalPerpendicular.y * -targetVelocityX);
        }

        // Air
        else if (!isGrounded && !isWall)
            ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);
        
    }

    private void PlayerJump()
    {
        playerStamina -= 0.5f;
        StaminaBar.Instance.UpdateStaminaSlider(playerStamina);

        float clampedVerticalSpeed = Mathf.Clamp(ObjRigidbody.velocity.y, -playerMaxJumpHeight, playerMaxJumpHeight);

        ObjRigidbody.velocity = new Vector2(ObjRigidbody.velocity.x, clampedVerticalSpeed);

        ObjRigidbody.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
    }
}
