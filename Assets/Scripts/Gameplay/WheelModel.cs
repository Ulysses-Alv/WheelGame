using Unity.Netcode;
using UnityEngine;

public class WheelModel : NetworkBehaviour
{
    [SerializeField] private MeshRenderer wheelRenderer;
    protected override void OnOwnershipChanged(ulong previous, ulong current)
    {
        Debug.Log($"{gameObject.name}: is now for {current}");
        ChangeColorClientRpc(current);
    }

    [ClientRpc]
    void ChangeColorClientRpc(ulong current)
    {
        wheelRenderer.material.color = current switch
        {
            0 => Color.white,
            1 => Color.green,
            2 => Color.blue,
            _ => Color.black,
        };
    }
}