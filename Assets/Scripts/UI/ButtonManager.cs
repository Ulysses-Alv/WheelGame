using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button quit, options, enterFriendLobby;

    [SerializeField] private GameObject optionsPanel;
    void Start()
    {
        enterFriendLobby.onClick.AddListener(() => { SteamFriends.ActivateGameOverlay("Friend"); });
        options.onClick.AddListener(() => { optionsPanel.SetActive(true); });
        quit.onClick.AddListener(() => { Application.Quit(); });
    }
}