using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public partial class Player : MonoBehaviour
{
    [SerializeField, Header("Health")]
                     public float playerDamage = 3.5f;
    [SerializeField] public int playerMaxHealth = 20;
    [SerializeField] public float playerHealth = 10f;
    [SerializeField] private float damageGetDelay = 0.7f;
    float oldPlayerLight;

    [SerializeField, Header("Slope Friction")]
    private PhysicsMaterial2D zeroFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    [SerializeField, Header("Layers")]
    public LayerMask enemyEntity;
    public LayerMask Ground;

    private Rigidbody2D ObjRigidbody;
    private Animator ObjAnimator;
    private SpriteRenderer ObjSprite;
    private CapsuleCollider2D ObjCapsule;
    private GameObject pieceOfLightPrefab;
    private GameObject lightsaberPrefab;

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

        pieceOfLightPrefab = Resources.Load<GameObject>("PieceofLight");
        lightsaberPrefab = Resources.Load<GameObject>("LightSaber");
        playerLight = GetComponentInChildren<Light2D>();

        colliderSize = ObjCapsule.size;

        StartCoroutine(PlayerRegeneration());

        HealthBar.Instance.UpdateHealthBar(playerHealth);

        oldPlayerLight = playerLight.intensity;

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
        if (isGrounded && !isAttacking && !isGetDamage)
        {
            if (Input.GetButtonDown("Fire1"))
                attackTimeCounter = 0f;


            if (Input.GetButton("Fire1"))
            {
                if (playerHealth >= 15)
                {
                    attackTimeCounter += Time.fixedDeltaTime;

                    if (attackTimeCounter >= .4f)
                        EnhancedAttack();
                }
                else PlayerAttack();

            }

            if (Input.GetButtonUp("Fire1"))
                if (!isEnhAttacking && attackTimeCounter < .4f)
                    PlayerAttack();
        }


        if (isGrounded && Input.GetButton("Jump"))
            PlayerJump();
    }


    public void PlayerGetDamage(int damage, Vector3 attackPosition)
    {
        if (!isGetDamage)
        {

            playerHealth -= damage;
            isGetDamage = true;
            State = States.getDamage;
            HealthBar.Instance.UpdateHealthBar(playerHealth);
            ObjRigidbody.velocity = Vector2.zero;

            Vector2 deltaPosition = transform.position - attackPosition;

            Vector2 impulse = (deltaPosition + new Vector2(0f, 10f)).normalized * 35f;

            int pieceCount = Random.Range(damage / 2, damage);

            foreach (int i in Enumerable.Range(0, pieceCount))
            {
                GameObject pieceOfLight = Instantiate(pieceOfLightPrefab, transform.position + new Vector3(0, 20f, 0), Quaternion.identity);

                Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
                pieceOfLight.GetComponent<Rigidbody2D>().AddForce(direction * 10f);
            }

            ObjRigidbody.AddForce(impulse, ForceMode2D.Impulse);

            StartCoroutine(ResetGetsDamageState());
        }
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
    attackEnh,  // 5
    block,      // 6 not used
    die,        // 7 not used
    stop,       // 8    
    rotate,     // 9      
    getDamage   // 10
}
