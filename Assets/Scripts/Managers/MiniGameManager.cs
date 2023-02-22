using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : BaseGameManager
{
    [SerializeField] private List<BaseMiniGame> _miniGames;

    public override void Prepare() => PrepareForServer();

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnCollectItemAction += ProcessCollecting;
    }

    public override void Activate() { }

    private void ProcessCollecting(CollectItemAction action) 
    {
        var clientNetIdentity = GetNetworkConnectionFromCollectableTarget(action);

        OnMiniGameStartEvent(clientNetIdentity.connectionToClient);
        TargetRpcActivateMiniGame(clientNetIdentity.connectionToClient, clientNetIdentity);
    }

    private NetworkIdentity GetNetworkConnectionFromCollectableTarget(CollectItemAction action)
    {
        return action.Collectable.Target.Identity;
    }

    private void OnMiniGameStartEvent(NetworkConnection targetConnection)
    {
        ProjectBus.Instance.SendAction(new MiniGameStartByAction(targetConnection));
    }

    [TargetRpc]
    private void TargetRpcActivateMiniGame(NetworkConnection targetConnection, NetworkIdentity identity)
    {
        var miniGame = GetRandomMiniGame();

            miniGame.OnCompleted += ProcessMiniGameCompletion;

            miniGame.SetPlayerIdentity(identity);

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

        CmdOnMiniGameFinishedEvent(game.PlayerIdentity, game.CompletionType);
    }

    private void DeactivateMiniGame(IMiniGame game)
    {
        game.OnCompleted -= ProcessMiniGameCompletion;

        game.Disable();
    }

    [Command(requiresAuthority = false)]
    private void CmdOnMiniGameFinishedEvent(NetworkIdentity identity, MiniGameCompletion type)
    {
        ProjectBus.Instance.SendAction(new MiniGameFinishAction(identity, type));
    }

    public override void Deactivate()
    {
        ProjectBus.OnCollectItemAction -= ProcessCollecting;
    }
}
