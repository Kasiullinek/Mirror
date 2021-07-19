using UnityEngine;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
}
