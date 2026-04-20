using Steamworks;
using System;

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
            throw new Exception("ALREADY HAS STEAM ID");
        }
    }

    public void AddNetId(ulong netID)
    {
        tokens++;

        if (NetworkID == default) NetworkID = netID;
        else throw new Exception("ALREADY HAS NET ID");
    }

    public void AddName(string name)
    {
        tokens++;
        if (this.name == default)
        {
            this.name = name;
        }
        else throw new Exception("ALREADY HAS NAME");
    }

    public bool IsComplete()
    {
        return tokens is 3;
    }
}
