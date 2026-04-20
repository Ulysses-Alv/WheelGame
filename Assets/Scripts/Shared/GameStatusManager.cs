using Unity.Netcode;

public static class GameStatusManager
{
    private static GameStatus CurrentStatus;
    private static bool _canVehiclesMove = false;

    public static bool canVehiclesMove => _canVehiclesMove;

    public static GameStatus GetCurrentStatus()
    {
        if (!NetworkManager.Singleton.IsServer) return GameStatus.NOT_SERVER;

        return CurrentStatus;
    }
    public static void SetCurrentStatus(GameStatus status)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        CurrentStatus = status;
    }
    public static void ChangeStatus(bool newStatus)
    {
        _canVehiclesMove = newStatus;
    }
    public static void ChangeStatus()
    {
        _canVehiclesMove = !_canVehiclesMove;
    }

    public static void StartGame()
    {
        SetCurrentStatus(GameStatus.STARTING);
    }
}
