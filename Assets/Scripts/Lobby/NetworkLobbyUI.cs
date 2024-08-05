using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkLobbyUI : MonoBehaviour
{
    private TextMeshProUGUI thisText;

    private void Awake()
    {
        thisText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ShowClientText;
        NetworkManager.Singleton.OnServerStarted += ShowServerText;
    }

    private void ShowClientText(ulong id)
    {
        string text = $"new Client connected with id: {id}";

        Debug.Log(text);
        thisText.text = text;
    }

    private void ShowServerText()
    {
        string serverStarted = "Server Started";
        Debug.Log(serverStarted);

        thisText.text += '\n' + serverStarted;
    }
}