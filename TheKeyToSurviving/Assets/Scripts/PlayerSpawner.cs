using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    public Transform hostSpawn;
    public Transform clientSpawn;

    private bool hostSpawned = false;
    private bool clientSpawned = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId && hostSpawned)
            return;

        if (clientId != NetworkManager.ServerClientId && clientSpawned)
            return;

        GameObject playerObj = Instantiate(playerPrefab);
        NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
        PlayerIdentity identity = playerObj.GetComponent<PlayerIdentity>();

        if (clientId == NetworkManager.ServerClientId)
        {
            playerObj.transform.position = hostSpawn.position;

            hostSpawned = true;
        }
        else
        {
            playerObj.transform.position = clientSpawn.position;

            clientSpawned = true;
        }

        netObj.SpawnAsPlayerObject(clientId, true);
    }
}