using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField] private HealthUI healthUI;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            health.Value = 100;

        health.OnValueChanged += OnHealthChanged;

        // IMPORTANT: bind UI for THIS instance (not owner-only)
        if (healthUI != null)
        {
            healthUI.Bind(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"[HEALTH] {OwnerClientId}: {oldValue} -> {newValue}");

        if (IsServer && newValue <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;
        Debug.Log($"[HEALTH] Took damage: {health.Value}");
    }

    private void Die()
    {
        Debug.Log($"[HEALTH] Player {OwnerClientId} died");
        GameManager.Instance.PlayerDied();
    }
}