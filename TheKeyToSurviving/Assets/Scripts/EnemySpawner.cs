using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemySpawner : NetworkBehaviour
{
    private bool hasSpawned;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(SpawnWhenReady());
    }

    private IEnumerator SpawnWhenReady()
    {
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

        // 🔥 Factory handles creation
        EnemyFactory.Instance.CreateEnemy(
            spawnPoint.transform.position,
            type
        );
    }
}