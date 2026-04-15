/*using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(100);
    public NetworkVariable<bool> hasKey = new NetworkVariable<bool>(false);

    // ===== DELEGATES (PATTERN #2) =====
    public event Action<int> OnHealthChanged;
    public static event Action<ulong, int> OnPlayerDamaged;

    private void Start()
    {
        health.OnValueChanged += (_, newValue) =>
        {
            OnHealthChanged?.Invoke(newValue);
        };
    }

    // ===== DAMAGE =====
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (health.Value <= 0) return;

        health.Value -= damage;

        OnPlayerDamaged?.Invoke(OwnerClientId, damage);

        if (health.Value <= 0)
        {
            health.Value = 0;
            Debug.Log($"Player {OwnerClientId} died");
        }
    }

    // ===== KEY PICKUP =====
    [ServerRpc(RequireOwnership = false)]
    public void CollectKeyServerRpc()
    {
        if (hasKey.Value) return;

        hasKey.Value = true;

        Debug.Log($"Player {OwnerClientId} collected key");

        GameManager.Instance.DeclareWinnerAndEndGame(OwnerClientId);
    }
}*/