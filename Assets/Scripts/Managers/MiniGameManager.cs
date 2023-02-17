using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _miniGamePanel = null;
    [SerializeField] private List<BaseMiniGame> _miniGames;

    private BaseMiniGame _currentMiniGame = null;

    private void Start()
    {
        _miniGamePanel.SetActive(true);

        _currentMiniGame = _miniGames[0];

        _currentMiniGame.OnCompleted += ProcessMiniGameComplition;
        _currentMiniGame.Enable();
        _currentMiniGame.Activate();
    }

    private void ProcessMiniGameComplition(IMiniGame game)
    {
        Debug.Log($"MiniGameManager.ProcessMiniGameComplition: is { game.CompletionType } ");

        _currentMiniGame.OnCompleted -= ProcessMiniGameComplition;

        _currentMiniGame.Disable();
        _currentMiniGame = null;

        _miniGamePanel.SetActive(false);
    }
}
