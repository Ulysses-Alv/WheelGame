using Unity.Netcode;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        SetInstance();
    }

    private void SetInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(this);
        }
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsServer) Destroy(this);
        if (!GameStatusManager.GetCurrentStatus().Equals(GameStatus.STARTING)) return;

        InGamePlayers inGamePlayers = TeamLobbyManager.instance.GetIngamePlayers();

        SpawnCarManager.instance.SpawnCars(inGamePlayers);
    }
    public void PauseGame()
    {

    }
    public void EndGame()
    {

    }
}

public static class GameStatusManager
{
    private static GameStatus CurrentStatus;

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
}

public enum GameStatus
{
    NOT_SERVER = -1, //-1
    STARTING,   //0
    PLAYING,    //1
    PAUSED,     //2
    ENDED,      //3
    ON_LOBBY    //4
}