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

        currentState = State.BETWEEN_TURNS;
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

                    StartCoroutine(WaitAndStartPlayerTurn());

                    currentState = State.WAITING_TO_START_TURN;

                    break;
                }
            case State.WAITING_TO_START_TURN:
                {
                    break;
                }
            case State.BETWEEN_TURNS:
                {
                    return;
                }
            case State.PLAYER_TURN:
                {
                    break;
                }
            case State.END:
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

        currentState = State.PLAYER_TURN;
    }

    private IEnumerator WaitAndFinishTurns(ulong clientIdWinner)
    {
        yield return new WaitForSeconds(3f);

        turnManager.EndTurns(clientIdWinner);
    }

    private void OnPlayerMakeHisMovement()
    {
        if (!IsServer) return;

        turnManager.RemoveCurrentPlayerTurn();

        currentState = State.BETWEEN_TURNS;
    }

    private void OnArrowCollided(ulong clientId, ContactPoint2D contactPoint2D, string tag)
    {
        if (!IsServer) return;

        if (tag == "Player")
        {
            ulong collidedPlayer = clientId == playerOneClientId ? playerTwoClientId : playerOneClientId;

            var playerController = NetworkManager.Singleton
            .ConnectedClients[collidedPlayer]
            .PlayerObject.GetComponent<PlayerNetworkController>();

            playerController.TakeDamage(50);

            if (playerController.IsDead())
            {
                StartCoroutine(WaitAndFinishTurns(clientId));

                currentState = State.END;
            }
            else
            {
                StartCoroutine(WaitAndStartPlayerTurn());

                currentState = State.WAITING_TO_START_TURN; 
            }
            
        } else
        {
            StartCoroutine(WaitAndStartPlayerTurn());

            currentState = State.WAITING_TO_START_TURN;
        }
    }

    public enum State
    {
        START,
        END,
        PLAYER_TURN,
        BETWEEN_TURNS,
        WAITING_TO_START_TURN
    }
}
