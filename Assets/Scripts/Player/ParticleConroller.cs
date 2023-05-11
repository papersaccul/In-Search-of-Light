using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleConroller : MonoBehaviour
{
    public static ParticleConroller Instance { get; set; }

    [SerializeField] ParticleSystem doubleJumpParticle;

    public void Start()
    {
        Instance = this;
    }

    public void DoubleJump()
    {
        doubleJumpParticle.Play();
    }
}
