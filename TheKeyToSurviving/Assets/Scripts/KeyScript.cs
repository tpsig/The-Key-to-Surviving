using UnityEngine;
using Unity.Netcode;

public class Key : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[KEY] Triggered by: {other.name} (Tag: {other.tag})");
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectKey();

            // Destroy across network
            NetworkObject.Despawn(true);
        }
    }
}