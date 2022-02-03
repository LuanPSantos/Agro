using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    private int currentPlayerIndex;
    private ulong[] players = new ulong[2];

    public void SetupPlayers(ulong playerOneClientId, ulong playerTwoClientId)
    {
        NetworkLog.LogInfoServer("SetupPlayers P1=" + playerOneClientId + " P2=" + playerTwoClientId);

        players[0] = playerOneClientId;
        players[1] = playerTwoClientId;

        SetEnablePlayerToPlay(players[0], false);
        SetEnablePlayerToPlay(players[1], false);

        currentPlayerIndex = Random.Range(0, 2);
    }

    public void EndTurns(ulong winner)
    {
        NetworkLog.LogInfoServer("EndTurns");

        SetEnablePlayerToPlay(players[0], false);
        SetEnablePlayerToPlay(players[1], false);

        MakeCameraLookToPlayerClientRpc(winner);
    }

    public void SetNextPlayerTurn()
    {
        int nextPlayerIndex = GetNextPlayerIndex();
        NetworkLog.LogInfoServer("P" + players[nextPlayerIndex] + " Turn");

        MakeCameraLookToPlayerClientRpc(players[nextPlayerIndex]);

        SetEnablePlayerToPlay(players[nextPlayerIndex], true);
        SetEnablePlayerColliders(players[nextPlayerIndex], false);
        SetEnablePlayerColliders(players[currentPlayerIndex], true);

        currentPlayerIndex = nextPlayerIndex;
    }

    public void RemoveCurrentPlayerTurn()
    {
        SetEnablePlayerToPlay(players[currentPlayerIndex], false);
    }

    public ulong GetCurrentPlayerCliendId()
    {
        return players[currentPlayerIndex];
    }

    private int GetNextPlayerIndex()
    {
        return (currentPlayerIndex + 1) % players.Length;
    }

    [ClientRpc]
    private void MakeCameraLookToPlayerClientRpc(ulong clientId)
    {
        CameraManager.Singleton.MakeCameraLookToPlayer(clientId);
    }

    private void SetEnablePlayerToPlay(ulong playerClientId, bool enable)
    {
        NetworkManager.Singleton.ConnectedClients[playerClientId]
            .PlayerObject.GetComponent<PlayerNetworkController>()
            .EnablePlayerToPlay(enable);
    }

    private void SetEnablePlayerColliders(ulong playerClientId, bool enable)
    {
        NetworkManager.Singleton.ConnectedClients[playerClientId]
            .PlayerObject.GetComponent<PlayerNetworkController>()
            .EnablePlayerColliders(enable);
    }
}
