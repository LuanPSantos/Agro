using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Singleton;
    public event Action<ulong, ulong> PlayersSpawned;
    public float minDistanceFromOrigin = 0f;
    public float maxDistanceFromOrigin = 20f;
    public float verticalPosition = 1.4f;

    public GameObject playerPrefab;

    void Awake()
    {
        StartSingleton();      
    }
    void Start()
    {
        GameManager.Singleton.SceneLoadedForPlayers += SceneLoadedForPlayersHandle;
    }

    private void SceneLoadedForPlayersHandle(ulong playerOneClientId, ulong playerTwoClientId)
    {
        float distance = CalculateSpawnDistance();

        SpawnPlayer(playerOneClientId, new Vector2(-distance, verticalPosition), Vector2.one);
        SpawnPlayer(playerTwoClientId, new Vector2(+distance, verticalPosition), Vector2.one);

        PlayersSpawned?.Invoke(playerOneClientId, playerTwoClientId);
    }

    private float CalculateSpawnDistance()
    {
        float distance = UnityEngine.Random.Range(minDistanceFromOrigin, maxDistanceFromOrigin);

        NetworkLog.LogInfoServer("CalculateSpawnPosition distance=" + distance);

        return distance;
    }

    private void SpawnPlayer(ulong clientId, Vector2 position, Vector2 scale)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.GetComponent<PlayerController>().clientId.Value = clientId;

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        
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
