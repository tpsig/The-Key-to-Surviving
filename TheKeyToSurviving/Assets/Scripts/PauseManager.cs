using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused;

    void Start()
    {
        // Subscribe to networked pause state changes
        if (GameManager.Instance != null)
            GameManager.Instance.isPausedNet.OnValueChanged += OnPauseStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.isPausedNet.OnValueChanged -= OnPauseStateChanged;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetPauseStateServerRpc(!GameManager.Instance.isPausedNet.Value);
        }
    }

    private void OnPauseStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
            PauseLocal();
        else
            ResumeLocal();
    }

    private void PauseLocal()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        SetCursor(false);
        Time.timeScale = 0f;
    }

    private void ResumeLocal()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        SetCursor(true);
        Time.timeScale = 1f;
    }

    public void ToggleSound()
    {
        AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
    }

    public void QuitGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGameState();

        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMenuServerRpc();
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetPauseStateServerRpc(false);
    }

    void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}