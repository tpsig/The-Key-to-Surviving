using UnityEngine;
using Unity.Netcode;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;

    private void Awake()
    {
        // Singleton (simple version)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Enemy CreateEnemy(Vector3 position, Enemy.MovementType type)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned in Factory!");
            return null;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);

        NetworkObject netObj = enemyObj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("Enemy prefab missing NetworkObject!");
            return null;
        }

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.movementType = type;
        }

        netObj.Spawn(true);

        return enemy;
    }
}