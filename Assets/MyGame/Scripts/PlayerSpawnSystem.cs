using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public Character[] characters = default;

    private int characterIndex = 1;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    public override void OnStartServer()
    {
        MyNetworkManager.OnServerReadied += SpawnPlayer;
    }

    public override void OnStartClient()
    {
        InputManager.Add(ActionMapNames.Player);
        InputManager.Controls.Player.Look.Enable();
    }

    [ServerCallback]
    private void OnDestroy()
    {
        MyNetworkManager.OnServerReadied -= SpawnPlayer;
    }
    public override void OnStopClient()
    {
        MyNetworkManager.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }

        characterIndex = PlayerPrefs.GetInt("PlayerIndex");

        Debug.Log("Character Index: " + characterIndex);

        GameObject playerInstance = Instantiate(characters[characterIndex].GameplayCharacterPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }
}
