using Unity.Netcode;
using UnityEngine;

public class CarControl : NetworkBehaviour
{
    [SerializeField] private float motorTorque = 2000;
    [SerializeField] private float brakeTorque = 2000;
    public float _brakeTorque => brakeTorque;

    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private float steeringRange = 30;
    [SerializeField] private float steeringRangeAtMaxSpeed = 10;
    [SerializeField] private float centreOfGravityOffset = -1f;
    [SerializeField] private float speedFactor;

    [SerializeField] private Rigidbody rigidBody;

    public float currentMotorTorque;
    public float currentSteerRange;
    public float forwardSpeed;

    public GameObject frontLeftWheel;
    public GameObject frontRightWheel;
    public GameObject rearLeftWheel;
    public GameObject rearRightWheel;

    [SerializeField] private NetworkObject[] wheels;

    private NetworkVariable<int> assignedWheelIndex = new NetworkVariable<int>();

    public NetworkObject[] GetWheels()
    {
        return wheels;
    }

    public override void OnNetworkSpawn()
    {
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
    }

    void Update()
    {
        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

        // Calculate how close the car is to top speed
        // as a number from zero to one
        speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);
    }
}
