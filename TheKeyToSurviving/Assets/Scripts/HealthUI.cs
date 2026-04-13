using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private PlayerHealth playerHealth;

    void Start()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();

        foreach (var p in players)
        {
            if (p.IsOwner)
            {
                playerHealth = p;
                break;
            }
        }

        if (playerHealth == null)
        {
            Debug.LogError("No local PlayerHealth found!");
            return;
        }

        playerHealth.OnHealthChanged += UpdateHealthText;

        UpdateHealthText(playerHealth.health.Value);
    }

    void UpdateHealthText(int health)
    {
        healthText.text = "Health: " + health;
    }
}