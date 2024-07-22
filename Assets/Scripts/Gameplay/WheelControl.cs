using Unity.Netcode;
using UnityEngine;

public class WheelControl : NetworkBehaviour
{
    public Transform wheelModel;
    public WheelCollider WheelCollider;

    public bool steerable;
    public bool motorized;

    private Vector3 position;
    private Quaternion rotation;

    public CarControl carControl { get; private set; }

    private void Awake()
    {
        WheelCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.SetPositionAndRotation(position, rotation);
    }

    [ClientRpc]
    public void SetParentAndEnableClientRpc(ulong parent)
    {
        NetworkObject carObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[parent];

        if (carObj == null) return;

        Transform carTransform = carObj.transform;

        transform.SetParent(carTransform);
        transform.localPosition = Vector3.zero;

        WheelCollider.enabled = true;
        carControl = carTransform.GetComponent<CarControl>();

        Debug.Log("WheelCollider enabled and CarControl set.");
    }
}
