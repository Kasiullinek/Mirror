using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    private MyNetworkManager room;

    private MyNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    public void CountdownEnded()
    {
        animator.enabled = false;
    }

    #region Server

    public override void OnStartServer()
    {
        MyNetworkManager.OnServerStopped += CleanUpServer;
        MyNetworkManager.OnServerReadied += CheckToStartRound;
    }

    [ServerCallback]

    private void OnDestroy()
    {
        CleanUpServer();
    }

    [Server]
    private void CleanUpServer()
    {
        MyNetworkManager.OnServerStopped -= CleanUpServer;
        MyNetworkManager.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]
    public void StartRound()
    {
        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        if (Room.GamePlayer.Count(x => x.connectionToClient.isReady) != Room.GamePlayer.Count) { return; }

        animator.enabled = true;

        RpcStartCountdown();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcStartCountdown()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound()
    {
        InputManager.Remove(ActionMapNames.Player);

    }

    #endregion
}
