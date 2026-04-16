using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    private bool hasSpawned;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(SpawnWhenReady());
    }

    private IEnumerator SpawnWhenReady()
    {
        // WAIT FOR SCENE TO FULLY STABILIZE
        yield return new WaitForSeconds(1f);

        if (hasSpawned) yield break;
        hasSpawned = true;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        SpawnEnemy("EnemySpawn1", Enemy.MovementType.Horizontal);
        SpawnEnemy("EnemySpawn2", Enemy.MovementType.Vertical);
        SpawnEnemy("EnemySpawn3", Enemy.MovementType.Horizontal);
    }

    private void SpawnEnemy(string tag, Enemy.MovementType type)
    {
        GameObject spawnPoint = GameObject.FindWithTag(tag);

        if (spawnPoint == null)
        {
            Debug.LogWarning($"Missing spawn point: {tag}");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is NOT assigned!");
            return;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);

        NetworkObject netObj = enemy.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("Enemy prefab missing NetworkObject!");
            return;
        }

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.movementType = type;
        }

        netObj.Spawn(true);

        Debug.Log("Spawned enemy at " + tag);
    }
}