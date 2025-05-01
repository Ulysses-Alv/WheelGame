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

    [SerializeField] CarControl _carControl;
    public CarControl carControl => _carControl;


    private void FixedUpdate()
    {
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.SetPositionAndRotation(position, rotation);
    }
}
