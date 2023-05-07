using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D ObjRigidbody;
    private GameObject pieceOfLightPrefab;

    protected float entityHealth;
    protected bool isGrounded = false;
    protected bool isPlayerInSight = false;

    private void Start()
    {
        pieceOfLightPrefab = Resources.Load<GameObject>("PieceofLight");
    }

    public virtual void EntityGetDamage(float damage)
    {
        animator = GetComponent<Animator>();
        entityHealth -= damage;

        if (entityHealth <= 0)
        {
            DropLight(Random.Range(4, 8));

            if (isGrounded)
                animator.SetBool("Die", true);

            else animator.SetBool("AirDie", true);
        }

        else animator.SetBool("Hurt", true);
    }

    public virtual void ToggleGetDamage()
    {
        GetComponent<Animator>().SetBool("Hurt", false);
    }

    protected virtual bool MeleeAttack(Vector3 entityDirection, bool isGetDamage, bool isDie)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.up + transform.right * entityDirection.x + new Vector3(0f, -11f, 0f), entityDirection, 9f);

        foreach (RaycastHit2D hit in hits)
            if (hit.collider.gameObject == Player.Instance.gameObject)
            {
                isPlayerInSight = true;

                if (!isGetDamage && !isDie)
                    GetComponent<Animator>().SetBool("Attack", true);
            }
            else
            {
                isPlayerInSight = false;
                GetComponent<Animator>().SetBool("Attack", false);
            }

        return isPlayerInSight;
    }

    protected virtual void IsGroundChecker()
    {
        animator = GetComponent<Animator>();
        ObjRigidbody = GetComponent<Rigidbody2D>();

        Vector2 boxSize = new Vector2(12f, 1.5f);

        Vector2 currentPosition = transform.position;
        Vector2 boxOrigin = currentPosition - new Vector2(0f, 15f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxOrigin, boxSize, 0f);
           

        foreach (Collider2D hit in hits)
        {
            isGrounded = hit != null && hit.gameObject != gameObject;

            if (hit != null && hit.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        if (!isGrounded && !(entityHealth <= 0))
        {
            if (ObjRigidbody.velocity.y < 0f)
            {
                animator.SetBool("Fall", true);
                animator.SetBool("Jump", false);
            }
            else
            {
                animator.SetBool("Fall", false);
                animator.SetBool("Jump", true);
            }

            animator.SetBool("isGrounded", false);
        }
        else
        {
            animator.SetBool("Fall", false);
            animator.SetBool("Jump", false);
            animator.SetBool("isGrounded", true);
        }
    }

    protected virtual void DropLight(int pieceСount)
    {
        for (int i = 0; i < pieceСount; i++)
        {
            GameObject pieceOfLight = Instantiate(pieceOfLightPrefab, transform.position, Quaternion.identity);

            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            pieceOfLight.GetComponent<Rigidbody2D>().AddForce(direction * 7f);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dropped"))
        {
            entityHealth += 1f;
            Destroy(other.gameObject);
        }
    }

    protected virtual void EntityDie()
    {
        Destroy(this.gameObject);
    }

}
