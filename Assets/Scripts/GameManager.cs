using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shared;

public class GameManager : NetworkBehaviour
{
    private string m_SceneName;
    public static GameManager instance;

    private NetworkVariable<CarTeam> winningTeam = new();

    public static event Action<CarTeam> OnGameEnded;

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
        if (!NetworkManager.Singleton.IsServer ||
             !GameStatusManager.GetCurrentStatus().Equals(GameStatus.ON_LOBBY)) return;

        GameStatusManager.SetCurrentStatus(GameStatus.STARTING);
        GameStatusManager.ChangeStatus(true);

        InGamePlayers inGamePlayers = TeamLobbyManager.instance.GetIngamePlayers();

        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;

        var status = NetworkManager.Singleton.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);

        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {m_SceneName} " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");

            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;

        // Transition from STARTING to PLAYING once scene is loaded
        GameStatusManager.SetCurrentStatus(GameStatus.PLAYING);

        InGamePlayers inGamePlayers = TeamLobbyManager.instance.GetIngamePlayers();
        SpawnCarManager.instance.SpawnCars(inGamePlayers);
    }

    public void PauseGame()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        GameStatusManager.SetCurrentStatus(GameStatus.PAUSED);
        GameStatusManager.ChangeStatus(false);
    }

    public void EndGame()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        GameStatusManager.SetCurrentStatus(GameStatus.ENDED);
        GameStatusManager.ChangeStatus(false);
    }

    internal void WinGame(CarTeam winnerTeam)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (GameStatusManager.GetCurrentStatus() != GameStatus.PLAYING) return;

        EndGame();

        winningTeam.Value = winnerTeam;
        OnGameEnded?.Invoke(winnerTeam);

        BroadcastWinnerClientRpc(winnerTeam);
    }

    [ClientRpc]
    private void BroadcastWinnerClientRpc(CarTeam winnerTeam)
    {
        if (NetworkManager.Singleton.IsServer) return;

        winningTeam.Value = winnerTeam;
        OnGameEnded?.Invoke(winnerTeam);

        Debug.Log($"[Client] Game Over! Winner: {winnerTeam}");
    }
}
