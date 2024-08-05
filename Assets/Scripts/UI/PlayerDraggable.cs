using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI playerText;

    public PlayerClient playerClient { get; private set; }
    private TeamLobby currentTeam;

    public void Initialize(PlayerClient newPlayer, TeamLobby initialTeam)
    {
        playerClient = newPlayer;
        playerText.text = playerClient.name;
        currentTeam = initialTeam;
        SetParent();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(TeamLobbyManager.instance.transform, false);
        playerText.raycastTarget = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    private void SetParent()
    {
        transform.SetParent(currentTeam.container.transform, false);
        transform.SetAsLastSibling();
    }

    public void SetNewTeam(TeamLobby teamLobby)
    {
        currentTeam = teamLobby;
        SetParent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        playerText.raycastTarget = true;
    }
}
