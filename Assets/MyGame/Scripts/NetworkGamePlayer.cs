using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

    private MyNetworkManager room;
    public MyNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayer.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayer.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
    
}
