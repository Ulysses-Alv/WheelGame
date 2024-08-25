using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnCarManager : MonoBehaviour
{
    public static SpawnCarManager instance;

    [SerializeField] private NetworkObject carPrefab;

    public NetworkObject frontLeftWheelPrefab;
    public NetworkObject frontRightWheelPrefab;
    public NetworkObject rearLeftWheelPrefab;
    public NetworkObject rearRightWheelPrefab;

    private NetworkObject carInstanceNet;
    [SerializeField] private Transform carPositionOne, carPositionTwo;
    Queue<Transform> carQueue = new();
    [SerializeField] NetworkObject[] wheelPrefabs;
    private List<NetworkObject> wheelInstances = new();

    public void SpawnCars(InGamePlayers inGamePlayers)
    {
        StartCoroutine(SpawnCar());
    }

    private void Awake()
    {
        instance = this;

        carQueue.Enqueue(carPositionOne);
        carQueue.Enqueue(carPositionTwo);
    }

    private IEnumerator SpawnCar()
    {
        yield return DictionaryOfWaitForSeconds.GetWaitForSeconds(10); //ANIMACION DE CAMARA O ALGO ASÍ.

        var carInstance = Instantiate(carPrefab.gameObject);
        carInstanceNet = carInstance.GetComponent<NetworkObject>();
        carInstanceNet.Spawn(true);

        var carPos = carQueue.Dequeue();
        carInstance.transform.SetPositionAndRotation(carPos.position, carPos.rotation);

        AssignOwnership();
    }

    private void AssignOwnership()
    {
        Queue<ulong> clientsIDs = GetClients();

        foreach (var wheelPrefab in wheelPrefabs)
        {
            GameObject instanceWheel = Instantiate(wheelPrefab.gameObject);
            var instanceWheelNetObj = instanceWheel.GetComponent<NetworkObject>();

            if (clientsIDs.TryDequeue(out ulong id))
            {
                instanceWheelNetObj.SpawnWithOwnership(id);
            }
            else
            {
                instanceWheelNetObj.SpawnWithOwnership(0);
            }

            wheelInstances.Add(instanceWheelNetObj);
        }

        UpdateClientWheelsParent();
    }

    private Queue<ulong> GetClients()
    {
        Queue<ulong> result = new();

        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            result.Enqueue(id);
        }

        return result;
    }


    private NetworkObject GetRandomWheel()
    {
        return NetworkManager.Singleton.LocalClient.OwnedObjects[Random.Range(0, NetworkManager.Singleton.LocalClient.OwnedObjects.Count)];
    }

    private void UpdateClientWheelsParent()
    {
        foreach (var wheelInstance in wheelInstances)
        {
            StartCoroutine(DelayedSetParentAndEnable(wheelInstance, carInstanceNet.NetworkObjectId));
        }
    }

    private IEnumerator DelayedSetParentAndEnable(NetworkObject wheelInstance, ulong carNetworkObjectId)
    {
        yield return DictionaryOfWaitForSeconds.GetWaitForSeconds(0.1f); // Delay to ensure all objects are initialized

        if (wheelInstance == null) yield break;

        wheelInstance.GetComponent<WheelControl>().SetParentAndEnableClientRpc(carNetworkObjectId);
    }
}