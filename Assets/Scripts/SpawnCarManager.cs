using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Shared;

public class SpawnCarManager : MonoBehaviour
{
    public static SpawnCarManager instance;

    [SerializeField] private NetworkObject carPrefab;

    [SerializeField] private Transform carPositionOne, carPositionTwo;

    private Queue<Transform> carPositions = new();

    public void SpawnCars(InGamePlayers inGamePlayers)
    {
        InstatiateCar(inGamePlayers.teamA, CarTeam.TeamA);
        // InstatiateCar(inGamePlayers.teamB)
    }

    private void Awake()
    {
        instance = this;

        carPositions.Enqueue(carPositionOne);
        carPositions.Enqueue(carPositionTwo);
    }

    private void InstatiateCar(List<PlayerClient> team, CarTeam carTeam)
    {
        Transform carPos = carPositions.Dequeue();

        GameObject carInstance = Instantiate(carPrefab.gameObject, carPos.position, carPos.rotation);
        carInstance.GetComponent<NetworkObject>().Spawn(true);

        carInstance.GetComponent<CarControl>().AssignOwnerShip(team, carTeam);
    }
}