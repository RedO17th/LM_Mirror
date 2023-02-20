using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UIManager : BaseGameManager
{
    [SerializeField] private GameObject _miniGamePanel = null;
    [SerializeField] private VariableJoystick _variableJoystick = null;

    public VariableJoystick Joystick => _variableJoystick;

    public override void Initialize() { }

    public override void Prepare()
    {
        PrepareForServer();
        PrepareForClient();
    }

    [Server]
    private void PrepareForServer()
    {
        //ProjectBus.OnMiniGameStartAction += ProcessAction;
        //ProjectBus.OnMiniGameFinishAction += ProcessAction;

        _variableJoystick.gameObject.SetActive(false);
    }

    [Client]
    private void PrepareForClient()
    {
        _variableJoystick.gameObject.SetActive(true);
    }

    public override void Activate() => base.Activate();

    private void ProcessAction(MiniGameStartAction obj)
    {
        ActivateGamePanel();
    }

    private void ActivateGamePanel() => _miniGamePanel.SetActive(true);

    private void ProcessAction(MiniGameFinishAction obj)
    {
        DeactivateGamePanel();
    }
    private void DeactivateGamePanel() => _miniGamePanel.SetActive(false);


}
