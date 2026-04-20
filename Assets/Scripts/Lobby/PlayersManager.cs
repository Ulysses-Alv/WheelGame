using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Shared;

public static class PlayersManager
{
    public readonly static List<PlayerClient> playersInGame = new();
    private static PlayerClient incompleteClient = null;
    public static readonly UnityEvent<PlayerClient> onClientAdded = new();
    public static readonly UnityEvent<PlayerClient> onClientRemoved = new();

    public static void AddName(string playerName)
    {
        incompleteClient ??= new PlayerClient();

        incompleteClient.AddName(playerName);
        Debug.Log("added name");

        TryAddToList();
    }
    public static void AddSteamID(CSteamID steamID)
    {
        incompleteClient ??= new PlayerClient();

        incompleteClient.ADDSteamId(steamID);
        Debug.Log("added steam");
        TryAddToList();
    }
    public static void AddNetID(ulong NetworkID)
    {
        incompleteClient ??= new PlayerClient();
        Debug.Log("added net");

        incompleteClient.AddNetId(NetworkID);
        TryAddToList();
    }

    private static void TryAddToList()
    {
        if (incompleteClient.IsComplete())
        {
            playersInGame.Add(incompleteClient);
            onClientAdded.Invoke(incompleteClient);
            incompleteClient = null;
        }
    }

    public static void RemoverPlayerWithSteamID(CSteamID steamID)
    {
        PlayerClient playerToRemove = playersInGame.Find(client => client.steamID == steamID);

        playersInGame.Remove(playerToRemove);
        onClientRemoved.Invoke(playerToRemove);
    }
}
