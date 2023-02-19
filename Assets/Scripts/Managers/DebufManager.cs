using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebufType { None = -1, NonCollect }

public class DebufManager : BaseGameManager
{
    [SerializeField] private BaseDebaf[] _debafs = null;

    public MiniGameResult MiniGameResult { get; private set; } = null;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Prepare()
    {
        ProjectBus.OnMiniGameFinishAction += ProcessAction;
    }

    public override void Activate()
    {
        base.Activate();
    }

    private void ProcessAction(MiniGameFinishAction action) => ProcessMiniGameFinishing(action.Result);
    private void ProcessMiniGameFinishing(MiniGameResult result)
    {
        MiniGameResult = result;

        CreateAndActivateDebuf(MiniGameResult.Collectable.DebufType);
    }

    private void CreateAndActivateDebuf(DebufType type)
    {
        var debuf = InstantiateDebuf(type);
            debuf.Initialize(this);
            debuf.Activate();
    }
    private BaseDebaf InstantiateDebuf(DebufType type)
    {
        return Instantiate(GetDebufPrefab(type), transform); 
    }
    private BaseDebaf GetDebufPrefab(DebufType type)
    {
        BaseDebaf result = null;

        foreach (var debuf in _debafs)
        {
            if (debuf.Type == type)
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