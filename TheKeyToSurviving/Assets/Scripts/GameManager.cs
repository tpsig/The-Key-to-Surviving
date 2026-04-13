using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int deadPlayers = 0;
    private bool gameEnded = false;

    [Header("Settings")]
    public string gameOverSceneName = "GameOverScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CollectKey()
    {
        if (gameEnded) return;

        Debug.Log("Key collected → WIN");
        WinGame();
    }

    public void PlayerDied()
    {
        if (gameEnded) return;

        deadPlayers++;

        Debug.Log("Player died. Total dead: " + deadPlayers);

        LoseGame();
    }

    private void WinGame()
    {
        gameEnded = true;
        Debug.Log("YOU WIN!");
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void LoseGame()
    {
        gameEnded = true;
        Debug.Log("ALL PLAYERS DEAD → GAME OVER");
        SceneManager.LoadScene(gameOverSceneName);
    }
}