using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private const int MaxLobbyMembers = 8;

    protected Callback<LobbyCreated_t> m_LobbyCreated;
    protected Callback<LobbyEnter_t> m_LobbyEntered;
    protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }

        m_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxLobbyMembers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        Debug.Log("Lobby created successfully!");
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        Debug.Log("Entered lobby successfully!");
        // If the player who created the lobby enters, start the server
        if (SteamMatchmaking.GetLobbyOwner((CSteamID)result.m_ulSteamIDLobby) == SteamUser.GetSteamID())
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t result)
    {
        // Check if the lobby is full
        int numMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)result.m_ulSteamIDLobby);
        if (numMembers == MaxLobbyMembers)
        {
            // Start the game if lobby is full
            Debug.Log("Lobby is full, starting the game...");
            // NetworkManager.Singleton.StartHost();
        }
    }
}
