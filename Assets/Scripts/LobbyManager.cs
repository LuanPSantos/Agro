using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Singleton;
    public event Action<ulong, ulong> PlayersConnected;

    private NetworkVariable<ulong> playerOneId = new NetworkVariable<ulong>((ulong)0);
    private NetworkVariable<ulong> playerTwoId = new NetworkVariable<ulong>((ulong)0);
    private NetworkVariable<bool> playerOneIsSetted = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> playerTwoIsSetted = new NetworkVariable<bool>(false);

    void Awake()
    {
        StartSingleton();
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += ServerStartedHandle;
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandle;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandle;
    }

    private void ServerStartedHandle()
    {
        NetworkLog.LogInfoServer("Server Started");
    }

    private void ClientConnectedHandle(ulong clientId)
    {
        if (!IsServer || !IsHost) return;

        NetworkLog.LogInfoServer("Client Connected with id = " + clientId);

        if(playerOneIsSetted.Value == false)
        {
            playerOneIsSetted.Value = true;
            playerOneId.Value = clientId;
        }
        else if(playerTwoIsSetted.Value == false)
        {
            playerTwoIsSetted.Value = true;
            playerTwoId.Value = clientId;
        }

        if(playerOneIsSetted.Value && playerTwoIsSetted.Value)
        {
            NetworkLog.LogInfoServer("Players ready to start");
            PlayersConnected?.Invoke(playerOneId.Value, playerTwoId.Value);
        }
    }

    private void ClientDisconnectedHandle(ulong clientId)
    {
        NetworkLog.LogInfoServer("Client Disconnected with id = " + clientId);
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
