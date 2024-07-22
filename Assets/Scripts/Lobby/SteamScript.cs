using Steamworks;
using UnityEngine;

public class SteamScript : MonoBehaviour
{

    private const int MaxLobbyMembers = 4;
    private CSteamID currentLobbyID;

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

        //CreateLobby();
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MaxLobbyMembers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        currentLobbyID = (CSteamID)result.m_ulSteamIDLobby;
        Debug.Log("Lobby created successfully!");
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        currentLobbyID = (CSteamID)result.m_ulSteamIDLobby;
        Debug.Log("Entered lobby successfully!");

        if (SteamMatchmaking.GetLobbyOwner(currentLobbyID) == SteamUser.GetSteamID())
        {
            //NetworkManager.Singleton.StartServer();
        }
        else
        {
            // NetworkManager.Singleton.StartClient();
        }
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t result)
    {
        // Check if the lobby is full or if any change occurs in the lobby members
        int numMembers = SteamMatchmaking.GetNumLobbyMembers(currentLobbyID);
        Debug.Log("Number of members in the lobby: " + numMembers);
    }

    public int GetLobbyMemberCount()
    {
        if (currentLobbyID != CSteamID.Nil)
        {
            return SteamMatchmaking.GetNumLobbyMembers(currentLobbyID);
        }
        else
        {
            Debug.LogError("Not currently in a lobby.");
            return 0;
        }
    }
}