using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectBus : MonoBehaviour
{
    public static event Action<CollectItemAction> OnCollectItemAction;
    public static event Action<MiniGameFinishAction> OnMiniGameFinishAction;

    #region Singletone
    public static ProjectBus Instance
    { 
        get
        {
            if (_instance == null)
                _instance = new ProjectBus();

            return _instance;
        }
    }

    private static ProjectBus _instance = null;
    #endregion

    public void SendAction(CollectItemAction action)
    {
        OnCollectItemAction?.Invoke(action);
    }
    public void SendAction(MiniGameFinishAction action)
    {
        OnMiniGameFinishAction?.Invoke(action);
    }

}

public class CollectItemAction
{
    public ICollectable Collectable { get; private set; } = null;

    public CollectItemAction(ICollectable collectable)
    {
        Collectable = collectable;
    }
}

public class MiniGameFinishAction
{
    public MiniGameResult Result { get; private set; } = null;

    public MiniGameFinishAction(MiniGameResult result)
    {
        Result = result;
    }
}