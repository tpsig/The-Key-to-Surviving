using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoresButton : MonoBehaviour {
    public void OnHighScoresClicked() {
        Debug.Log("Button Clicked");
        SceneManager.LoadScene("HighScoreScene");
    }
}