using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class ArrowBehaviour : NetworkBehaviour
{
    public static event Action ArrowCollided;
    public static event Action<ContactPoint2D> ArrowCollidedWithPlayer;

    private Rigidbody2D rb;
    private bool hasCollided = false;
    private Collider2D arrowCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        NetworkLog.LogInfoServer("Arrow Spawned");

        StartCoroutine(EnableCollider());

        CameraManager.Singleton.RemovePlayersFromGroup();
        CameraManager.Singleton.AddArrowToTargetGroup(transform);
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

        if (collision.gameObject.CompareTag("Player"))
        {
            ArrowCollidedWithPlayer?.Invoke(collision.GetContact(0));
        }

        ArrowCollided?.Invoke();

        Destroy(gameObject, 3f);
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
        arrowCollider.enabled = false;
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);

        arrowCollider.enabled = true;
    }
}
