using UnityEngine;
using UnityEngine.Events;

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
