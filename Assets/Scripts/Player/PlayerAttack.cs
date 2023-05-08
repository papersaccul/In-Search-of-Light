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
            collider.GetComponent<Entity>().EntityGetDamage(playerDamage);
    }

    private void EnhancedAttack()
    {
        Debug.Log("Enh Attack");

        playerStamina = 0f;
        StaminaBar.Instance.UpdateStaminaSlider(playerStamina);

        playerHealth -= 5;
        HealthBar.Instance.UpdateHealthBar(playerHealth);

        isEnhAttacking = true;

        if (isGrounded && isRecharged)
        {
            State = States.attackEnh;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(EnhancedAttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.41f);
        isAttacking = false;
    }

    private IEnumerator EnhancedAttackAnimation()
    {
        float saberOffsetX = ObjSprite.flipX ? -7f : +7f;
        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 25f, .3f);

        yield return new WaitForSeconds(.3f);

        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, oldPlayerLight, .3f);
        GameObject newSaber = Instantiate(lightsaberPrefab, new Vector3(ObjAttackPosition.position.x + saberOffsetX, ObjAttackPosition.position.y, 0f), Quaternion.identity);
        newSaber.GetComponent<Rigidbody2D>().AddForce(ObjSprite.flipX ? Vector2.left * 1000f : Vector2.right * 1000f);

        newSaber.GetComponentInChildren<Light2D>().transform.right *= ObjSprite.flipX ? -1f : 1f;

        yield return new WaitForSeconds(.3f);
        
        isEnhAttacking = false;
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(.7f);
        isRecharged = true;
    }
}
