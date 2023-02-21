using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebufType { None = -1, NonCollect }

public class DebufManager : BaseGameManager
{
    [SerializeField] private BaseDebaf[] _debafs = null;

    public PlayerType TargetPlayerType { get; private set; } = PlayerType.None;  

    private DebufType _necessaryDebufType = DebufType.None;

    public override void Prepare()
    {
        PrepareForServer();
    }

    [Server]
    private void PrepareForServer()
    {
        ProjectBus.OnCollectItemAction += ProcessItemCollectAction;
        ProjectBus.OnMiniGameFinishAction += ProcessMiniGameFinishAction;
    }

    private void ProcessItemCollectAction(CollectItemAction action)
    {
        TargetPlayerType = action.Collectable.TargetType;
        _necessaryDebufType = action.Collectable.DebufType;
    }

    private void ProcessMiniGameFinishAction(MiniGameFinishAction action)
    {
        Debug.Log($"DebufManager.ProcessMiniGameFinishAction: finish type is { action.CompletionType } ");

        ProcessMiniGameCompletion(action.CompletionType);
    }

    private void ProcessMiniGameCompletion(MiniGameCompletion type)
    {
        if (type == MiniGameCompletion.Correct)
        {
            TargetPlayerType = PlayerType.None;
            _necessaryDebufType = DebufType.None;
        }
        else
        {
            CreateAndActivateDebuf();
        }
    }

    private void CreateAndActivateDebuf()
    {
        var debuf = InstantiateDebuf();
            debuf.Initialize(this);
            debuf.Activate();
    }

    private BaseDebaf InstantiateDebuf()
    {
        return Instantiate(GetDebufPrefab(), transform); 
    }
    private BaseDebaf GetDebufPrefab()
    {
        BaseDebaf result = null;

        foreach (var debuf in _debafs)
        {
            if (debuf.Type == _necessaryDebufType)
            {
                result = debuf;
                break;
            }
        }

        return result;
    }
}

public class BaseDebaf : MonoBehaviour
{
    [SerializeField] protected DebufType _debufType = DebufType.None;

    public DebufType Type => _debufType;

    protected DebufManager _debufManager = null;

    public virtual void Initialize(DebufManager manager) { _debufManager = manager; }
    public virtual void Activate() { }
    protected virtual void ProcessDeactivation() { }
}