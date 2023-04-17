using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerHealth = 10;
    [SerializeField] private float damageGetDelay = 1f;
    [SerializeField] private float playerDamage = 10f;
    [SerializeField] private float playerSpeedMultiplier = 50f;
    [SerializeField] private float playerJumpForce = 50f;
    [SerializeField] private float playerMaxJumpHeight = 30f;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isGetsDamage = false;


    private Rigidbody2D ObjRigidbody;
    private Animator ObjAnimator;
    private SpriteRenderer ObjSprite;

    public static Player Instance { get; set; }

    private States State
    {
        get { return (States)ObjAnimator.GetInteger("State"); }
        set { ObjAnimator.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        ObjRigidbody = GetComponent<Rigidbody2D>();
        ObjAnimator = GetComponent<Animator>();
        ObjSprite = GetComponentInChildren<SpriteRenderer>();

        Instance = this;
    }

    private void Update()
    {
        if (isGrounded && !isAttacking) State = States.idle;

        if (!isAttacking && Input.GetButton("Horizontal"))
            PlayerRun();

        if (isGrounded && Input.GetButton("Jump"))
            PlayerJump();

        if (isGrounded && !isAttacking && Input.GetButtonDown("Fire1"))
            PlayerAttack();

        if (!Input.GetButton("Horizontal") && !isAttacking && Mathf.Abs(ObjRigidbody.velocity.x) > 10f && Mathf.Abs(ObjRigidbody.velocity.x) < 35f)
            State = States.stop;

        IsGroundChecker(); // if use it in FixedUpdate(), then the character gets a little stuck in the walls.
    }

    private void PlayerRun()
    {
        float targetVelocityX = Input.GetAxis("Horizontal") * playerSpeedMultiplier;
        float currentVelocityX = ObjRigidbody.velocity.x;

        if (isGrounded && Mathf.Abs(currentVelocityX) > 10f)
            State = States.run;

        float smoothTime = 3f;

        float newVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, smoothTime * Time.deltaTime);

        ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);

        if (Mathf.Abs(currentVelocityX) > 1f)
            ObjSprite.flipX = targetVelocityX < 0f;
    }

    private void PlayerJump()
    {
        float clampedVerticalSpeed = Mathf.Clamp(ObjRigidbody.velocity.y, -playerMaxJumpHeight, playerMaxJumpHeight);
        ObjRigidbody.velocity = new Vector2(ObjRigidbody.velocity.x, clampedVerticalSpeed);

        ObjRigidbody.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
    }

    private void IsGroundChecker()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 14.6f; 

        RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, direction, distance);

        foreach (RaycastHit2D hit in hits)
            isGrounded = (hit.collider != null && hit.collider.gameObject != gameObject);

        if (!isGrounded)
        {
            if (ObjRigidbody.velocity.y < 0)
                State = States.fall;
            else
                State = States.jump;
        }
    }

    public void PlayerGetDamage()
    {
        if (!isGetsDamage)
        {
            playerHealth -= 1;
            Debug.Log(playerHealth);
            isGetsDamage = true;
            StartCoroutine(ResetGetsDamageState());
        }
    }

    private void PlayerAttack()
    {
        State = States.attack1;
        isAttacking = true;
        StartCoroutine(ResetAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.41f);
        isAttacking = false;
    }

    private IEnumerator ResetGetsDamageState()
    {
        yield return new WaitForSeconds(damageGetDelay);
        isGetsDamage = false;
    }

}

public enum States
{
    idle,       // 0
    run,        // 1
    jump,       // 2
    fall,       // 3
    attack1,    // 4
    attack2,    // 5
    block,      // 6
    die,        // 7
    stop        // 8    
}
