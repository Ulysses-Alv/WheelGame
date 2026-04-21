using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Shared;

public class TeamLobby : MonoBehaviour, IDropHandler
{
    private List<PlayerClient> clients = new();
    public GameObject container;
    public UnityEvent<PlayerClient> OnDropEvent = new UnityEvent<PlayerClient>();

    public void AddPlayer(PlayerClient net_player)
    {
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
            clients.Remove(playerClient);
        }
    }
    public List<PlayerClient> GetPlayerClients()
    {
        return clients;
    }
}