using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BloodBehaviour : NetworkBehaviour
{
    private ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    void Start()
    {
        ArrowBehaviour.ArrowCollidedWithPlayer += ArrowCollidedWithPlayerHandle;
    }

    private void ArrowCollidedWithPlayerHandle(ContactPoint2D contactPoint2D)
    {
        if (!IsClient) return;

        particle.transform.position = contactPoint2D.point;

        particle.Play();
    }
}
