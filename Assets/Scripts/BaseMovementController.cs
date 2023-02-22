using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BaseMovementController : NetworkBehaviour
{
    private IMovable _movable = null;

    private VariableJoystick _variableJoystick = null;

    private Vector3 _inputDirection = Vector3.zero;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
    }

    [Client]
    private void Start()
    {
        _variableJoystick = GameSystem.GetManager<UIManager>().Joystick;
    }

    public void Enable() => RpcEnable();

    [ClientRpc]
    private void RpcEnable()
    {
        _inputDirection = Vector3.zero;

        enabled = true;
    }

    public void Disable() => RpcDisable();

    [ClientRpc]
    private void RpcDisable()
    {
        enabled = false;

        _inputDirection = Vector3.zero;
    }

    void Update()
    {
        if(isClient)
            SetInputDirection();
    }

    private void SetInputDirection()
    {
        _inputDirection = new Vector3(_variableJoystick.Horizontal, 0f, _variableJoystick.Vertical);
        
        CmdSendInputDirectionToRotation(_inputDirection);
    }

    [Command]
    private void CmdSendInputDirectionToRotation(Vector3 inputDirection)
    {
        _movable.RotateByDirection(inputDirection);
    }

    private void FixedUpdate()
    {
        if (isClient)
            CmdSendInputDirectionForMove(_inputDirection);
    }

    [Command]
    private void CmdSendInputDirectionForMove(Vector3 inputDirection)
    {
        _movable.MoveByDirection(inputDirection);
    }
}
