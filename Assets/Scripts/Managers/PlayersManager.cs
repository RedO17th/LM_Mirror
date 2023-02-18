using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { None = -1, Red, Blue, Yellow }

public class PlayersManager : BaseGameManager
{
    [SerializeField] private BasePlayer[] _players = null;

    public event Action<IPlayer> OnPlayerSpawned;

    private MiniGameManager _miniGameManager = null;

    public override void Initialize() 
    {
        _miniGameManager = GameSystem.GetManager<MiniGameManager>();
    }

    public override void Prepare() 
    {
        _miniGameManager.OnMiniGameFinished += ProcessMiniGameFinishing;
    }

    public override void Activate()
    {
        foreach (var player in _players)
        {
            OnPlayerSpawned?.Invoke(player);
        }
    }

    private void ProcessMiniGameFinishing(MiniGameResult result)
    {
        Debug.Log($"PlayersManager.ProcessMiniGameFinishing:");
        Debug.Log($"Mini game is { result.MiniGame.GetType() } ");
        Debug.Log($"From { result.Collectable.GetType() } with result = { result.MiniGame.CompletionType } ");
    }
} 
 