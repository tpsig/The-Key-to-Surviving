using UnityEngine;
using TMPro;
using Unity.Netcode;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private PlayerHealth playerHealth;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientReady;
        StartCoroutine(FindLocalPlayer());
    }

    private void OnClientReady(ulong clientId)
    {
        StartCoroutine(FindLocalPlayer());
    }

    private System.Collections.IEnumerator FindLocalPlayer()
    {
        while (playerHealth == null)
        {
            foreach (var p in FindObjectsOfType<PlayerHealth>())
            {
                if (p.NetworkObject.IsOwner)
                {
                    playerHealth = p;

                    Debug.Log("[UI] Bound to player: " + p.name);

                    playerHealth.health.OnValueChanged += OnHealthChanged;

                    UpdateHealthText(playerHealth.health.Value);
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"[UI] Health changed: {oldValue} -> {newValue}");
        UpdateHealthText(newValue);
    }

    private void UpdateHealthText(int value)
    {
        if (healthText != null)
            healthText.text = "Health: " + value;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.health.OnValueChanged -= OnHealthChanged;

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientReady;
    }
}