using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseBackground; 
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (pauseBackground) pauseBackground.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        settingsPanel.SetActive(false);

        if (pauseBackground)
            pauseBackground.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        if (pauseBackground)
            pauseBackground.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

 
    public void BackFromSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
