using Mirror;
using UnityEngine;

public interface IMovable
{
    void MoveByDirection(Vector3 direction);
    void RotateByDirection(Vector3 direction);
}

public interface IPlayer : IMovable
{
    PlayerType Type { get; }
    Vector3 Position { get; }
}

public class BasePlayer : NetworkBehaviour, IPlayer
{
    [SerializeField] private PlayerType _playerType = PlayerType.None;

    [SerializeField] private float _speedMovement = 5f;
    [SerializeField] private float _speedRotation = 5f;

    public PlayerType Type => _playerType;
    public Vector3 Position => transform.position;

    private Rigidbody _playerRB = null;

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();  
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

    public void MoveByDirection(Vector3 direction)
    {
        _playerRB.velocity = direction * _speedMovement * Time.deltaTime;
    }
}
