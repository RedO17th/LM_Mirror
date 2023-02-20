using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum PlayerType { None = -1, Red, Blue, Yellow }

public class PlayersManager : BaseGameManager
{
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
        ProjectBus.OnClientConnectAction += ProcessAction;
    }

    private void ProcessAction(ClientConnectAction action)
    {
        if (_currentPlayerIndex == _players.Length - 1) return;

        _currentPlayerIndex++;

        var prefab = _players[_currentPlayerIndex];

        var player = Instantiate(prefab);

        NetworkServer.AddPlayerForConnection(action.Connection, player.gameObject);
    }

    public override void Activate() { }
} 
 