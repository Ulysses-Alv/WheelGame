using Netcode.Transports;
using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private const int MaxLobbyMembers = 8;
    public static LobbyManager instance;
    public CSteamID lobbyID { get; private set; }

    private List<CSteamID> playersList = new();

    private CallResult<LobbyCreated_t> m_LobbyCreated;
    private Callback<LobbyEnter_t> m_LobbyEntered;
    private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
    private Callback<GameLobbyJoinRequested_t> m_GameLobbyRequestJoin;
    private Callback<PersonaStateChange_t> m_PersonaStateChange;
    [SerializeField] private SteamNetworkingSocketsTransport steamNetworkingSocketsTransport;

    private LobbyManagerUI lobbyManagerUI;

    private void Awake()
    {
        instance = this;
        lobbyManagerUI = GetComponent<LobbyManagerUI>();
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }

        m_LobbyCreated = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
        m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_GameLobbyRequestJoin = Callback<GameLobbyJoinRequested_t>.Create(OnOverlayTriesToJoin);
        m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaChange);
    }

    public void CreateLobby()
    {
        SteamAPICall_t apiCall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxLobbyMembers);
        m_LobbyCreated.Set(apiCall);
    }

    private void OnLobbyCreated(LobbyCreated_t result, bool bIOFailure)
    {
        if (result.m_eResult != EResult.k_EResultOK || bIOFailure)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        lobbyID = (CSteamID)result.m_ulSteamIDLobby;

        Debug.Log("Lobby created successfully!");
    }

    private void OnOverlayTriesToJoin(GameLobbyJoinRequested_t result)
    {
        Debug.Log("Joining Lobby...");
        SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        Debug.Log("Entered lobby successfully!");

        if (IsLobbyOwner())
        {
            Debug.Log("Host Started");

            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("Client Started");

            steamNetworkingSocketsTransport.ConnectToSteamID = (ulong)SteamMatchmaking.GetLobbyOwner(lobbyID);
            NetworkManager.Singleton.StartClient();
        }

        lobbyManagerUI.OpenLobbyUI(IsLobbyOwner());
    }
    public bool IsLobbyOwner()
    {
        return SteamMatchmaking.GetLobbyOwner(lobbyID).Equals(SteamUser.GetSteamID());
    }
    private void OnLobbyChatUpdate(LobbyChatUpdate_t result)
    {
        lobbyManagerUI.SetPlayersLobbyInfo(lobbyID);
    }
}
