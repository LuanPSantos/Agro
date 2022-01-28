using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{
    public static CameraManager Singleton;
    public CinemachineTargetGroup targetGroup;

    void Awake()
    {
        StartSingleton();
    }
    void Start()
    {
        //SpawnManager.Singleton.PlayersSpawned += PlayersSpawnedHandle;
    }

    private void PlayersSpawnedHandle(ulong one)
    {
        if (!IsServer || !IsHost) return;

        LookToPlayersClientRpc();

    }

    public void AddToTargetGroup(Transform playerTransform)
    {
        targetGroup.AddMember(playerTransform, 1, 1);
    }

    [ClientRpc]
    private void LookToPlayersClientRpc()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach(PlayerController player in players)
        {
            targetGroup.AddMember(player.transform, 1, 1);
        }
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
