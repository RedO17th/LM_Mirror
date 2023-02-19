using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCollectDebuf : BaseDebaf
{
    [SerializeField] private float _workTime = 5f;

    private CollectableManager _collectableManager = null;

    private Coroutine _timerRoutine = null;

    private PlayerType _targetType = PlayerType.None;


    public override void Initialize(DebufManager manager)
    {
        base.Initialize(manager);

        _collectableManager = GameSystem.GetManager<CollectableManager>();

        _targetType = GetTargetCollectableType();
    }

    private PlayerType GetTargetCollectableType()
    { 
        return _debufManager.MiniGameResult.Collectable.TargetType;
    }

    public override void Activate()
    {
        _collectableManager.DeactivateCollectables(_targetType);
        _timerRoutine = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(_workTime);

        ProcessDeactivation();
    }

    protected override void ProcessDeactivation()
    {
        _collectableManager.ActivateCollectables(_targetType);
        _collectableManager = null;

        _targetType = PlayerType.None;

        Destroy(gameObject);
    }
}
