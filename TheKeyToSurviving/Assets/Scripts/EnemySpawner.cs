using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    private bool hasSpawned = false;

    public override void OnNetworkSpawn()
    {
        Debug.Log("SPAWNING ENEMIES CALLED");

        if (!IsServer) return;

        // ✅ PREVENT DOUBLE SPAWN
        if (hasSpawned) return;
        hasSpawned = true;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        SpawnEnemyWithTag("EnemySpawn1", Enemy.MovementType.Horizontal);
        SpawnEnemyWithTag("EnemySpawn2", Enemy.MovementType.Vertical);
        SpawnEnemyWithTag("EnemySpawn3", Enemy.MovementType.Horizontal);
    }

    private void SpawnEnemyWithTag(string tag, Enemy.MovementType movementType)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(tag);

        if (spawnPoint == null)
        {
            Debug.LogWarning($"No spawn point found with tag: {tag}");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);

        NetworkObject netObj = enemyObj.GetComponent<NetworkObject>();
        Enemy enemyScript = enemyObj.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.movementType = movementType;
        }

        netObj.Spawn(true); // IMPORTANT: ownership = server
    }
}