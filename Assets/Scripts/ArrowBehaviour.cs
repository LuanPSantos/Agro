using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class ArrowBehaviour : NetworkBehaviour
{
    public static event Action ArrowCollided;

    private Rigidbody2D rb;
    private bool hasCollided = false;
    private Collider2D arrowCollider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(hasCollided == false)
        {
            AlignRotation();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {      
        hasCollided = true;
        DisablePhysics();
        ArrowCollided?.Invoke();

        if(!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 5f);
        }

        arrowCollider.enabled = false;
    }

    private void AlignRotation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void DisablePhysics()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

     public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            NetworkLog.LogInfoServer("OnNetworkSpawn arrow");
            CameraManager.Singleton.RemovePlayersFromGroup();
            CameraManager.Singleton.AddArrowToTargetGroup(transform);
        }
    }
}
