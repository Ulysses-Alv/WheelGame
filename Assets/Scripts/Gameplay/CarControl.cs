using Player.Movement;
using System.Collections.Generic;
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

    private WheelsIDs wheelsIDs;

    [SerializeField] private PlayerMovement[] wheels;

    private NetworkVariable<int> assignedWheelIndex = new();


    public override void OnNetworkSpawn()
    {
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        wheelsIDs = new(GetInstanceID());
    }

    void Update()
    {
        forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);
        currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);
    }

    internal void AssignOwnerShip(List<PlayerClient> team)
    {
        if (!IsServer) return;

        Queue<PlayerClient> queue = new(team);
        PlayerClient previous = null;

        foreach (var wheel in wheels)
        {
            if (queue.TryDequeue(out PlayerClient client))
            {
                wheel.AssignOwnerClientRpc(client.NetworkID);
                previous = client;
            }
            else
            {
                // wheel.AssignOwnerClientRpc(previous.NetworkID);
            }
        }
    }
}

public struct WheelsIDs
{
    int F_LeftId;
    int F_RightId;
    int B_LeftId;
    int B_RightId;

    public WheelsIDs(int instanceID)
    {
        var instanceString = instanceID.ToString();

        F_LeftId = int.Parse(instanceString + 1.ToString());
        F_RightId = int.Parse(instanceString + 2.ToString());
        B_LeftId = int.Parse(instanceString + 3.ToString());
        B_RightId = int.Parse(instanceString + 4.ToString());
    }
}