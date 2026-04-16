using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<bool> gameEndedNet = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> deadPlayersNet = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    // ✅ NOW A NORMAL SERVER METHOD (NOT ServerRpc)
    public void CollectKey()
    {
        if (!IsServer || gameEndedNet.Value) return;

        Debug.Log("Key collected → WIN");
        WinGame();
    }

    public void PlayerDied()
    {
        if (!IsServer || gameEndedNet.Value) return;

        deadPlayersNet.Value++;
        Debug.Log("Player died. Total dead: " + deadPlayersNet.Value);

        LoseGame();
    }

    private void WinGame()
    {
        gameEndedNet.Value = true;

        Debug.Log("YOU WIN!");

        DespawnAllPlayers();

        NetworkManager.Singleton.SceneManager.LoadScene(
            gameOverSceneName,
            LoadSceneMode.Single
        );

        WinGameClientRpc();
    }

    private void LoseGame()
    {
        gameEndedNet.Value = true;

        Debug.Log("GAME OVER");

        DespawnAllPlayers();

        NetworkManager.Singleton.SceneManager.LoadScene(
            gameOverSceneName,
            LoadSceneMode.Single
        );

        LoseGameClientRpc();
    }

    [ClientRpc]
    private void WinGameClientRpc()
    {
        Debug.Log("CLIENT: YOU WIN!");
    }

    [ClientRpc]
    private void LoseGameClientRpc()
    {
        Debug.Log("CLIENT: GAME OVER");
    }

    private void DespawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.Despawn(true);
            }
        }
    }
}