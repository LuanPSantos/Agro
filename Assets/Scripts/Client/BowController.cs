using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BowController : MonoBehaviour
{
    public static event Action<float> LaunchForcePercentChanged;

    public event Action<float, Vector3, Vector3, Quaternion> Fired;

    public float timeToFullLoadLauchForce = 3f;
    public float releaseThreshold = 0.01f;
    public int maxLaunchForce = 2000;
    public Transform bowTransform;

    private Camera mainCamera;
    private float currentLaunchForcePercent;
    private float timeSpentPulling = 0f;

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
            Fired?.Invoke(GetLaunchForce(), GetArrowSpawnPosition(), GetAimDirection(), GetAimRotation());
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
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - bowTransform.position).normalized;
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
        NetworkLog.LogInfoServer("PullArrow " + currentLaunchForcePercent);
        timeSpentPulling += Time.deltaTime;

        currentLaunchForcePercent = Mathf.Clamp(timeSpentPulling / timeToFullLoadLauchForce, 0f, 1f);
        LaunchForcePercentChanged?.Invoke(currentLaunchForcePercent);
    }

    private void ReleaseArrow()
    {
        NetworkLog.LogInfoServer("ReleaseArrow " + currentLaunchForcePercent);
        timeSpentPulling -= Time.deltaTime;

        currentLaunchForcePercent = Mathf.Clamp(timeSpentPulling / timeToFullLoadLauchForce, 0f, 1f);
        LaunchForcePercentChanged?.Invoke(currentLaunchForcePercent);
    }
}
