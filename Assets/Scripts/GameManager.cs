using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private string m_SceneName;
    public static GameManager instance;

#if UNITY_EDITOR
    [SerializeField] SceneAsset gameScene;

    private void OnValidate()
    {
        if (gameScene != null)
        {
            m_SceneName = gameScene.name;
        }
    }
#endif

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

        GameStatusManager.StartGame();

        InGamePlayers inGamePlayers = TeamLobbyManager.instance.GetIngamePlayers();
        NetworkManager.SceneManager.LoadScene("", LoadSceneMode.Single);

        var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);

        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {m_SceneName} " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }

        // SpawnCarManager.instance.SpawnCars(inGamePlayers);
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

    public static void StartGame()
    {
        SetCurrentStatus(GameStatus.STARTING);
    }
}

public enum GameStatus
{
    NOT_SERVER = -1, //-1
    ON_LOBBY,    //0
    STARTING,   //1
    PLAYING,    //2
    PAUSED,     //3
    ENDED       //4
}