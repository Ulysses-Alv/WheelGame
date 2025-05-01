using UnityEngine;
using UnityEngine.Events;

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

public class CheckPoint : MonoBehaviour
{
    UnityEvent<Collider> onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
    public void onTriggerEnterAddListener(UnityAction<Collider> callback)
    {
        onTriggerEnter.AddListener(callback);
    }
}