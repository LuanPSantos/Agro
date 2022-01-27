using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{

    public CinemachineTargetGroup targetGroup;

    void Start()
    {
        SpawnManager.Singleton.PlayersSpawned += OnPlayersSpawned;
    }

    private void OnPlayersSpawned(ulong one, ulong two)
    {
        if (!IsServer || !IsHost) return;

        LookToPlayersClientRpc();

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
}
