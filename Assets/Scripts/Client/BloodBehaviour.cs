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
        if (!IsServer) return;

        SpawnBloodClientRpc(contactPoint2D.point, contactPoint2D.normal);
    }

    [ClientRpc]
    private void SpawnBloodClientRpc(Vector2 point, Vector2 normal)
    {
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        particle.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        particle.transform.position = point;

        particle.Play();
    }
}
