using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public interface IPlayer
{
    PlayerType Type { get; }

    Vector3 Position { get; }

    void MoveByDirection(Vector3 direction);
    void RotateByDirection(Vector3 direction);
}

public class BasePlayer : MonoBehaviour, IPlayer
{
    [SerializeField] private PlayerType _playerType = PlayerType.None;

    [SerializeField] private float _speedMovement = 5f;
    [SerializeField] private float _speedRotation = 5f;

    public PlayerType Type => _playerType;
    public Vector3 Position => transform.position;

    float horizontal = 0f;
    float vertical = 0f;
    Vector3 _inputDirection = Vector3.zero;

    private Rigidbody _playerRB = null;

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();  
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(horizontal, 0f, vertical);

        RotateByDirection(_inputDirection);
    }

    public void RotateByDirection(Vector3 direction)
    {
        var directionRotation = new Vector3(direction.x, 0f, direction.z);
        if (directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speedRotation);

            transform.rotation = targetRotation;
        }
    }

    private void FixedUpdate()
    {
        MoveByDirection(_inputDirection);
    }

    public void MoveByDirection(Vector3 direction)
    {
        _playerRB.velocity = direction * _speedMovement * Time.deltaTime;
    }
}
