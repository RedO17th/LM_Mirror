using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerType { None = -1, Red, Blue, Yellow }

public class PlayersManager : BaseGameManager
{
    [SerializeField] private Transform _leftUpSpawnCorner = null;
    [SerializeField] private Transform _rightDownSpawnCorner = null;

    [SerializeField] private BasePlayer[] _players = null;

    private List<BasePlayer> _createdPlayers;
    
    private int _currentPlayerIndex = -1;

    public override void Initialize()
    {
        _createdPlayers = new List<BasePlayer>();
    }

    public override void Prepare() => PrepareForServer();

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnClientConnectAction += ProcessConnectAction;
        ProjectBus.OnClientDisconnectAction  += ProcessDisconnectAction;

        ProjectBus.OnMiniGameStartAction += ProcessMiniGameStart;
        ProjectBus.OnMiniGameFinishAction += ProcessMiniGameFinish;
    }

    public override void Activate() { }

    //Connect
    private void ProcessConnectAction(ClientConnectAction action)
    {
        if (_currentPlayerIndex == _players.Length - 1) return;

        CreateNetworkPlayer(action);
    }

    private void CreateNetworkPlayer(ClientConnectAction action)
    {
        _currentPlayerIndex++;

        var prefab = _players[_currentPlayerIndex];

        NetworkClient.RegisterPrefab(prefab.gameObject);

        var player = CreatePlayer(prefab);
            player.SetSpawnPosition(GetSpawnPosition());

            _createdPlayers.Add(player);

        NetworkServer.AddPlayerForConnection(action.Connection, player.gameObject);

        ProjectBus.Instance.SendAction(new PlayerSpawnAction(player));
    }

    private BasePlayer CreatePlayer(BasePlayer prefab)
    {
        return Instantiate(prefab);
    }
    private Vector3 GetSpawnPosition()
    {
        float xPosition = Random.Range(_leftUpSpawnCorner.position.x, _rightDownSpawnCorner.position.x);
        float zPosition = Random.Range(_leftUpSpawnCorner.position.z, _rightDownSpawnCorner.position.z);

        return new Vector3(xPosition, 0f, zPosition);
    }

    private void ProcessMiniGameStart(MiniGameStartAction obj) => StopPlayer();
    private void StopPlayer()
    {
        Debug.Log($"PlayersManager.StopPlayer");

        foreach (var player in _createdPlayers)
        {
            Debug.Log($"Player is {player.gameObject.name}");

            if (player.isLocalPlayer)
            {
                Debug.Log($"isLocalPlayer is { player.gameObject.name }");

                player.StopMovement();
                break;
            }
        }
    }

    private void ProcessMiniGameFinish(MiniGameFinishAction obj) => StartPlayer();
    private void StartPlayer()
    {
        Debug.Log($"PlayersManager.StartPlayer");

        foreach (var player in _createdPlayers)
        {
            Debug.Log($"Player is {player.gameObject.name}");

            if (player.isLocalPlayer)
            {
                Debug.Log($"isLocalPlayer is {player.gameObject.name}");

                player.StartMovemet();
                break;
            }
        }
    }

    //Disconnect (template)
    private void ProcessDisconnectAction(ClientDisconnectAction obj)
    {
        _currentPlayerIndex--;
    }

    public override void Deactivate()
    {
        ProjectBus.OnClientConnectAction -= ProcessConnectAction;
        ProjectBus.OnClientDisconnectAction -= ProcessDisconnectAction;

        ProjectBus.OnMiniGameStartAction -= ProcessMiniGameStart;
        ProjectBus.OnMiniGameFinishAction -= ProcessMiniGameFinish;
    }
} 
 