using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanelUI : MonoBehaviour
{
    public Text winnerText;
    public Text timeText;
    public Text rollsText;
    public Text scoreText; 

    public void Show(string winnerName, float timeSeconds, int rolls)
    {
        gameObject.SetActive(true);

        int score = CalculateScore(timeSeconds, rolls);

        winnerText.text = winnerName;
        timeText.text =  FormatTime(timeSeconds);
        rollsText.text = " " + rolls;
        scoreText.text = " " + score;

        LeaderboardStorage.AddEntry(winnerName, timeSeconds, rolls, score);

        Time.timeScale = 0f;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private int CalculateScore(float timeSeconds, int rolls)
    {
        int score = Mathf.RoundToInt(10000f - timeSeconds * 10f - rolls * 100f);
        return Mathf.Max(0, score);
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        float s = seconds % 60f;
        return $"{m:00}:{s:00.00}";
    }
}
