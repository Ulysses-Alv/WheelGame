using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SpawnCarManager : MonoBehaviour
{
    public static SpawnCarManager instance;

    [SerializeField] private NetworkObject carPrefab;

    [SerializeField] private Transform carPositionOne, carPositionTwo;

    private Queue<Transform> carPositions = new();

    public void SpawnCars(InGamePlayers inGamePlayers)
    {
        StartCoroutine(SpawnCar(inGamePlayers));
    }

    private void Awake()
    {
        instance = this;

        carPositions.Enqueue(carPositionOne);
        carPositions.Enqueue(carPositionTwo);
    }

    private IEnumerator SpawnCar(InGamePlayers inGamePlayers)
    {
        yield return DictionaryOfWaitForSeconds.GetWaitForSeconds(1); //ANIMACION DE CAMARA O ALGO ASÍ.
        InstatiateCar(inGamePlayers.teamA);
        // InstatiateCar(inGamePlayers.teamB);
    }
    private void InstatiateCar(List<PlayerClient> team)
    {
        var carInstance = Instantiate(carPrefab.gameObject);

        carInstance.GetComponent<NetworkObject>().Spawn(true);
        var carPos = carPositions.Dequeue();
        carInstance.transform.SetPositionAndRotation(carPos.position, carPos.rotation);
        carInstance.GetComponent<CarControl>().AssignOwnerShip(team);
    }
}