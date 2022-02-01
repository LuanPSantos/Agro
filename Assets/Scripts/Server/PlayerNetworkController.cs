using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerNetworkController : NetworkBehaviour
{
    private BowController bowController;
    private Collider2D[] playerColliders;

    private NetworkVariable<ulong> clientId = new NetworkVariable<ulong>((ulong) 0);
    private NetworkVariable<bool> canPlay = new NetworkVariable<bool>(false);


    void Start()
    {
        playerColliders = GetComponentsInChildren<Collider2D>();
        bowController = GetComponent<BowController>();
    }

    void Update()
    {
        if (canPlay.Value && !bowController.enabled)
        {
            bowController.enabled = true;
            EnablePlayersColliders(false);
        }
        else if(!canPlay.Value && bowController.enabled)
        {
            bowController.enabled = false;
            EnablePlayersColliders(true);
        }
    }

    public void SetCanPlay(bool canPlay)
    {
        if (!IsServer) return;

        this.canPlay.Value = canPlay;
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            NetworkLog.LogInfoServer("OnNetworkSpawn id=" + clientId.Value);
            CameraManager.Singleton.AddPlayerTransform(clientId.Value, transform);
            CameraManager.Singleton.AddPlayerToTargetGroup(clientId.Value);
        }
    }

    public void SetClientId(ulong clientId)
    {
        this.clientId.Value = clientId;
    }

    private void EnablePlayersColliders(bool enabled)
    {
        foreach(Collider2D col in playerColliders)
        {
            col.enabled = enabled;
        }
    }
}
