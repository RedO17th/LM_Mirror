using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : BaseGameManager
{
    [Header("Info components")]
    [SerializeField] private GameObject _infoPanel = null;
    [SerializeField] private TextMeshProUGUI _text = null;

    [Tooltip("Time of showing info panel")]
    [Range(1, 21)]
    [SerializeField] private float _timeDurtion = 0f;

    [Space]
    [SerializeField] private StandaloneInputModule _inputModule = null;
    [SerializeField] private GameObject _miniGamePanel = null;
    [SerializeField] private VariableJoystick _variableJoystick = null;

    public VariableJoystick Joystick => _variableJoystick;

    private float _timeLeft = 0f;

    public override void Prepare() => PrepareForServer();

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnPlayerSpawnAction += ProcessPlayerSpawnAction;

        ProjectBus.OnMiniGameStartAction += ProcessMiniGameStartAction;
        ProjectBus.OnMiniGameFinishAction += ProcessMiniGameFinishAction;

        ProjectBus.OnShowMiniGameInfoAction += ProcessShowInfoAction;

        _variableJoystick.gameObject.SetActive(false);
    }

    private void ProcessPlayerSpawnAction(PlayerSpawnAction action) => RpcActivatInputView();

    [ClientRpc]
    private void RpcActivatInputView()
    {
        _variableJoystick.gameObject.SetActive(true);
    }

    private void ProcessMiniGameStartAction(MiniGameStartByAction action)
    {
        TargetRpcActivateGamePanel(action.PlayerNetConnection);
    }

    [TargetRpc]
    private void TargetRpcActivateGamePanel(NetworkConnection targetConnection)
    {
        _miniGamePanel.SetActive(true);

        _variableJoystick.SetJoystickStartPosition();

        _inputModule.DeactivateModule();
    }

    private void ProcessMiniGameFinishAction(MiniGameFinishAction action)
    {
        TargetRpcDeactivateGamePanel(action.PlayerNetConnection);
    }

    [TargetRpc]
    private void TargetRpcDeactivateGamePanel(NetworkConnection targetConnection)
    {
        _miniGamePanel.SetActive(false);
        _inputModule.ActivateModule();
    }

    private void ProcessShowInfoAction(ShowMiniGameInfoAction action)
    {
        var targetConnection = action.TargetIdentity.connectionToClient;
        var clientIdentity = action.ClientIdentity;

        TargetRpcShowInfoAboutDebuffedPlayers(targetConnection, clientIdentity);
    }

    [TargetRpc]
    private void TargetRpcShowInfoAboutDebuffedPlayers(NetworkConnection targetConnection, NetworkIdentity client)
    {
        var player = client.GetComponent<BasePlayer>();

        SetInfoIntoPanelAboutDebuffedPlayer(player);
    }

    #region InfoPanel

    private void SetInfoIntoPanelAboutDebuffedPlayer(BasePlayer player)
    {
        ActivateInfoPanel();

        if (RecordsIsExisting())
        {
            AddRecord(GetNewLineInfo(player));

            return;
        }

        AddRecord(GetLineInfo(player));    
        StartCoroutine(InfoPanelTimer());
    }

    private void ActivateInfoPanel()
    {
        if (_infoPanel.activeInHierarchy == false)
            _infoPanel.SetActive(true);
    }

    private bool RecordsIsExisting()
    {
        return (_text.text != string.Empty) ? true : false;
    }

    private void AddRecord(string record)
    {
        SetInfoIntoPanel(record);
        SetTimeDurationForInfoPanelTimer();
    }
    private void SetInfoIntoPanel(string info)
    {
        _text.text += info;
    }
    private void SetTimeDurationForInfoPanelTimer()
    {
        _timeLeft = _timeDurtion;
    }

    private string GetNewLineInfo(BasePlayer player)
    {
        return $"\n { player.Name } попался..";
    }

    private string GetLineInfo(BasePlayer player)
    {
        return $"{ player.Name } попался..";
    }

    private IEnumerator InfoPanelTimer()
    {
        while (_timeLeft > 0f)
        {
            _timeLeft -= Time.deltaTime;

            yield return null;
        }

        DeactivateInfoPanel();
    }

    private void DeactivateInfoPanel()
    {
        _infoPanel.SetActive(false);
        _text.text = string.Empty;
    }

    #endregion

    public override void Deactivate()
    {
        ProjectBus.OnPlayerSpawnAction -= ProcessPlayerSpawnAction;

        ProjectBus.OnMiniGameStartAction -= ProcessMiniGameStartAction;
        ProjectBus.OnMiniGameFinishAction -= ProcessMiniGameFinishAction;

        ProjectBus.OnShowMiniGameInfoAction -= ProcessShowInfoAction;
    }
}
