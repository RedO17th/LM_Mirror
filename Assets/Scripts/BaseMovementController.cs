using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovementController : MonoBehaviour
{
    private IMovable _movable = null;

    private VariableJoystick _variableJoystick = null;

    private Vector3 _inputDirection = Vector3.zero;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();  
    }

    private void Start()
    {
        _variableJoystick = GameSystem.GetManager<UIManager>().Joystick;
    }

    void Update()
    {
        _inputDirection = new Vector3(_variableJoystick.Horizontal, 0f, _variableJoystick.Vertical);

        _movable.RotateByDirection(_inputDirection);
    }

    private void FixedUpdate()
    {
        _movable.MoveByDirection(_inputDirection);
    }
}
