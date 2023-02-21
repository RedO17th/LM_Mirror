using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : BaseGameManager
{
    [SerializeField] private StandaloneInputModule _inputModule = null;
    [SerializeField] private GameObject _miniGamePanel = null;
    [SerializeField] private VariableJoystick _variableJoystick = null;

    public VariableJoystick Joystick => _variableJoystick;

    public override void Initialize() { }

    public override void Prepare() => PrepareForServer();

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnPlayerSpawnAction += ProcessPlayerSpawnAction;

        ProjectBus.OnMiniGameStartAction += ProcessMiniGameStartAction;
        ProjectBus.OnMiniGameFinishAction += ProcessMiniGameFinishAction;

        _variableJoystick.gameObject.SetActive(false);
    }

    public override void Activate() => base.Activate();

    private void ProcessPlayerSpawnAction(PlayerSpawnAction action) => RpcActivatInputView();

    [ClientRpc]
    private void RpcActivatInputView()
    {
        _variableJoystick.gameObject.SetActive(true);
    }

    private void ProcessMiniGameStartAction(MiniGameStartAction obj) => RpcActivateGamePanel();

    [ClientRpc]
    private void RpcActivateGamePanel() 
    {
        _miniGamePanel.SetActive(true);

        _variableJoystick.SetJoystickStartPosition();

        _inputModule.DeactivateModule();
    }

    private void ProcessMiniGameFinishAction(MiniGameFinishAction obj) => RpcDeactivateGamePanel();

    [ClientRpc]
    private void RpcDeactivateGamePanel()
    {
        _miniGamePanel.SetActive(false);
        _inputModule.ActivateModule();
    }

    public override void Deactivate()
    {
        ProjectBus.OnPlayerSpawnAction -= ProcessPlayerSpawnAction;

        ProjectBus.OnMiniGameStartAction -= ProcessMiniGameStartAction;
        ProjectBus.OnMiniGameFinishAction -= ProcessMiniGameFinishAction;
    }
}
