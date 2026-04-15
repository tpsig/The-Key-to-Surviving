using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> health =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[HEALTH] Spawned: {name} Owner={OwnerClientId} IsOwner={IsOwner} IsServer={IsServer}");

        if (IsServer)
            health.Value = 100;

        // Subscribe once per object
        health.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"[HEALTH] {name}: {oldValue} -> {newValue}");
    }

    // SERVER ONLY
    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;

        Debug.Log($"[HEALTH] Took damage. New value: {health.Value}");

        if (health.Value <= 0)
        {
            health.Value = 0;
            GameManager.Instance.PlayerDied();
        }
    }
}