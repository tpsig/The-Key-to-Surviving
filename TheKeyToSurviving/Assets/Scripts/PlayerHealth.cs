using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(100);

    public event Action<int> OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 100;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;
        OnHealthChanged?.Invoke(health.Value);

        if (health.Value <= 0)
        {
            health.Value = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} died");

        GetComponent<PlayerController>().enabled = false;

        GameManager.Instance.PlayerDied();
    }
}