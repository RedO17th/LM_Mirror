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

    [Client]
    void Update()
    {
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

    [Client]
    private void FixedUpdate()
    {
        CmdSendInputDirectionForMove(_inputDirection);
    }

    [Command]
    private void CmdSendInputDirectionForMove(Vector3 inputDirection)
    {
        _movable.MoveByDirection(inputDirection);
    }
}
