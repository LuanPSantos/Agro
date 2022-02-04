using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerNetworkController : NetworkBehaviour
{
    public int maxHealth = 100;

    private BowController bowController;
    private Collider2D[] playerColliders;

    private NetworkVariable<ulong> clientId = new NetworkVariable<ulong>((ulong) 0);
    private NetworkVariable<bool> canPlay = new NetworkVariable<bool>(false);
    private NetworkVariable<int> currentHeath = new NetworkVariable<int>();

    void Awake()
    {
        playerColliders = GetComponentsInChildren<Collider2D>();
        bowController = GetComponent<BowController>();
    }
    void Start()
    {
        bowController.enabled = false;
        bowController.SetClientId(clientId.Value);

        EnablePlayerColliders(true);
    }

    void Update()
    {
        bowController.enabled = canPlay.Value;
    }

    public void EnablePlayerToPlay(bool canPlay)
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
 
            NicknameSetter();
        }

       

        if (IsServer)
        {
            currentHeath.Value = maxHealth;
        }
    }

    public void SetClientId(ulong clientId)
    {
        if (!IsServer) return;

        this.clientId.Value = clientId;
    }

    public ulong GetPlayerClientId()
    {
        return clientId.Value;
    }

    public void EnablePlayerColliders(bool enabled)
    {
        foreach(Collider2D col in playerColliders)
        {
            col.enabled = enabled;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHeath.Value -= damage;

        currentHeath.Value = Mathf.Clamp(currentHeath.Value, 0, maxHealth);
    }

    public bool IsDead()
    {
        return currentHeath.Value == 0;
    }

    private void NicknameSetter()
    {
        string nickname = PlayerPrefs.GetString("nickname");

        Debug.Log("==> " + nickname);

        SetPlayerNicknameServerRpc(clientId.Value, nickname);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNicknameServerRpc(ulong cliendId, string nickname)
    {
        if (!IsServer) return;

        PlayerPrefs.SetString(cliendId.ToString(), nickname);
    }
}
