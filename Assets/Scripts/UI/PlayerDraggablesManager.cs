using UnityEngine;

public class PlayerDraggablesManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerDraggablePrefab;

    public static PlayerDraggablesManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNewPlayer(PlayerClient newPlayer)
    {
        var initialTeam = TeamLobbyManager.instance.AddPlayerRandomly(newPlayer);
        Debug.Log("CREATED");
        Instantiate(PlayerDraggablePrefab).
            GetComponent<PlayerDraggable>().
            Initialize(newPlayer, initialTeam);
    }
}