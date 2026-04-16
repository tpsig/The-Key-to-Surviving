using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private PlayerHealth playerHealth;

    public void Bind(PlayerHealth target)
    {
        if (playerHealth != null)
            playerHealth.health.OnValueChanged -= OnHealthChanged;

        playerHealth = target;

        if (playerHealth == null) return;

        playerHealth.health.OnValueChanged += OnHealthChanged;

        UpdateHealth(playerHealth.health.Value);
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealth(newValue);
    }

    private void UpdateHealth(int value)
    {
        healthText.text = $"Health: {value}";
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.health.OnValueChanged -= OnHealthChanged;
    }
}