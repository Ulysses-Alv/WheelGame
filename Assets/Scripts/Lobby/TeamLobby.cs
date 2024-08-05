using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TeamLobby : MonoBehaviour, IDropHandler
{
    private List<PlayerClient> clients = new();
    public GameObject container;
    public UnityEvent<PlayerClient> OnDropEvent = new UnityEvent<PlayerClient>();

    public void AddPlayer(PlayerClient net_player)
    {
        Debug.Log($"ADDED PLAYER {net_player.name} TO THIS TEAM {gameObject.name}");
        clients.Add(net_player);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var pdrag = eventData.pointerDrag.GetComponent<PlayerDraggable>();
        OnDropEvent.Invoke(pdrag.playerClient);

        pdrag.SetNewTeam(this);
    }

    public void TryRemovePlayer(PlayerClient playerClient)
    {
        if (clients.Contains(playerClient))
        {
            Debug.Log($"REMOVED PLAYER {playerClient.name} FROM THIS TEAM {gameObject.name}");
            clients.Remove(playerClient);
        }
    }
}