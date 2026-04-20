using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI completionTimeText;
    public TMP_InputField playerNameInput;

    public Button retryButton;
    public TextMeshProUGUI retryButtonText;
    public Button backToMenuButton;

    void Start()
    {
        if (GameManager.Instance == null)
            return;

        bool won = GameManager.Instance != null && GameManager.Instance.playerWonNet.Value;
        float time = GameManager.Instance != null ? GameManager.Instance.finalTimeNet.Value : 0f;

        retryButton.gameObject.SetActive(true);
        backToMenuButton.gameObject.SetActive(true);

        if (won)
        {
            completionTimeText.text = "Final Time: " + time;
            retryButtonText.text = time >= 100 ? "Play Again" : "Try Again";
        }
        else
        {
            completionTimeText.text = "FAILED ATTEMPT";
            retryButtonText.text = "Try Again";
        }
    }

    public void OnSubmitTime()
    {
        if (GameManager.Instance == null || DatabaseManager.Instance == null)
            return;

        if (!GameManager.Instance.playerWonNet.Value)
            return;

        string playerName =
            string.IsNullOrEmpty(playerNameInput.text)
            ? "Anonymous"
            : playerNameInput.text;

        float time = GameManager.Instance.finalTimeNet.Value;

        DatabaseManager.Instance.SaveHighScore(playerName, time);
    }

    public void TryAgain()
    {
        OnSubmitTime();

        GameManager.Instance?.RequestTryAgainServerRpc();
    }

    // =====================================================
    // 🔥 FIXED: FULL NETWORK SHUTDOWN ON MENU RETURN
    // =====================================================

    public void BackToMenu()
    {
        OnSubmitTime();

        GameManager.Instance?.RequestReturnToMenuAndShutdownServerRpc();
    }
}