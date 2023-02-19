using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private BaseGameManager[] _baseGameManagers = null;

    public static GameSystem Instance => _instance;
    private static GameSystem _instance = null;

    private void Awake()
    {
        InitializeSingletone();
    }

    private void InitializeSingletone()
    {
        if (_instance == null) _instance = this;
        else
            Destroy(gameObject);
    }

    public static T GetManager<T>() where T : BaseGameManager
    { 
        T manager = null;

        foreach (var m in _instance._baseGameManagers)
        {
            if(m is T)
            {
                manager = m as T; 
                break;
            }
        }

        return manager;
    }

    private void Start()
    {
        InitializeGameManagers();
        PrepareAndActivateGameManagers();
    }

    private void InitializeGameManagers()
    {
        foreach (var manager in _baseGameManagers)
            manager.Initialize();
    }

    private void PrepareAndActivateGameManagers()
    {
        foreach (var manager in _baseGameManagers)
        {
            manager.Prepare();
            manager.Activate();
        }
    }
}

public class BaseGameManager : MonoBehaviour
{
    public virtual void Initialize() { }
    public virtual void Prepare() { }
    public virtual void Activate() { }
}
