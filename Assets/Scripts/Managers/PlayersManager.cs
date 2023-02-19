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

    public override void Prepare() 
    {
        ProjectBus.OnMiniGameFinishAction += ProcessAction;
    }

    public override void Activate()
    {
        foreach (var player in _players)
        {
            OnPlayerSpawned?.Invoke(player);
        }
    }

    private void ProcessAction(MiniGameFinishAction action) => ProcessMiniGameFinishing(action.Result);
    private void ProcessMiniGameFinishing(MiniGameResult result)
    {
        Debug.Log($"PlayersManager.ProcessMiniGameFinishing:");
        Debug.Log($"Mini game is { result.MiniGame.GetType() } ");
        Debug.Log($"From { result.Collectable.GetType() } with result = { result.MiniGame.CompletionType } ");
    }
} 
 