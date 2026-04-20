using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighScoresDisplay : MonoBehaviour {
    public TextMeshProUGUI[] timeTexts;

    void Start() {
        DisplayHighScores();
    }

    void DisplayHighScores() {
        List<HighScore> topTimes = DatabaseManager.Instance.GetTopHighScores();

        if (topTimes.Count == 0) {
            for (int i = 0; i < timeTexts.Length; i++) {
                timeTexts[i].text = (i + 1) + ". No scores yet!";
            }
            return;
        }

        for (int i = 0; i < timeTexts.Length; i++) {
            if (i < topTimes.Count) {
                HighScore completionTime = topTimes[i];
                int rank = i + 1;

                timeTexts[i].text =
                    rank + ". " +
                    completionTime.playerName + " - " +
                    completionTime.completionTime.ToString("F1") + "s";
            }
            else {
                timeTexts[i].text = (i + 1) + ". ---";
            }
        }
    }
}