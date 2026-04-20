using System.Collections.Generic;

public struct InGamePlayers
{
    public List<PlayerClient> teamA;
    public List<PlayerClient> teamB;
    public List<PlayerClient> spects;

    public InGamePlayers(List<PlayerClient> teamA, List<PlayerClient> teamB)
    {
        this.teamA = teamA;
        this.teamB = teamB;
        spects = new List<PlayerClient>();
    }

    public InGamePlayers(List<PlayerClient> teamA, List<PlayerClient> teamB, List<PlayerClient> spects) : this(teamA, teamB)
    {
        this.spects = spects;
    }
}
