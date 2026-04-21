using UnityEngine;
using UnityEngine.Events;
using Shared;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private CheckPoint _lineaDeLlegada;
    public CheckPoint LineaDeLlegada => _lineaDeLlegada;

    void Start()
    {
        if (LineaDeLlegada == null)
        {
            Debug.LogWarning("[TrackManager] No CheckPoint assigned. Use TrackBootstrap or assign manually.");
            return;
        }
        LineaDeLlegada.onTriggerEnterAddListener(WinGame);
    }

    private void WinGame(Collider car)
    {
        var winnerTeam = car.gameObject.GetComponentInParent<CarControl>()._carTeam;
        GameManager.instance.WinGame(winnerTeam);
    }
}
