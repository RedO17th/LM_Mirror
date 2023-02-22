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

    private void ProcessMiniGameStart(MiniGameStartByAction action)
    {
        StopPlayerByNetConnection(action.PlayerNetConnection);
    }
    private void StopPlayerByNetConnection(NetworkConnection targetConnection)
    {
        foreach (var player in _createdPlayers)
        {
            if (player.Identity.connectionToClient == targetConnection)
            {
                player.StopMovement();
                break;
            }
        }
    }

    private void ProcessMiniGameFinish(MiniGameFinishAction action)
    {
        StartPlayerByNetConnection(action.PlayerNetConnection);
        ShowMiniGameInfo(action);
    }

    private void StartPlayerByNetConnection(NetworkConnection targetConnection)
    {
        foreach (var player in _createdPlayers)
        {
            if (player.Identity.connectionToClient == targetConnection)
            {
                player.StartMovemet();
                break;
            }
        }
    }
    private void ShowMiniGameInfo(MiniGameFinishAction action)
    {
        if (action.CompletionType == MiniGameCompletion.Incorrect)
        {
            ShowInfoAboutDebuffedPlayers(action);
        }
    }

    private void ShowInfoAboutDebuffedPlayers(MiniGameFinishAction action)
    {
        foreach (var anotherPlayer in _createdPlayers)
        {
            if (anotherPlayer.Identity.connectionToClient != action.PlayerNetConnection)
            {
                var targetIdentity = anotherPlayer.Identity;
                var clientIdentity = action.PlayerIdentity;

                ProjectBus.Instance.SendAction(new ShowMiniGameInfoAction(targetIdentity, clientIdentity));
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

        _currentPlayerIndex = -1;
    }
} 
 