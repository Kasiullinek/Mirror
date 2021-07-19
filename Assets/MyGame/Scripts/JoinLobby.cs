using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobby : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;
    [SerializeField] private Button backButton = null;

    private void OnEnable()
    {
        MyNetworkManager.OnClientConnected += HandleClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleClientDiconnected;
    }
    private void OnDisable()
    {
        MyNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleClientDiconnected;
    }

    public void Joinlobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
        backButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        backButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
    private void HandleClientDiconnected()
    {
        joinButton.interactable = false;
        backButton.interactable = false;
    }
}
