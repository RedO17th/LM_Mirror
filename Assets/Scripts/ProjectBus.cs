using Mirror;
using System;

public class ProjectBus
{
    public static event Action<CollectItemAction> OnCollectItemAction;
    public static event Action<MiniGameStartAction> OnMiniGameStartAction;
    public static event Action<MiniGameFinishAction> OnMiniGameFinishAction;

    public static event Action<ClientConnectAction> OnClientConnectAction;
    public static event Action<ClientDisconnectAction> OnClientDisconnectAction;

    public static event Action<PlayerSpawnAction> OnPlayerSpawnAction;

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

    private ProjectBus() { }
    #endregion

    public void SendAction(CollectItemAction action)
    {
        OnCollectItemAction?.Invoke(action);
    }
    public void SendAction(MiniGameStartAction action)
    {
        OnMiniGameStartAction?.Invoke(action);
    }
    public void SendAction(MiniGameFinishAction action)
    {
        OnMiniGameFinishAction?.Invoke(action);
    }

    public void SendAction(ClientConnectAction action)
    {
        OnClientConnectAction?.Invoke(action);
    }
    public void SendAction(ClientDisconnectAction action)
    {
        OnClientDisconnectAction?.Invoke(action);
    }
    public void SendAction(PlayerSpawnAction action)
    {
        OnPlayerSpawnAction?.Invoke(action);
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

public class MiniGameStartAction { }

public class MiniGameFinishAction
{
    public MiniGameResult Result { get; private set; } = null;

    public MiniGameFinishAction(MiniGameResult result)
    {
        Result = result;
    }
}

public class ClientConnectAction 
{
    public NetworkConnectionToClient Connection { get; private set; } = null;

    public ClientConnectAction(NetworkConnectionToClient connection) 
    {
        Connection = connection;
    }
}

public class ClientDisconnectAction
{
    public ClientDisconnectAction() { }
}

public class PlayerSpawnAction
{
    public IPlayer Player { get; private set; } = null;

    public PlayerSpawnAction(IPlayer player) 
    {
        Player = player;
    }
}
