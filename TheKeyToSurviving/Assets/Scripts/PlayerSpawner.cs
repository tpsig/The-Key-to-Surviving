using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    public Transform hostSpawn;
    public Transform clientSpawn;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnAllExistingPlayers();
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }

    private void SpawnAllExistingPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject playerObj = Instantiate(playerPrefab);

        Vector3 spawnPos =
            (clientId == NetworkManager.ServerClientId)
                ? hostSpawn.position
                : clientSpawn.position;

        playerObj.transform.position = spawnPos;

        NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);
    }
}