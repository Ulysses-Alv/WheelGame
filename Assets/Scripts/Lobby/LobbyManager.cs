using Netcode.Transports;
using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private const int MaxLobbyMembers = 8;
    public static LobbyManager instance;
    public CSteamID LobbyID { get; private set; }
    public CSteamID firstTeamLobbyID { get; private set; }
    public CSteamID secondTeamLobbyID { get; private set; }

    private CallResult<LobbyCreated_t> m_mainLobbyCreated;
    private Callback<LobbyEnter_t> m_LobbyEntered;
    private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
    private Callback<GameLobbyJoinRequested_t> m_GameLobbyRequestJoin;

    [SerializeField] private SteamNetworkingSocketsTransport steamNetworkingSocketsTransport;

    private List<string> playerNames;

    private LobbyManagerUI lobbyManagerUI;

    private void Awake()
    {
        SetInstance();
        DontDestroyOnLoad(gameObject);

        lobbyManagerUI = GetComponent<LobbyManagerUI>();
    }

    private void SetInstance()
    {
        switch (instance)
        {
            case null:
                instance = this;
                break;
            default:
                if (instance != null && instance != this)
                {
                    Destroy(this);
                }

                break;
        }
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }
        NetworkManager.Singleton.OnClientConnectedCallback += AddClientId;

        m_mainLobbyCreated = CallResult<LobbyCreated_t>.Create(OnMainLobbyCreated);
        m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_GameLobbyRequestJoin = Callback<GameLobbyJoinRequested_t>.Create(OnOverlayTriesToJoin);
    }



    public void CreateLobby()
    {
        SteamAPICall_t m_lobby = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxLobbyMembers);
        m_mainLobbyCreated.Set(m_lobby);
    }

    private void OnMainLobbyCreated(LobbyCreated_t result, bool bIOFailure)
    {
        if (result.m_eResult != EResult.k_EResultOK || bIOFailure)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        Debug.Log("Host Started");

        NetworkManager.Singleton.StartHost();

        SetLobbyID((CSteamID)result.m_ulSteamIDLobby);

        Debug.Log("Lobby created successfully!");
    }

    private void OnOverlayTriesToJoin(GameLobbyJoinRequested_t result)
    {
        Debug.Log("Joining Lobby...");
        SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        if (result.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            Debug.Log($"FAILED TO ENTER LOBBY WITH ID:{result.m_ulSteamIDLobby}." +
                   $"\n RESULT: {(EChatRoomEnterResponse)result.m_EChatRoomEnterResponse}");
            return;
        }

        lobbyManagerUI.OpenLobbyUI(IsLobbyOwner());

        if (IsLobbyOwner())
        {
            CSteamID steamID = SteamMatchmaking.GetLobbyOwner(LobbyID);

            var playerName = SteamFriends.GetFriendPersonaName(steamID);

            PlayersManager.AddSteamID(steamID);
            PlayersManager.AddName(playerName);
            return;
        }

        Debug.Log("Entered lobby successfully!");
        Debug.Log("Client Started");

        SetLobbyID((CSteamID)result.m_ulSteamIDLobby);

        steamNetworkingSocketsTransport.ConnectToSteamID = (ulong)SteamMatchmaking.GetLobbyOwner(LobbyID);

        NetworkManager.Singleton.StartClient();
    }

    private void SetLobbyID(CSteamID id)
    {
        if (LobbyID != default) return;

        LobbyID = id;
    }

    public bool IsLobbyOwner()
    {
        return SteamMatchmaking.GetLobbyOwner(LobbyID).Equals(SteamUser.GetSteamID()) || NetworkManager.Singleton.IsServer;
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t result)
    {
        if (!IsLobbyOwner())
        {
            Debug.Log("NOT OWNER" + "CHAT");

            return;
        }

        CSteamID steamID = (CSteamID)result.m_ulSteamIDUserChanged;

        var palyerName = SteamFriends.GetFriendPersonaName(steamID);

        PlayersManager.AddSteamID(steamID);
        PlayersManager.AddName(palyerName);
        Debug.Log("LOBBY UPDATE");
        lobbyManagerUI.SetPlayersLobbyInfo(LobbyID);
    }

    private void AddClientId(ulong clientId)
    {
        if (!IsLobbyOwner())
        {
            Debug.Log("NOT OWNER" + "ADDCLIENT");

            return;
        }

        Debug.Log("NEW CLIENT");
        PlayersManager.AddNetID(clientId);
    }

    private void OnGameStarted()
    {
        if (!IsLobbyOwner()) return;

        SteamMatchmaking.SetLobbyJoinable(LobbyID, false);

        if (GameConfigurationManager.instance.IsRandomTeam)
        {
            TeamLobbyManager.instance.ShufflePlayers();
        }
    }
}

public class GameConfigurationManager : MonoBehaviour
{
    public static GameConfigurationManager instance;

    public bool IsRandomTeam { get; private set; }

}