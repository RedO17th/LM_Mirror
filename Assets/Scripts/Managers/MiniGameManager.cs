using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : BaseGameManager
{
    [SerializeField] private GameObject _miniGamePanel = null;
    [SerializeField] private List<BaseMiniGame> _miniGames;

    public event Action OnMiniGameStarted;
    public event Action<MiniGameResult> OnMiniGameFinished;

    private CollectableManager _collectableManager = null;

    private List<MiniGameResult> _miniGameResults = null;

    public override void Initialize()
    {
        _collectableManager = GameSystem.GetManager<CollectableManager>();

        _miniGameResults = new List<MiniGameResult>();
    }

    public override void Prepare()
    {
        _collectableManager.OnItemCollected += ProcessCollecting;
    }

    public override void Activate() { }

    private void ProcessCollecting(ICollectable collectable)
    {
        _miniGamePanel.SetActive(true);

        var miniGame = ActivateRandomMiniGame();

        AddMiniGameResult(miniGame, collectable);
    }

    private IMiniGame ActivateRandomMiniGame()
    {
        var miniGame = GetRandomMiniGame();

            miniGame.OnCompleted += ProcessMiniGameComplition;

            miniGame.Enable();
            miniGame.Activate();

        return miniGame;
    }

    private BaseMiniGame GetRandomMiniGame()
    {
        var randomIndex = UnityEngine.Random.Range(0, _miniGames.Count - 1);

        return _miniGames[randomIndex];
    }

    private void AddMiniGameResult(IMiniGame game, ICollectable collectable)
    {
        _miniGameResults.Add(new MiniGameResult(game, collectable));
    }

    private void ProcessMiniGameComplition(IMiniGame game)
    {
        OnMiniGameFinishedEvent(game);
        DeactivateRandomMiniGame(game);

        _miniGamePanel.SetActive(false);
    }

    private void OnMiniGameFinishedEvent(IMiniGame game)
    {
        var result = GetMiniGameResult(game);

        _miniGameResults.Remove(result);

        OnMiniGameFinished?.Invoke(result);
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

    private void DeactivateRandomMiniGame(IMiniGame game)
    {
        game.OnCompleted -= ProcessMiniGameComplition;

        game.Disable();
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
