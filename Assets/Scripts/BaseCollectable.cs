using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface ICollectable
{
    event Action<ICollectable> OnCollected;

    DebufType DebufType { get; }
    IPlayer Target { get; }
    PlayerType TargetType { get; }

    void Enable();
    void Activate();
    void Deactivate();
    void Disable();
}

public class BaseCollectable : NetworkBehaviour, ICollectable
{
    [SerializeField] private DebufType _debufType = DebufType.None;
    [SerializeField] private PlayerType _targetType = PlayerType.None;

    public event Action<ICollectable> OnCollected;

    public DebufType DebufType => _debufType;
    public IPlayer Target => _target;
    public PlayerType TargetType => _targetType;

    protected CollectableManager _manager = null;
    protected IPlayer _target = null;

    public bool _isActivated = true;

    protected float _collisionDistance = 0f;

    private const float POSITIONTRASHOLD = 1f;

    public virtual void Initialize(CollectableManager manager, IPlayer target)
    {
        _manager = manager;
        _target = target;

        _collisionDistance = _manager.CollisionDistance;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void Enable() => gameObject.SetActive(true);

    public void Activate()
    {
        _isActivated = true;
    }

    [Server]
    private void Update()
    {
        if (_isActivated && _target != null)
        {
            CheckCollisionWithTarget();
        }
    }

    protected virtual void CheckCollisionWithTarget()
    {
        if (CheckCollisionDistance())
        {
            OnCollected?.Invoke(this);
        }
    }
    protected virtual bool CheckCollisionDistance()
    {
        var targetPosition = _target != null ? _target.Position 
                                             : Vector3.one * (_collisionDistance + POSITIONTRASHOLD);

        return Vector3.Distance(transform.position, targetPosition) <= _collisionDistance;
    }

    public void Deactivate()
    {
        _isActivated = false;
    }

    [Server]
    public virtual void Disable()
    {
        DisableObject();
        RpcDisableObject();
    }

    private void DisableObject() => gameObject.SetActive(false);

    [ClientRpc]
    private void RpcDisableObject() => DisableObject();
}