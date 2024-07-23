using TMPro;
using UnityEngine;

public class PlayerListUiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerList, playerCount;

    public void UpdateListPlayer(string listOfPlayers)
    {
        playerList.text = listOfPlayers;
    }

    public void UpdatePlayerCount(int playerCount, int maxPlayers)
    {
        this.playerCount.text = $"{playerCount}/{maxPlayers}";
    }
}
