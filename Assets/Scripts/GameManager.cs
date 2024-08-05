using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject carPrefab;

    public NetworkObject frontLeftWheelPrefab;
    public NetworkObject frontRightWheelPrefab;
    public NetworkObject rearLeftWheelPrefab;
    public NetworkObject rearRightWheelPrefab;

    private NetworkObject carInstanceNet;

    [SerializeField] NetworkObject[] wheelPrefabs;
    private List<NetworkObject> wheelInstances = new();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this);
        }

    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsServer) Destroy(this);
        if (!GameStatusManager.GetCurrentStatus().Equals(GameStatus.STARTING)) return;

        StartCoroutine(SpawnCar());
    }
    private IEnumerator SpawnCar()
    {
        yield return DictionaryOfWaitForSeconds.GetWaitForSeconds(10);

        var carInstance = Instantiate(carPrefab.gameObject);
        carInstanceNet = carInstance.GetComponent<NetworkObject>();
        carInstanceNet.Spawn(true);

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
public partial class GameManager
{

}

public static class GameStatusManager
{
    private static GameStatus CurrentStatus;

    public static GameStatus GetCurrentStatus()
    {
        if (!NetworkManager.Singleton.IsServer) return GameStatus.NOT_SERVER;

        return CurrentStatus;
    }
    public static void SetCurrentStatus(GameStatus status)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        CurrentStatus = status;
    }
}

public enum GameStatus
{
    NOT_SERVER = -1, //-1
    STARTING,   //0
    PLAYING,    //1
    PAUSED,     //2
    ENDED,      //3
    ON_LOBBY    //4
}