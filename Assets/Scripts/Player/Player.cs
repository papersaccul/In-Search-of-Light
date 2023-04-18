using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerHealth = 10;
    [SerializeField] private float damageGetDelay = 1f;
    [SerializeField] public float playerDamage = 3.5f;

    [SerializeField] private float playerSpeedMultiplier = 50f;
    [SerializeField] private float playerJumpForce = 50f;
    [SerializeField] private float playerMaxJumpHeight = 30f;

    [SerializeField] private bool isGrounded = false;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool isRecharged = false;

    [SerializeField] private bool isGetsDamage = false;

    private float swordOffsetX;
    public float attackRange = 24.5f;
    public LayerMask enemyEntity;

    public Transform ObjAttackPosition;
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
        isRecharged = true;

        Instance = this;
    }

    private void Update()
    {
        if (isGrounded && !isAttacking) 
            State = States.idle;

        if (!isAttacking && Input.GetButton("Horizontal"))
            PlayerRun();

        if (isGrounded && Input.GetButton("Jump"))
            PlayerJump();

        if (!Input.GetButton("Horizontal") && !isAttacking && Mathf.Abs(ObjRigidbody.velocity.x) > 10f && Mathf.Abs(ObjRigidbody.velocity.x) < 35f)
            State = States.stop;

        if (isGrounded && !isAttacking && Input.GetButtonDown("Fire1"))
            PlayerAttack();

        swordOffsetX = ObjSprite.flipX ? -8.5f : +8.5f;
        ObjAttackPosition.position = new Vector3(ObjRigidbody.position.x + swordOffsetX, ObjAttackPosition.position.y, 0f);

        IsGroundChecker(); // if use it in FixedUpdate(), then the character gets a little stuck in the walls.
    }

    private void PlayerRun()
    {
        float targetVelocityX = Input.GetAxis("Horizontal") * playerSpeedMultiplier;
        float currentVelocityX = ObjRigidbody.velocity.x;
        float smoothTime = 3f;
        float newVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, smoothTime * Time.deltaTime);

        ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);


        if (Mathf.Abs(currentVelocityX) >= 0f && Mathf.Abs(currentVelocityX) < 2f)
        {
            if (ObjSprite.flipX != targetVelocityX < 0f)
                State = States.rotate;
        }

        else
        {
            State = States.run;
            ObjSprite.flipX = targetVelocityX < 0f;
        }
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


    private void PlayerAttack()
    {
        if (isGrounded && isRecharged)
        {
            State = States.attack1;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ObjAttackPosition.position, attackRange);
    }

    private void onAttack()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(ObjAttackPosition.position, attackRange, enemyEntity);

        foreach (Collider2D collider in colliders)
            collider.GetComponent<Enitity>().EntityGetDamage();
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

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.41f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.65f);
        isRecharged = true;
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
    stop,       // 8    
    rotate      // 9      
}
