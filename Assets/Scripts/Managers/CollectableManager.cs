using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : BaseGameManager
{
    [Range(1f, 3f)]
    [SerializeField] private float _collisionDistance = 1.5f;
    [SerializeField] private BaseCollectable[] _collectables;

    public float CollisionDistance => _collisionDistance;

    //��������� � ProjectBus
    private PlayersManager _playersManager = null;
    //

    public override void Initialize()
    {
        _playersManager = GameSystem.GetManager<PlayersManager>();

        DisableCollectable();
    }

    private void DisableCollectable()
    {
        foreach (var collectable in _collectables)
            collectable.Disable();
    }

    public override void Prepare()
    {
        _playersManager.OnPlayerSpawned += ProcessSpawningOfPlayer;
    }

    private void ProcessSpawningOfPlayer(IPlayer player)
    {
        foreach (var collectable in _collectables)
        {
            if (collectable.TargetType == player.Type)
            {
                collectable.OnCollected += ProcessCollectingItem;

                collectable.Initialize(this, player);
                collectable.Enable();
            }
        }
    }

    private void ProcessCollectingItem(ICollectable collectable)
    {
        collectable.OnCollected -= ProcessCollectingItem;
        collectable.Disable();


        ProjectBus.Instance.SendAction(new CollectItemAction(collectable));
    }

    public override void Activate() { }

    public void ActivateCollectables(PlayerType targetType)
    {
        foreach (var collectable in _collectables)
        {
            if (collectable.TargetType == targetType)
                collectable.Activate();
        }
    }

    public void DeactivateCollectables(PlayerType targetType)
    {
        foreach (var collectable in _collectables)
        {
            if (collectable.TargetType == targetType)
                collectable.Deactivate();
        }
    }
}
