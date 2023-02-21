using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectableManager : BaseGameManager
{
    [Range(1, 5)]
    [SerializeField] private int _numberOfCollectable = 3;
    [SerializeField] private Transform _leftUpSpawnCorner = null;
    [SerializeField] private Transform _rightDownSpawnCorner = null;

    [Range(1f, 3f)]
    [SerializeField] private float _collisionDistance = 1.5f;
    [SerializeField] private BaseCollectable[] _collectablePrefabs;

    public float CollisionDistance => _collisionDistance;

    private List<BaseCollectable> _collected;

    public override void Initialize() { }

    public override void Prepare()
    {
        PrepareToServer();
    }

    [Server]
    private void PrepareToServer()
    {
        _collected = new List<BaseCollectable>();

        ProjectBus.OnPlayerSpawnAction += ProcessPlayerSpawnAction;
    }

    private void ProcessPlayerSpawnAction(PlayerSpawnAction action)
    {
        SpawnCollectable(action);
    }

    private void SpawnCollectable(PlayerSpawnAction action)
    {
        var itemPrefab = GetCollectablePrefab(action.Player.Type);

        if (itemPrefab)
        {
            for (int i = 0; i < _numberOfCollectable; i++)
            {
                var item = Instantiate(itemPrefab);

                    item.OnCollected += ProcessCollectingItem;

                    item.Initialize(this, action.Player);
                    item.SetSpawnPosition(GetSpawnPosition());
                    item.Enable();
                    item.Activate();

                _collected.Add(item);

                NetworkServer.Spawn(item.gameObject);
            }
        }
    }

    private BaseCollectable GetCollectablePrefab(PlayerType targetType)
    {
        BaseCollectable result = null;

        foreach (var collectable in _collectablePrefabs)
        {
            if (collectable.TargetType == targetType)
            {
                result = collectable;
                break;
            }
        }

        return result;
    }
    private Vector3 GetSpawnPosition()
    {
        float xPosition = Random.Range(_leftUpSpawnCorner.position.x, _rightDownSpawnCorner.position.x);
        float zPosition = Random.Range(_leftUpSpawnCorner.position.z, _rightDownSpawnCorner.position.z);

        return new Vector3(xPosition, 0f, zPosition);
    }

    private void ProcessCollectingItem(ICollectable collectable)
    {
        collectable.OnCollected -= ProcessCollectingItem;

        collectable.Deactivate();
        collectable.Disable();

        ProjectBus.Instance.SendAction(new CollectItemAction(collectable));
    }

    public override void Activate() { }

    public void ActivateCollectables(PlayerType targetType)
    {
        foreach (var collectable in _collected)
        {
            if (collectable.TargetType == targetType)
                collectable.Activate();
        }
    }

    public void DeactivateCollectables(PlayerType targetType)
    {
        foreach (var collectable in _collected)
        {
            if (collectable.TargetType == targetType)
                collectable.Deactivate();
        }
    }
}
