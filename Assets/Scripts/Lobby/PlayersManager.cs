using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public static class PlayersManager
{
    public readonly static List<PlayerClient> players = new();
    private static PlayerClient incompleteClient = null;

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
            players.Add(incompleteClient);
            PlayerDraggablesManager.Instance.CreateNewPlayer(incompleteClient);
            incompleteClient = null;
        }
    }
}


public class PlayerClient
{
    public CSteamID steamID { get; private set; }
    public string name { get; private set; }
    public ulong NetworkID { get; private set; }

    private int tokens;


    public void ADDSteamId(CSteamID steamID)
    {
        tokens++;
        if (this.steamID == default)
            this.steamID = steamID;

        else
        {
            throw new System.Exception("ALREADY HAS STEAM ID");
        }
    }

    public void AddNetId(ulong netID)
    {
        tokens++;

        if (NetworkID == default) NetworkID = netID;
        else throw new System.Exception("ALREADY HAS NET ID");
    }

    public void AddName(string name)
    {
        tokens++;
        if (this.name == default)
        {
            this.name = name;

        }
        else throw new System.Exception("ALREADY HAS NAME");
    }

    public bool IsComplete()
    {
        return tokens is 3;
    }
}