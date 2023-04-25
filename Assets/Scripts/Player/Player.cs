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
    [SerializeField, Header("Health")] 
                     public float playerDamage = 3.5f;
    [SerializeField] private int playerHealth = 10;
    [SerializeField] private float damageGetDelay = 0.7f;

    [SerializeField, Header("Movement")] 
                     private float playerSpeedMultiplier = 55f;
    [SerializeField] private float playerJumpForce = 50f;
    [SerializeField] private float playerMaxJumpHeight = 30f;

    [SerializeField, Header("Ground")] 
                     private bool isGrounded = false;
    [SerializeField] private bool isSlope = false;
    [SerializeField] private bool slopeLeft = false;
    [SerializeField] private bool slopeRight = false;
    [SerializeField] private bool leftFeet = false;
    [SerializeField] private bool rightFeet = false;
    [SerializeField] private float slopeCheckDistance;

    [SerializeField, Header("Attack")] 
                     public bool isAttacking = false;
    [SerializeField] public bool isRecharged = false;
    [SerializeField] public float attackRange = 24.5f;
    [SerializeField] private bool isGetDamage = false;
                     public Transform ObjAttackPosition;

    [SerializeField, Header("Slope Friction")]
                     private PhysicsMaterial2D zeroFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    [SerializeField, Header("Layers")]
    public LayerMask enemyEntity;
    public LayerMask Ground;
    

    private Vector2 colliderSize;
    private Vector2 SlopeNormalPerpendicular;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    private Rigidbody2D ObjRigidbody;
    private Animator ObjAnimator;
    private SpriteRenderer ObjSprite;
    private CapsuleCollider2D ObjCapsule;

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
        ObjCapsule = GetComponentInChildren<CapsuleCollider2D>();

        colliderSize = ObjCapsule.size;

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (isGrounded && !isAttacking && !isGetDamage)
            State = States.idle;

        if (!isAttacking && !isGetDamage && Input.GetButton("Horizontal"))
            PlayerRun();

        if (!Input.GetButton("Horizontal") && !isAttacking && Mathf.Abs(ObjRigidbody.velocity.x) > 10f && Mathf.Abs(ObjRigidbody.velocity.x) < 35f)
            State = States.stop;

        IsGroundChecker();
        SlopeChecker();
    }

    private void Update()
    {
        if (isGrounded && !isAttacking && !isGetDamage && Input.GetButtonDown("Fire1"))
            PlayerAttack();

        if (isGrounded && Input.GetButton("Jump"))
            PlayerJump();
    }

    private void PlayerRun()
    {
        float targetVelocityX = Input.GetAxis("Horizontal") * playerSpeedMultiplier;
        float currentVelocityX = ObjRigidbody.velocity.x;
        float smoothTime = 3f;
        float newVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, smoothTime * Time.fixedDeltaTime);

        // Ground
        if (isGrounded && !isSlope)
        {
            ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);
        }

        // Slope
        else if (isGrounded && isSlope)
        {
            ObjRigidbody.velocity = new Vector2(SlopeNormalPerpendicular.x * -targetVelocityX, SlopeNormalPerpendicular.y * -targetVelocityX);
        }

        // Air
        else if (!isGrounded)
        {
            ObjRigidbody.velocity = new Vector2(newVelocityX, ObjRigidbody.velocity.y);
        }

        // anim
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
        Vector2 boxSize = new(2f, 0.1f);

        Collider2D[] collidersRight = Physics2D.OverlapBoxAll(currentPosition - new Vector2(-1.1f, boxSize.y / 2f), boxSize, 0f);
        Collider2D[] collidersLeft = Physics2D.OverlapBoxAll(currentPosition - new Vector2(1.1f, boxSize.y / 2f), boxSize, 0f);
        
        foreach (Collider2D collider in collidersRight)
        {
            rightFeet = collider.gameObject.CompareTag("Ground");
            slopeRight = collider.gameObject.CompareTag("Slope");
        }

        foreach (Collider2D collider in collidersLeft)
        {
            leftFeet = collider.gameObject.CompareTag("Ground");
            slopeLeft = collider.gameObject.CompareTag("Slope");
        }

        isGrounded = leftFeet || rightFeet || slopeLeft || slopeRight;

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
            ObjRigidbody.sharedMaterial = Input.GetButton("Horizontal") ? zeroFriction : fullFriction;
    }

    // Debug Ground Checker
    private void OnDrawGizmos()
    {
        Vector2 currentPosition = transform.position;
        Vector2 boxSize = new(2f, 0.1f);

        if (leftFeet && !slopeLeft) Gizmos.color = Color.green;
        else if (slopeLeft) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(currentPosition - new Vector2(1.1f, boxSize.y / 2f), boxSize);

        if (rightFeet && !slopeRight) Gizmos.color = Color.green;
        else if (slopeRight) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(currentPosition - new Vector2(-1.1f, boxSize.y / 2f), boxSize);
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
    die,        // 7 not used
    stop,       // 8    
    rotate,     // 9      
    getDamage   // 10
}
