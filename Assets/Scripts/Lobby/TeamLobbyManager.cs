using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamLobbyManager : MonoBehaviour
{
    public static TeamLobbyManager instance;

    [SerializeField] private TeamLobby FirstTeamLobby, SecondTeamLobby;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        FirstTeamLobby.OnDropEvent.AddListener(AddPlayerInFirstLobby);
        SecondTeamLobby.OnDropEvent.AddListener(AddPlayerInSecondLobby);
    }

    public void ShufflePlayers()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        List<PlayerClient> playersList = PlayersManager.playersInGame;

        Shuffle(playersList);


        int midPoint = playersList.Count / 2;

        for (int i = 0; i < midPoint; i++)
        {
            FirstTeamLobby.AddPlayer(playersList[i]);
        }

        for (int i = midPoint; i < playersList.Count; i++)
        {
            SecondTeamLobby.AddPlayer(playersList[i]);
        }
    }

    public void AddPlayerInFirstLobby(PlayerClient playerClient)
    {
        FirstTeamLobby.AddPlayer(playerClient);
        SecondTeamLobby.TryRemovePlayer(playerClient);
    }

    public void AddPlayerInSecondLobby(PlayerClient playerClient)
    {
        FirstTeamLobby.TryRemovePlayer(playerClient);
        SecondTeamLobby.AddPlayer(playerClient);
    }

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    public TeamLobby AddPlayerRandomly(PlayerClient clientId)
    {
        if (Random.Range(0, 2) is 1)
        {
            FirstTeamLobby.AddPlayer(clientId);
            return FirstTeamLobby;
        }
        else
        {
            SecondTeamLobby.AddPlayer(clientId);
            return SecondTeamLobby;
        }
    }

    public InGamePlayers GetIngamePlayers()
    {
        var firstTeam = FirstTeamLobby.GetPlayerClients();
        var secondTeam = SecondTeamLobby.GetPlayerClients();

        InGamePlayers result = new(firstTeam, secondTeam);

        return result;
    }
}

public struct InGamePlayers
{
    public List<PlayerClient> teamA;
    public List<PlayerClient> teamB;

    public InGamePlayers(List<PlayerClient> teamA, List<PlayerClient> teamB)
    {
        this.teamA = teamA;
        this.teamB = teamB;
    }
}