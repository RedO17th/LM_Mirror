using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : BaseGameManager
{
    [SerializeField] private List<BaseMiniGame> _miniGames;

    public override void Initialize() { }

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

    private void ProcessAction(CollectItemAction action) => ProcessCollecting();
    private void ProcessCollecting()
    {
        OnMiniGameStartEvent();
        RpcActivateMiniGame();
    }

    private void OnMiniGameStartEvent()
    {
        ProjectBus.Instance.SendAction(new MiniGameStartAction());
    }

    [ClientRpc]
    private void RpcActivateMiniGame()
    {
        var miniGame = GetRandomMiniGame();

            miniGame.OnCompleted += ProcessMiniGameCompletion;

            miniGame.Enable();
            miniGame.Activate();
    }

    private BaseMiniGame GetRandomMiniGame()
    {
        var randomIndex = UnityEngine.Random.Range(0, _miniGames.Count - 1);

        return _miniGames[randomIndex];
    }

    [Client]
    private void ProcessMiniGameCompletion(IMiniGame game)
    {
        DeactivateMiniGame(game);

        CmdOnMiniGameFinishedEvent(game.CompletionType);
    }

    private void DeactivateMiniGame(IMiniGame game)
    {
        game.OnCompleted -= ProcessMiniGameCompletion;

        game.Disable();
    }

    [Command(requiresAuthority = false)]
    private void CmdOnMiniGameFinishedEvent(MiniGameCompletion type)
    {
        ProjectBus.Instance.SendAction(new MiniGameFinishAction(type));
    }
}
