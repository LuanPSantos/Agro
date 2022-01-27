using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;


public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton;

    public event Action<ulong, ulong> PlayersLoaded;

    private const string gameplayScene = "Gameplay";
    void Awake()
    {
        StartSingleton();
        DontDestroyOnLoad(gameObject);

        LobbyManager.Singleton.PlayersConnected += PlayersConnectedHandle;    
    }

    private void PlayersConnectedHandle(ulong playerOneClientId, ulong playerTwoClientId)
    {
        LoadGamePlayScene();
    }

    private void SceneLoadedHandle(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer || !IsHost) return;

        if(clientsCompleted.Count == 2)
        {
            PlayersLoaded?.Invoke(clientsCompleted[0], clientsCompleted[1]);
        }
    }

    private void LoadGamePlayScene()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoadedHandle;
        NetworkManager.Singleton.SceneManager.LoadScene(gameplayScene, LoadSceneMode.Single);
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
