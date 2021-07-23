using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using Cinemachine;
using Inputs;

public class Chat : NetworkBehaviour
{
    [Header("Chat")]
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text text = null;
    [SerializeField] private TMP_InputField inputField= null;

    [Header("Menu")]
    [SerializeField] private GameObject menuUI = null;

    private static event Action<string> OnMessage;

    private bool IsGameStarted = false;

    public string DisplayName = "";

    public override void OnStartClient()
    {
        StartCoroutine("WaitForFiveSeconds");
    }

    private IEnumerator WaitForFiveSeconds()
    {
        yield return new WaitForSeconds(5);
        IsGameStarted = true;
        StopCoroutine("WaitForFiveSeconds");
    }

    private void Update()
    {
        if (!hasAuthority) { return; }
         
        if (IsGameStarted == true)
        {
            HandleChatAndMenu();
        }

    }

    private void HandleChatAndMenu()
    {
        if (Input.GetKeyDown(KeyCode.F12) && menuUI.active == false)
        {
            chatUI.active = !chatUI.active;
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            menuUI.SetActive(true);
        }

        if (chatUI.activeInHierarchy == true || menuUI.active == true)
        {
            InputManager.Add(ActionMapNames.Player);
        }
        if (chatUI.activeInHierarchy == false || menuUI.active == false)
        {
            InputManager.Remove(ActionMapNames.Player);
        }
    }

    #region Chat
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        OnMessage += HandleNewMessage;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string mess)
    {
        text.text += mess;
    }

    [Client]
    public void Send(string mess)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }

        if(string.IsNullOrWhiteSpace(mess)) { return; }

        CmdSendMessage(mess);

        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string mess)
    {
        RpcHandleMessage($"[{DisplayName}]: {mess}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string mess)
    {
        OnMessage?.Invoke($"\n{ mess}");
    }
    #endregion


    #region Menu
    public void Exit()
    {
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();
    }
    #endregion
}
