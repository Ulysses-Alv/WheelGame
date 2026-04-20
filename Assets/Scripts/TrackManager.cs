using UnityEngine;
using UnityEngine.Events;
using Shared;

public class TrackManager : MonoBehaviour
{
    [SerializeField] CheckPoint LineaDeLlegada;

    void Start()
    {
        LineaDeLlegada.onTriggerEnterAddListener(WinGame);
    }

    private void WinGame(Collider car)
    {
        var winnerTeam = car.gameObject.GetComponentInParent<CarControl>()._carTeam;
        GameManager.instance.WinGame(winnerTeam);
    }
}
