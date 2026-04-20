using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;

public static class GameServices
{
    public static GameManager GM =>
        GameManager.Instance;

    public static NetworkManager NM =>
        NetworkManager.Singleton;
}
public class MainMenuUI : MonoBehaviour
{

    public void StartGame()
    {
        GameServices.GM?.StartGame();
    }

    public void OpenHighScores()
    {
        GameServices.GM?.GoToHighScoreScene();
    }
}