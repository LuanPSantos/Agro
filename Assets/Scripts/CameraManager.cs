using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{
    public static CameraManager Singleton;
    public CinemachineTargetGroup targetGroup;

    private Transform[] playersTransforms = new Transform[2];
    private Transform arrowTransform;

    void Awake()
    {
        StartSingleton();
    }

    public void AddPlayerTransform(ulong playerClientId, Transform playerTransform)
    {
        if (!IsClient) return;

        playersTransforms[playerClientId] = playerTransform;
        NetworkLog.LogInfoServer("AddPlayerTransform id=" + playerClientId + " transform="+ playersTransforms[playerClientId]);
    }

    public void AddPlayerToTargetGroup(ulong playerClientId)
    {
        if (!IsClient) return;

        NetworkLog.LogInfoServer("AddPlayerToTargetGroupClientRpc id=" + playerClientId);
        Add(playersTransforms[playerClientId]);
    }

    public void RemovePlayerFromTargetGroup(ulong playerClientId)
    {
        if (!IsClient) return;

        NetworkLog.LogInfoServer("RemovePlayerFromTargetGroupClientRpc id=" + playerClientId);

        Remove(playersTransforms[playerClientId]);
    }

    public void RemovePlayersFromGroup()
    {
        if (!IsClient) return;

        foreach (Transform playerTransform in playersTransforms)
        {
            Remove(playerTransform);
        }
    }

    public void AddArrowToTargetGroup(Transform currentArrowTransform)
    {
        if (!IsClient) return;

        arrowTransform = currentArrowTransform;
        Add(arrowTransform);
    }

    public void RemoveArrowToTargetGroup()
    {
        if (!IsClient) return;

        Remove(arrowTransform);
    }

    private void Add(Transform target)
    {
        targetGroup.AddMember(target, 1, 1);
    }

    private void Remove(Transform target)
    {
        targetGroup.RemoveMember(target);
    }

    private void StartSingleton()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
