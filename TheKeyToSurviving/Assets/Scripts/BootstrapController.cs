using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapController : MonoBehaviour
{
    private static bool initialized = false;

    private void Awake()
    {
        if (initialized)
            return;

        initialized = true;

        DontDestroyOnLoad(gameObject);

        Debug.Log("[Bootstrap] Systems initialized");

        SceneManager.LoadScene("MainMenuScene");
    }
}