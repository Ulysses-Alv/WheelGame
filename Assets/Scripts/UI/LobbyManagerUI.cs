using Steamworks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject LobbyMenu;

    [SerializeField] private Button inviteFriend, startGame, configuration;

    private void Start()
    {
        configuration.onClick.AddListener(OpenConfigurationMenu);
        inviteFriend.onClick.AddListener(OpenSteamFriendsOverlay);
        startGame.onClick.AddListener(StartGame);
    }

    private void OpenConfigurationMenu()
    {
        throw new NotImplementedException();
    }

    public void SetPlayersLobbyInfo(CSteamID lobbyID)
    {
        int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        string listOfPlayers = GetListOfPlayers(playerCount, lobbyID);
        int maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

    }

    public void OpenLobbyUI(bool isOwner)
    {
        LobbyMenu.SetActive(true);

        startGame.gameObject.SetActive(isOwner);
        inviteFriend.gameObject.SetActive(isOwner);
    }

    private void OpenSteamFriendsOverlay()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }
        Debug.Log("Opening Overlay");
        SteamFriends.ActivateGameOverlayInviteDialog(LobbyManager.instance.LobbyID);
    }

    private void StartGame()
    {
        Debug.Log("TODAVIA NO SE INICIA");
    }

    private string GetListOfPlayers(int memberCount, CSteamID lobbyID)
    {
        string listOfPlayers = "";
        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            string memberName = SteamFriends.GetFriendPersonaName(memberID);
            listOfPlayers += $"{memberName}\n";
        }

        return listOfPlayers;
    }
}
