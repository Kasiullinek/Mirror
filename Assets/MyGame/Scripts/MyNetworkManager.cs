using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayer> RoomPlayer { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayer { get; } = new List<NetworkGamePlayer>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = RoomPlayer.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();

            RoomPlayer.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayer.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach(var player in RoomPlayer)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach(var player in RoomPlayer)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("Scene_Map_01");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Scene_Map"))
        {
            for(int i = RoomPlayer.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayer[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayer[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName.StartsWith("Scene_Map"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
