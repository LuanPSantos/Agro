using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowNetworkSpawner : NetworkBehaviour
{
    public GameObject arrow;

    private BowController bowController;

    private void Awake()
    {
        bowController = GetComponent<BowController>();
    }
    void Start()
    {
        bowController.Fired += FiredHandle;
    }

    private void FiredHandle(float force, Vector3 positon, Vector3 direction, Quaternion rotation)
    {
        FireServerRpc(force, positon, direction, rotation);
    }

    [ServerRpc]
    void FireServerRpc(float force, Vector3 positon, Vector3 direction, Quaternion rotation)
    {
        if (!IsServer) return;

        NetworkLog.LogInfoServer("FireServerRpc");

        GameObject spawnedArrow = Instantiate(arrow, positon, rotation);
        spawnedArrow.GetComponent<NetworkObject>().Spawn();

        spawnedArrow.GetComponent<Rigidbody2D>().AddForce(direction * force);
    }
}
