using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleConroller : MonoBehaviour
{
    public static ParticleConroller Instance { get; set; }

    [SerializeField] ParticleSystem doubleJumpParticle;
    [SerializeField] ParticleSystem dashParticle;

    public void Start()
    {
        Instance = this;
    }

    public void DoubleJump()
    {
        doubleJumpParticle.Play();
    }

    public void Dash(float direction)
    {
        dashParticle.transform.localScale = new Vector2(-2 * direction, dashParticle.transform.localScale.y);
        dashParticle.Play();
    }
}
