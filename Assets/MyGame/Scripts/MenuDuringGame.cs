using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MenuDuringGame : MonoBehaviour
{
    [SerializeField] private GameObject menu = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            InputManager.Add(ActionMapNames.Player);
            menu.SetActive(true);
        }
        else if (menu.activeInHierarchy == false)
        {
            InputManager.Remove(ActionMapNames.Player);
        }
    }
    public void Exit()
    {
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();        
    }
}
