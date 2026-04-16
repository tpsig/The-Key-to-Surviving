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

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    public NetworkVariable<bool> gameEndedNet = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
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
    }

    // =====================================================
    // 🎮 MENU
    // =====================================================

    public void CreateParty()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            return;

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host created");
    }

    public void JoinParty()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            return;

        NetworkManager.Singleton.StartClient();
        Debug.Log("Client joining");
    }

    public void StartGame()
    {
        if (!IsServer || sceneLoading) return;

        LoadScene(prisonScene);
    }

        // =====================================================
    // 🌍 SCENE SYSTEM
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

        Debug.Log("Scene loaded: " + sceneName);

        // ❌ NEVER spawn in GameOverScene
        if (sceneName == gameOverSceneName)
            return;

        // ✔ ONLY spawn in gameplay scenes
        if (sceneName == prisonScene || sceneName == diningRoomScene)
        {
            SpawnAllPlayers();
        }
    }

    // =====================================================
    // 👥 SPAWNING (FIXED NULL ISSUE)
    // =====================================================

    private void SpawnAllPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is NOT assigned in GameManager!");
            return;
        }

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.ClientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab);

        NetworkObject netObj = player.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("PlayerPrefab missing NetworkObject!");
            return;
        }

        netObj.SpawnAsPlayerObject(clientId, true);

        Debug.Log($"Spawned player {clientId}");
    }

    // =====================================================
    // 🔑 KEY
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

    // =====================================================
    // 💀 DEATH (SERVER AUTHORITATIVE)
    // =====================================================

    public void PlayerDiedServer(ulong clientId)
    {
        if (!IsServer || gameEndedNet.Value) return;

        Debug.Log($"Player died: {clientId}");

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.Despawn(true);
            }
        }

        LoseGame();
    }

    // =====================================================
    // 🏆 WIN / LOSE
    // =====================================================

    private void WinGame()
    {
        if (gameEndedNet.Value) return;

        gameEndedNet.Value = true;

        DespawnAllPlayers();
        LoadScene(gameOverSceneName);
    }

    private void LoseGame()
    {
        if (gameEndedNet.Value) return;

        gameEndedNet.Value = true;

        DespawnAllPlayers();
        LoadScene(gameOverSceneName);
    }

    // =====================================================
    // CLEANUP
    // =====================================================

    private void DespawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
                client.PlayerObject.Despawn(true);
        }
    }
}