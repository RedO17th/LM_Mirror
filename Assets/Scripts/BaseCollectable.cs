using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    event Action<ICollectable> OnCollected;

    uint NetID { get; }

    DebufType DebufType { get; }
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

    public uint NetID => netId;

    public DebufType DebufType => _debufType;
    public PlayerType TargetType => _targetType;

    protected CollectableManager _manager = null;
    protected IPlayer _target = null;

    protected bool _isActivated = true;

    protected float _collisionDistance = 0f;

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
        return Vector3.Distance(transform.position, _target.Position) <= _collisionDistance;
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