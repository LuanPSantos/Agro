using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BowController : NetworkBehaviour
{
    public static event Action<float> LaunchForcePercentChanged;

    public static event Action Fired;

    public float timeToFullLoadLauchForce = 3f;
    public float releaseThreshold = 0.01f;
    public int maxLaunchForce = 2000;
    public Transform bowTransform;
    public GameObject arrow;

    private Camera mainCamera;
    private float currentLaunchForcePercent;
    private float timeSpentPulling = 0f;

    private ulong clientId;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Aim();
        Pull();
        Fire();
    }

    private void Aim()
    {
        bowTransform.rotation = GetAimRotation();
    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireServerRpc(clientId, GetLaunchForce(), GetArrowSpawnPosition(), GetAimDirection(), GetAimRotation());

            enabled = false;
        }
    }

    private void Pull()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            PullArrow();
        }
        else
        {
            if (currentLaunchForcePercent > releaseThreshold)
            {
                ReleaseArrow();
            }
        }        
    }

    private Quaternion GetAimRotation()
    {
        Vector3 directionToLook = GetAimDirection();

        float angle = Mathf.Atan2(directionToLook.y, directionToLook.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector3 GetAimDirection()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - (Vector2)bowTransform.position).normalized;
    }

    private float GetLaunchForce()
    {
        return maxLaunchForce;
    }

    private Vector2 GetArrowSpawnPosition()
    {
        return bowTransform.TransformPoint(Vector3.right);
    }

    private void PullArrow()
    {
        timeSpentPulling += Time.deltaTime;

        currentLaunchForcePercent = Mathf.Clamp(timeSpentPulling / timeToFullLoadLauchForce, 0f, 1f);
        LaunchForcePercentChanged?.Invoke(currentLaunchForcePercent);
    }

    private void ReleaseArrow()
    {
        timeSpentPulling -= Time.deltaTime;

        currentLaunchForcePercent = Mathf.Clamp(timeSpentPulling / timeToFullLoadLauchForce, 0f, 1f);
        LaunchForcePercentChanged?.Invoke(currentLaunchForcePercent);
    }

    public void SetClientId(ulong clientId)
    {
        this.clientId = clientId;
    }

    [ServerRpc]
    void FireServerRpc(ulong clientId, float force, Vector3 positon, Vector3 direction, Quaternion rotation)
    {
        if (!IsServer) return;

        NetworkLog.LogInfoServer("FireServerRpc");

        GameObject spawnedArrow = Instantiate(arrow, positon, rotation);
        spawnedArrow.GetComponent<NetworkObject>().Spawn();
        spawnedArrow.GetComponent<ArrowBehaviour>().SetClientId(clientId);
        spawnedArrow.GetComponent<Rigidbody2D>().AddForce(direction * force);

        Fired?.Invoke();
    }
}
