using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseBackground;
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        pauseBackground.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseBackground.SetActive(isPaused);
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;

        pauseBackground.SetActive(false);
        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
