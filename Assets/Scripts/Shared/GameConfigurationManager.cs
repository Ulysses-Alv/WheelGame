using UnityEngine;

public class GameConfigurationManager : MonoBehaviour
{
    public static GameConfigurationManager instance;

    public bool IsRandomTeam { get; private set; }

}
