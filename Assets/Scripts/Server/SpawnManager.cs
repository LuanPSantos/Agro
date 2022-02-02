using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SpawnManager : NetworkBehaviour
{
    public float minDistanceFromOrigin = 0f;
    public float maxDistanceFromOrigin = 20f;
    public float verticalPosition = 1.4f;

    public GameObject playerPrefab;

    public void SpawnPlayers(ulong playerOneClientId, ulong playerTwoClientId)
    {
        float distance = CalculateSpawnDistance();

        SpawnPlayer(playerOneClientId, new Vector2(-distance, verticalPosition), Vector2.one);
        SpawnPlayer(playerTwoClientId, new Vector2(+distance, verticalPosition), Vector2.one);
    }

    private float CalculateSpawnDistance()
    {
        float distance = UnityEngine.Random.Range(minDistanceFromOrigin, maxDistanceFromOrigin);

        return distance;
    }

    private void SpawnPlayer(ulong clientId, Vector2 position, Vector2 scale)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.GetComponent<PlayerNetworkController>().SetClientId(clientId);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId); 
    }
}
