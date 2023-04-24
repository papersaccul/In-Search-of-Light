using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] public float playerDamage = 3.5f;
    [SerializeField] private int playerHealth = 10;
    [SerializeField] private float damageGetDelay = 0.7f;

    [SerializeField] private float playerSpeedMultiplier = 55f;
    [SerializeField] private float playerJumpForce = 50f;
    [SerializeField] private float playerMaxJumpHeight = 30f;

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isSlope = false;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool isRecharged = false;
    [SerializeField] private bool isGetDamage = false;

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

    private void FixedUpdate()
    {
        if (isGrounded && !isAttacking && !isGetDamage)
            State = States.idle;

        if (!isAttacking && !isGetDamage && Input.GetButton("Horizontal"))
            PlayerRun();

        if (isGrounded && Input.GetButton("Jump"))
            PlayerJump();

        if (!Input.GetButton("Horizontal") && !isAttacking && Mathf.Abs(ObjRigidbody.velocity.x) > 10f && Mathf.Abs(ObjRigidbody.velocity.x) < 35f)
            State = States.stop;

        if (isGrounded && !isAttacking && !isGetDamage && Input.GetButtonDown("Fire1"))
            PlayerAttack();

        IsGroundChecker(); // if use it in FixedUpdate(), then the character gets a little stuck in the walls.
    }

    private void PlayerRun()
    {
        float targetVelocityX = Input.GetAxis("Horizontal") * playerSpeedMultiplier;
        float currentVelocityX = ObjRigidbody.velocity.x;
        float smoothTime = 3f;
        float newVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, smoothTime * Time.fixedDeltaTime);

        if (!isSlope)
            ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);

        else ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y + 50f * Time.fixedDeltaTime);


        if (Mathf.Abs(currentVelocityX) >= 0f && Mathf.Abs(currentVelocityX) < 2f && ObjSprite.flipX != targetVelocityX < 0f)
            State = States.rotate;

        else if ((Mathf.Abs(currentVelocityX) > 5f)) State = States.run;
                ObjSprite.flipX = targetVelocityX < 0f;
    } 

    private void PlayerJump()
    {
        float clampedVerticalSpeed = Mathf.Clamp(ObjRigidbody.velocity.y, -playerMaxJumpHeight, playerMaxJumpHeight);
        
        ObjRigidbody.velocity = new Vector2(ObjRigidbody.velocity.x, clampedVerticalSpeed);

        ObjRigidbody.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
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

    // Debug Attack Range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(ObjAttackPosition.position, attackRange);
    }

    // Will be called by animator
    private void onAttack()
    {
        float swordOffsetX;

        swordOffsetX = ObjSprite.flipX ? -8.5f : +8.5f;
        ObjAttackPosition.position = new Vector3(ObjRigidbody.position.x + swordOffsetX, ObjAttackPosition.position.y, 0f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(ObjAttackPosition.position, attackRange, enemyEntity);

        foreach (Collider2D collider in colliders)
            collider.GetComponent<Entity>().EntityGetDamage();
    }

    public void PlayerGetDamage(int damage, Vector3 attackPosition)
    {
        if (!isGetDamage)
        {
            playerHealth -= damage;
            Debug.Log(playerHealth);
            isGetDamage = true;
            State = States.getDamage;
            
            ObjRigidbody.velocity = Vector2.zero;

            Vector2 deltaPosition = transform.position - attackPosition;

            Vector2 impulse = (deltaPosition + new Vector2(0f, 10f)).normalized * 35f;

            ObjRigidbody.AddForce(impulse, ForceMode2D.Impulse);

            StartCoroutine(ResetGetsDamageState());
        }
    }

    private void IsGroundChecker()
    {
        Vector2 currentPosition = transform.position;
        Vector2 boxSize = new Vector2(3.5f, 0.1f);

        Vector2 boxCastOrigin = currentPosition - new Vector2(0f, boxSize.y / 2f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCastOrigin, boxSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            isGrounded = (collider.gameObject != null && collider.gameObject != gameObject);
            isSlope = collider.gameObject.CompareTag("Slope");
        }

        if (!isGrounded)
        {
            if (ObjRigidbody.velocity.y < 0)
             State = States.fall;
            else
                State = States.jump;
        }
    }

    // Debug Ground Checker
    private void OnDrawGizmos()
    {
        Vector2 boxSize = new Vector2(3.5f, 0.1f);

        Gizmos.color = (!isGrounded && !isSlope) ? Color.red : (isSlope ? Color.yellow : Color.green);

        Gizmos.DrawWireCube(transform.position - new Vector3(0f, boxSize.y / 2f, 0f), boxSize);
    }

    // Someday I'll redo it through the Unity animator.
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
        isGetDamage = false;
    }

}

public enum States
{
    idle,       // 0
    run,        // 1
    jump,       // 2
    fall,       // 3
    attack1,    // 4
    attack2,    // 5 not used
    block,      // 6 not used
    die,        // 7
    stop,       // 8    
    rotate,     // 9      
    getDamage   // 10
}
