using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public State currentState;

    private SpawnManager spawnManager;
    private TurnManager turnManager;

    private ulong playerOneClientId;
    private ulong playerTwoClientId;

    void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
        turnManager = GetComponent<TurnManager>();

        BowController.Fired += OnPlayerMakeHisMovement;
        ArrowBehaviour.ArrowCollided += OnArrowCollided;
    }

    void Start()
    {
        GameplaySceneManager.Singleton.SceneLoadedForPlayers += SceneLoadedForPlayersHandle;

        currentState = State.WAITING_PLAYERS;
    }

    void Update()
    {
        if (!IsServer) return;

        switch(currentState)
        {
            case State.START:
                {
                    spawnManager.SpawnPlayers(playerOneClientId, playerTwoClientId);
                    turnManager.SetupPlayers(playerOneClientId, playerTwoClientId);

                    currentState = State.WAITING_AFTER_STARTED;

                    break;
                }
            case State.WAITING_AFTER_STARTED:
                {
                    StartCoroutine(WaitAndStartPlayerTurn());

                    currentState = State.WAITING_PLAYERS;

                    break;
                }
            case State.END:
                {
                    break;
                }
            case State.WAITING_PLAYERS:
                {
                    break;
                }
        }
    }

    private void SceneLoadedForPlayersHandle(ulong playerOneClientId, ulong playerTwoClientId)
    {
        if (!IsServer) return;

        this.playerOneClientId = playerOneClientId;
        this.playerTwoClientId = playerTwoClientId;

        currentState = State.START;
    }

    private IEnumerator WaitAndStartPlayerTurn()
    {
        yield return new WaitForSeconds(3f);

        turnManager.SetNextPlayerTurn();
    }

    private void OnPlayerMakeHisMovement()
    {
        if (!IsServer) return;

        turnManager.RemoveCurrentPlayerTurn();
    }

    private void OnArrowCollided()
    {
        if (!IsServer) return;
        
        StartCoroutine(WaitAndStartPlayerTurn());

        currentState = State.WAITING_PLAYERS;
    }

    public enum State
    {
        START,
        END,
        WAITING_PLAYERS,
        WAITING_AFTER_STARTED
    }
}
