using Unity.Netcode;
using UnityEngine;

public class ChangeColor : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        GetComponentInChildren<MeshRenderer>().material.color = GetColor();
    }

    private Color GetColor()
    {
        if (OwnerClientId == 0)
        {
            return Color.red;
        }
        else
        {
            return Color.green;
        }
    }
}
