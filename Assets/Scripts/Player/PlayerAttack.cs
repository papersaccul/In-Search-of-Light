using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [SerializeField, Header("Attack")]
    public bool isAttacking = false;
    [SerializeField] public bool isRecharged = false;
    [SerializeField] public float attackRange = 24.5f;
    [SerializeField] private bool isGetDamage = false;
    public Transform ObjAttackPosition;

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
}
