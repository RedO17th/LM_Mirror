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

    private int _currentPlayerIndex = -1;

    public override void Initialize() { }
    public override void Prepare() 
    {
        PrepareForServer();
    }

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnClientConnectAction += ProcessConnectAction;
        ProjectBus.OnClientDisconnectAction  += ProcessDisconnectAction;
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

        var player = CreatePlayer(_players[_currentPlayerIndex]);

        float xPosition = Random.Range(_leftUpSpawnCorner.position.x, _rightDownSpawnCorner.position.x);
        float zPosition = Random.Range(_leftUpSpawnCorner.position.z, _rightDownSpawnCorner.position.z);

        player.GetComponent<Transform>().position = new Vector3(xPosition, 0f, zPosition);

        NetworkServer.AddPlayerForConnection(action.Connection, player);

        ProjectBus.Instance.SendAction(new PlayerSpawnAction());
    }

    private GameObject CreatePlayer(BasePlayer prefab)
    {
        return Instantiate(prefab).gameObject;
    }

    //Disconnect
    private void ProcessDisconnectAction(ClientDisconnectAction obj)
    {
        _currentPlayerIndex--;
    }
} 
 