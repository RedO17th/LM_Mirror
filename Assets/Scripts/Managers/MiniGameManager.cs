using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : BaseGameManager
{
    [SerializeField] private List<BaseMiniGame> _miniGames;

    private List<MiniGameResult> _miniGameResults = null;

    public override void Initialize()
    {
        _miniGameResults = new List<MiniGameResult>();
    }

    public override void Prepare()
    {
        PrepareForServer();
    }

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnCollectItemAction += ProcessAction;
    }

    public override void Activate() { }

    private void ProcessAction(CollectItemAction action) => ProcessCollecting(action.Collectable);
    private void ProcessCollecting(ICollectable collectable)
    {
        OnMiniGameStartEvent();
        RpcActivateMiniGame();
    }

    private void OnMiniGameStartEvent()
    {
        ProjectBus.Instance.SendAction(new MiniGameStartAction());
    }

    [ClientRpc]
    //private void RpcProcessCollecting(ICollectable collectable)
    private void RpcActivateMiniGame()
    {
        var miniGame = ActivateRandomMiniGame();

        //AddMiniGameResult(miniGame, collectable);
    }
    private IMiniGame ActivateRandomMiniGame()
    {
        var miniGame = GetRandomMiniGame();

            miniGame.OnCompleted += ProcessMiniGameCompletion;

            miniGame.Enable();
            miniGame.Activate();

        return miniGame;
    }
    private BaseMiniGame GetRandomMiniGame()
    {
        var randomIndex = UnityEngine.Random.Range(0, _miniGames.Count - 1);

        return _miniGames[randomIndex];
    }

    //private void AddMiniGameResult(IMiniGame game, ICollectable collectable)
    //{
    //    _miniGameResults.Add(new MiniGameResult(game, collectable));
    //}

    [Client]
    private void ProcessMiniGameCompletion(IMiniGame game)
    {
        DeactivateRandomMiniGame(game);
        OnMiniGameFinishedEvent(game);
    }

    private void DeactivateRandomMiniGame(IMiniGame game)
    {
        game.OnCompleted -= ProcessMiniGameCompletion;

        game.Disable();
    }

    private void OnMiniGameFinishedEvent(IMiniGame game)
    {
        var result = GetMiniGameResult(game);

        //_miniGameResults.Remove(result);

        ProjectBus.Instance.SendAction(new MiniGameFinishAction(result));
    }

    private MiniGameResult GetMiniGameResult(IMiniGame game)
    {
        MiniGameResult mnResult = null;

        foreach (var result in _miniGameResults)
        {
            if (result.MiniGame == game)
            {
                mnResult = result;
                break;
            }
        }

        return mnResult;
    }


}

public class MiniGameResult
{
    public IMiniGame MiniGame { get; private set; } = null;
    public ICollectable Collectable { get; private set; } = null;

    public MiniGameResult(IMiniGame game, ICollectable collectable) 
    {
        MiniGame = game;
        Collectable = collectable;
    }
}
