using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameManager : NetworkBehaviour
{
    public virtual void Initialize() { }
    public virtual void Prepare() { }
    public virtual void Activate() { }
}

public class GameSystem : NetworkManager
{
    [SerializeField] private BaseGameManager[] _baseGameManagers = null;

    public static GameSystem Instance => _instance;
    private static GameSystem _instance = null;

    public override void Awake()
    {
        base.Awake();

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

    public override void OnStartServer()
    {
        Debug.Log($"GameSystem.OnStartServer");

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

    public override void OnStartClient()
    {
        InitializeGameManagers();
        PrepareAndActivateGameManagers();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        ProjectBus.Instance.SendAction(new ClientConnectAction(conn));
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        ProjectBus.Instance.SendAction(new ClientDisconnectAction());
    }


}
