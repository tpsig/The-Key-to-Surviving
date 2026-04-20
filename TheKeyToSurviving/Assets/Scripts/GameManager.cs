using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scenes")]
    public string prisonScene = "PrisonScene";
    public string diningRoomScene = "DiningRoomScene";
    public string gameOverSceneName = "GameOverScene";
    public string mainMenuScene = "MainMenuScene";

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Factory")]
    [SerializeField] private EnemyFactory enemyFactory;

    public float CurrentTime { get; private set; }
    private float gameStartTime;
    private bool timerRunning;

    public NetworkVariable<bool> gameEndedNet = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // =========================
    // NEW SYNCED GAME OVER DATA
    // =========================
    public NetworkVariable<float> finalTimeNet = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> playerWonNet = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> isPausedNet = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private bool sceneLoading;

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

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;

        gameEndedNet.Value = false;
        StartTimer();
    }

    private void StartTimer()
    {
        gameStartTime = Time.time;
        timerRunning = true;
    }

    private void StopTimer()
    {
        if (!timerRunning) return;

        CurrentTime = Time.time - gameStartTime;
        timerRunning = false;
    }

    // =====================================================
    // MENU
    // =====================================================

    public void CreateParty()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            return;

        NetworkManager.Singleton.StartHost();
    }

    public void GoToHighScoreScene()
    {
        Debug.Log("[GameManager] Loading HighScoreScene");

        // Optional safety: stop timer so it doesn't keep running in background
        StopTimer();

        // IMPORTANT:
        // Do NOT touch NetworkManager here (no shutdown, no RPCs)
        // This is a local UI-only transition

        SceneManager.LoadScene("HighScoreScene");
    }

    public void StartGame()
    {
        if (!IsServer || sceneLoading) return;

        LoadScene(prisonScene);
        StartTimer();
    }

    public void OnHighScoresClicked()
    {
        SceneManager.LoadScene("HighScoreScene");
    }

    // =====================================================
    // RETURN TO MENU
    // =====================================================

    [ServerRpc(RequireOwnership = false)]
    public void RequestReturnToMenuServerRpc()
    {
        StopTimer();
        DespawnAllPlayers();
        ReturnToMenuClientRpc();
    }

    [ClientRpc]
    private void ReturnToMenuClientRpc()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestReturnToMenuAndShutdownServerRpc()
    {
        StopTimer();
        DespawnAllPlayers();

        // Move everyone FIRST
        NetworkManager.Singleton.SceneManager.LoadScene(
            mainMenuScene,
            LoadSceneMode.Single
        );

        // Then shutdown after load completes
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += ShutdownAfterMenuLoad;
    }

    private void ShutdownAfterMenuLoad(
        string sceneName,
        LoadSceneMode mode,
        List<ulong> completed,
        List<ulong> timedOut)
    {
        if (sceneName != mainMenuScene) return;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= ShutdownAfterMenuLoad;

        Debug.Log("[GameManager] Shutting down network AFTER menu load");

        NetworkManager.Singleton.Shutdown();
    }

    // =====================================================
    // TRY AGAIN
    // =====================================================

    [ServerRpc(RequireOwnership = false)]
    public void RequestTryAgainServerRpc()
    {
        gameEndedNet.Value = false;

        CurrentTime = 0f;
        StartTimer();

        LoadScene(prisonScene);
    }

    // =====================================================
    // SCENES
    // =====================================================

    private void LoadScene(string scene)
    {
        sceneLoading = true;

        NetworkManager.Singleton.SceneManager.LoadScene(
            scene,
            LoadSceneMode.Single
        );
    }

    private void OnSceneLoaded(
        string sceneName,
        LoadSceneMode mode,
        List<ulong> completed,
        List<ulong> timedOut)
    {
        sceneLoading = false;

        if (!IsServer) return;

        if (sceneName == gameOverSceneName)
        {
            StopTimer();
            return;
        }

        if (sceneName == prisonScene || sceneName == diningRoomScene)
        {
            SpawnAllPlayers();
        }
    }

    // =====================================================
    // SPAWNING
    // =====================================================

    private void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.ClientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab);

        NetworkObject netObj = player.GetComponent<NetworkObject>();

        netObj.SpawnAsPlayerObject(clientId, true);
    }

    // =====================================================
    // GAME LOGIC
    // =====================================================

    public void CollectKey()
    {
        if (!IsServer || gameEndedNet.Value) return;

        string scene = SceneManager.GetActiveScene().name;

        if (scene == prisonScene)
            LoadScene(diningRoomScene);
        else if (scene == diningRoomScene)
            WinGame();
    }

    public void PlayerDiedServer(ulong clientId)
    {
        if (!IsServer || gameEndedNet.Value) return;

        LoseGame();
    }

    // =====================================================
    // WIN / LOSE (SYNCED)
    // =====================================================

    private void WinGame()
    {
        if (gameEndedNet.Value) return;

        gameEndedNet.Value = true;

        // 🔥 CRITICAL FIX: finalize time FIRST
        StopTimer();

        playerWonNet.Value = true;

        // 🔥 ensure we use the FINAL computed time
        finalTimeNet.Value = CurrentTime;

        DespawnAllPlayers();
        LoadScene(gameOverSceneName);
    }

    private void LoseGame()
    {
        if (gameEndedNet.Value) return;

        gameEndedNet.Value = true;

        StopTimer();

        playerWonNet.Value = false;

        // explicitly set AFTER stop timer (safe but irrelevant)
        finalTimeNet.Value = 0f;

        DespawnAllPlayers();
        LoadScene(gameOverSceneName);
    }

    public void ResetGame()
    {
        if (!IsServer) return;

        gameEndedNet.Value = false;
        CurrentTime = 0f;

        StartTimer();
    }

    private void DespawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
                client.PlayerObject.Despawn(true);
        }
    }

    public void ClearLocalState()
    {
        CurrentTime = 0f;
        gameEndedNet.Value = false;
        playerWonNet.Value = false;
        finalTimeNet.Value = 0f;
    }

    public EnemyFactory GetEnemyFactory() => enemyFactory;

    public void SaveGameState()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("LastTime", CurrentTime);
        PlayerPrefs.SetInt("GameEnded", gameEndedNet.Value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadGameState()
    {
        if (!PlayerPrefs.HasKey("LastScene")) return;

        string scene = PlayerPrefs.GetString("LastScene");

        CurrentTime = PlayerPrefs.GetFloat("LastTime");

        gameEndedNet.Value = PlayerPrefs.GetInt("GameEnded") == 1;

        SceneManager.LoadScene(scene);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPauseStateServerRpc(bool pause)
    {
        isPausedNet.Value = pause;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReturnToMenuServerRpc()
    {
        RequestReturnToMenuAndShutdownServerRpc();
    }
}