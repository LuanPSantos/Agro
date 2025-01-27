using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class ArrowBehaviour : NetworkBehaviour
{
    public static event Action<ulong, ContactPoint2D, string> ArrowCollided;

    private Rigidbody2D rb;
    private bool hasCollided = false;
    private Collider2D arrowCollider;

    private ulong clientId;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkLog.LogInfoServer("Arrow Spawned");

        StartCoroutine(EnableCollider());

        if (!IsClient) return;

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
        if (hasCollided) return;

        NetworkLog.LogInfoServer("Arrow OnCollisionEnter2D with " + collision.gameObject.tag);
        hasCollided = true;
        DisablePhysics();

        ArrowCollided?.Invoke(clientId, collision.GetContact(0), collision.gameObject.tag);

        Destroy(gameObject, 3f);
    }

    public void SetClientId(ulong clientId)
    {
        this.clientId = clientId;
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
