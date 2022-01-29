using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{
    private BowController bowController;
    private NetworkVariable<bool> canPlay = new NetworkVariable<bool>(false);
    private Collider2D playerCollider;
    public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>((ulong) 0);

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        bowController = GetComponent<BowController>();
    }

    void Update()
    {
        if (IsOwner || IsServer || IsHost)
        {
            if (canPlay.Value)
            {
                bowController.enabled = true;
                playerCollider.enabled = false;
            }
            else
            {
                bowController.enabled = false;
                playerCollider.enabled = true;
            }
        }
    }

    public void SetCanPlay(bool canPlay)
    {
        if (!IsServer || !IsHost) return;

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
}
