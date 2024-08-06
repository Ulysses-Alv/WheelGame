using System.Collections.Generic;
using UnityEngine;

public class PlayerDraggablesManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerDraggablePrefab;

    public static PlayerDraggablesManager Instance { get; private set; }

    private List<PlayerDraggable> instancesDraggables = new();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PlayersManager.onClientAdded.AddListener(CreateNewPlayer);
        PlayersManager.onClientRemoved.AddListener(DestroyInstance);
    }

    private void CreateNewPlayer(PlayerClient newPlayer)
    {
        var initialTeam = TeamLobbyManager.instance.AddPlayerRandomly(newPlayer);
        Debug.Log("CREATED");

        var instance = Instantiate(PlayerDraggablePrefab).GetComponent<PlayerDraggable>();
        instancesDraggables.Add(instance);

        instance.Initialize(newPlayer, initialTeam);
    }

    private void DestroyInstance(PlayerClient playerToRemove)
    {
        var instance = instancesDraggables.Find((player) => player.playerClient == playerToRemove);
        Destroy(instance);
    }
}