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
    void Start()
    {
   
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
        targetGroup.AddMember(playersTransforms[playerClientId], 1, 1);
    }

    public void RemovePlayerFromTargetGroup(ulong playerClientId)
    {
        if (!IsClient) return;

        NetworkLog.LogInfoServer("RemovePlayerFromTargetGroupClientRpc id=" + playerClientId);

        targetGroup.RemoveMember(playersTransforms[playerClientId]);
    }

    public void RemovePlayersFromGroup()
    {
        foreach(Transform playerTransform in playersTransforms)
        {
            targetGroup.RemoveMember(playerTransform);
        }
    }

    public void AddArrowToTargetGroup(Transform currentArrowTransform)
    {
        arrowTransform = currentArrowTransform;
        targetGroup.AddMember(arrowTransform, 1, 1);
    }

    public void RemoveArrowToTargetGroup()
    {
        targetGroup.RemoveMember(arrowTransform);
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
