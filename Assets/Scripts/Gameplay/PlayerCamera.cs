using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public override void OnNetworkSpawn()
    {
        virtualCamera.gameObject.SetActive(IsOwner);
    }

    private void Update()
    {
        // Ensure the camera remains active/inactive based on ownership
        if (virtualCamera.gameObject.activeSelf != IsOwner)
        {
            virtualCamera.gameObject.SetActive(IsOwner);
        }
    }
}