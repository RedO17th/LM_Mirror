using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { None = -1, Red, Blue, Yellow }

public class PlayersManager : BaseGameManager
{
    [SerializeField] private BasePlayer[] _players = null;

    public event Action<IPlayer> OnPlayerSpawned;

    public override void Initialize() { }

    public override void Prepare() { }
    public override void Activate()
    {
        foreach (var player in _players)
        {
            OnPlayerSpawned?.Invoke(player);
        }
    }
} 
 