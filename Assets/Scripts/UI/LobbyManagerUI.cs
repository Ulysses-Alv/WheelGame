using Steamworks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject LobbyMenu;

    [SerializeField] private PlayerListUiManager playerListUiManager;
    [SerializeField] private Button inviteFriend, startGame;
    private void Start()
    {
        inviteFriend.onClick.AddListener(OpenSteamFriendsOverlay);
        startGame.onClick.AddListener(StartGame);
    }

    public void SetPlayersLobbyInfo(CSteamID lobbyID)
    {
        int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        string listOfPlayers = GetListOfPlayers(playerCount, lobbyID);
        int maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

        playerListUiManager.UpdatePlayerCount(playerCount, maxPlayers);
        playerListUiManager.UpdateListPlayer(listOfPlayers);
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

        SteamFriends.ActivateGameOverlayInviteDialog(LobbyManager.instance.lobbyID);
    }
    private void StartGame()
    {
        throw new NotImplementedException();
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
