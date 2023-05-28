using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public partial class Player : MonoBehaviour
{
    [SerializeField, Header("Attack")]
                     public bool isAttacking = false;
    [SerializeField] private bool isEnhAttacking = false;
    [SerializeField] public bool isRecharged = false;
    [SerializeField] public float attackRange = 24.5f;
    [SerializeField] private bool isGetDamage = false;

    private float attackTimeCounter;

    public Transform ObjAttackPosition;
    public Light2D playerLight;

    private void PlayerAttack()
    {
        if (isGrounded && isRecharged)
        {
            playerStamina -= 2f;
            StaminaBar.Instance.UpdateStaminaSlider(playerStamina);

            State = States.attack1;
            WeaponState = WeaponStates.attack;
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
        float attackOffsetX = 0f;

        if (MainHand == MainHand.defaultSword)
        {
            attackOffsetX = playerFlipX ? -8.5f : +8.5f;
        }

        ObjAttackPosition.position = new Vector3(ObjRigidbody.position.x + attackOffsetX, ObjAttackPosition.position.y, 0f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(ObjAttackPosition.position, attackRange, enemyEntity);

        foreach (Collider2D collider in colliders)
            collider.GetComponent<Entity>().EntityGetDamage(playerDamage);
    }

    private void EnhancedAttack()
    {
        if (isGrounded && isRecharged)
        {
            playerStamina = 0f;
            StaminaBar.Instance.UpdateStaminaSlider(playerStamina);

            playerHealth -= 5f;
            HealthBar.Instance.UpdateHealthBar(playerHealth);

            isEnhAttacking = true;

            Debug.Log(MainHand);

            if (MainHand == MainHand.defaultSword || MainHand == MainHand.spear)
            {
                State = States.attackEnh;
                WeaponState = WeaponStates.attack;
                isAttacking = true;
                isRecharged = false;

                StartCoroutine(LightAttackAnimation());
                StartCoroutine(AttackCoolDown());
            }
        }
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.41f);
        isAttacking = false;
        WeaponState = WeaponStates.idle;
    }

    private IEnumerator LightAttackAnimation()
    {
        Dictionary<MainHand, GameObject> weaponPrefabs = new Dictionary<MainHand, GameObject>() {
            { MainHand.defaultSword, Resources.Load<GameObject>("LightSaber") },
            { MainHand.spear, Resources.Load<GameObject>("LightSpear") },
        };

        Dictionary<MainHand, float> LightSpeeds = new Dictionary<MainHand, float>() {
            { MainHand.defaultSword, 1000f }, 
            { MainHand.spear, 1500f }
        };

        GameObject lightPrefab = null;

        if (!weaponPrefabs.TryGetValue(MainHand, out lightPrefab))
        {
            Debug.LogError($"Invalid MainHand: {MainHand}");
            yield break;
        }

        float lightSpeed = 0f;

        if (!LightSpeeds.TryGetValue(MainHand, out lightSpeed))
        {
            Debug.LogError($"Invalid MainHand: {MainHand}");
            yield break;
        }

        if (lightPrefab == null)
        {
            Debug.LogError("The ligth attack object is missing from the Resources folder.");
            yield break;
        }

        float saberOffsetX = playerFlipX ? -7f : +7f;
        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 25f, .3f);

        yield return new WaitForSeconds(.3f);

        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, oldPlayerLight, .3f);

        GameObject newSaber = Instantiate(lightPrefab, new Vector3(ObjAttackPosition.position.x + saberOffsetX, ObjAttackPosition.position.y, 0f), Quaternion.identity);
        newSaber.GetComponentInChildren<Light2D>().transform.right *= playerFlipX ? -1f : 1f;
        newSaber.GetComponent<Rigidbody2D>().AddForce(playerFlipX ? Vector2.left * lightSpeed : Vector2.right * lightSpeed);

        yield return new WaitForSeconds(.3f);

        WeaponState = WeaponStates.idle;
        isEnhAttacking = false;
        isAttacking = false;

        yield return true;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(.7f);
        isRecharged = true;
    }
}
