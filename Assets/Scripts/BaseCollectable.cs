using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    event Action<ICollectable> OnCollected;
    PlayerType TargetType { get; }

    void Enable();
    void Disable();
}

public class BaseCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private PlayerType _targetType = PlayerType.None;

    public event Action<ICollectable> OnCollected;

    public PlayerType TargetType => _targetType;

    protected CollectableManager _manager = null;
    protected IPlayer _target = null;

    protected float _collisionDistance = 0f;
    public virtual void Initialize(CollectableManager manager, IPlayer target)
    {
        _manager = manager;
        _target = target;

        _collisionDistance = _manager.CollisionDistance;
    }

    public virtual void Enable() => gameObject.SetActive(true);
    public virtual void Disable() => gameObject.SetActive(false);

    private void Update() => CheckCollisionWithTarget();

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
}
