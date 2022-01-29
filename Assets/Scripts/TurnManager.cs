using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Singleton;
    public ulong numbersOfPlayers = 2;

    private NetworkVariable<ulong> currentPlayerClientId = new NetworkVariable<ulong>();
    private ulong serverOffset = 1;
    void Awake()
    {
        StartSingleton();
    }

    void Start()
    {
        SpawnManager.Singleton.PlayersSpawned += PlayersSpawnedHandle;
        ArrowBehaviour.ArrowCollided += ArrowCollidedHandle;
    }

    private void PlayersSpawnedHandle(ulong playerOne, ulong playerTwo)
    {
        StartCoroutine(StartTurn(playerOne, playerTwo));
    }

    private IEnumerator StartTurn(ulong playerOne, ulong playerTwo)
    {
        if (!IsServer || !IsHost) yield return null;

        yield return new WaitForSeconds(3);

        NetworkLog.LogInfoServer("StartTurn for players with clientId=" + playerOne + " and clientId=" + playerTwo);

        RemovePlayerFromTargetGroupClientRpc(playerTwo);

        SetPlayerTurn(playerOne, true);

        currentPlayerClientId.Value = playerOne;
    }

    private void ArrowCollidedHandle()
    {
        StartCoroutine(SwitchTurn());
    }

    private IEnumerator SwitchTurn()
    {
        if (!IsServer || !IsHost) yield return null;

        yield return new WaitForSeconds(3);

        ulong nextPlayerClientId = GetNextPlayerClientId();
        NetworkLog.LogInfoServer("SwitchTurn " + nextPlayerClientId);

        RemoveArrowToTargetGroupClientRpc();
        AddPlayerFromTargetGroupClientRpc(nextPlayerClientId);

        SetPlayerTurn(currentPlayerClientId.Value, false);
        SetPlayerTurn(nextPlayerClientId, true);

        currentPlayerClientId.Value = nextPlayerClientId;
    }

    private ulong GetNextPlayerClientId()
    {
        if(IsHost)
        {
            return (currentPlayerClientId.Value + 1) % numbersOfPlayers;
        }
        return serverOffset + currentPlayerClientId.Value % numbersOfPlayers;
    }

    [ClientRpc]
    private void RemovePlayerFromTargetGroupClientRpc(ulong clientId)
    {
        CameraManager.Singleton.RemovePlayerFromTargetGroup(clientId);
    }

    [ClientRpc]
    private void AddPlayerFromTargetGroupClientRpc(ulong clientId)
    {
        CameraManager.Singleton.AddPlayerToTargetGroup(clientId);
    }

    [ClientRpc]
    private void RemoveArrowToTargetGroupClientRpc()
    {
        CameraManager.Singleton.RemoveArrowToTargetGroup();
    }

    private void SetPlayerTurn(ulong playerClientId, bool canPlay)
    {
        NetworkManager.Singleton.ConnectedClients[playerClientId]
            .PlayerObject.GetComponent<PlayerController>()
            .SetCanPlay(canPlay);
    }

    private void StartSingleton()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
