using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private bool isInitializing = false;

    // PlayerPrefs keys
    private const string KEY_VOLUME = "Volume";
    private const string KEY_FULLSCREEN = "Fullscreen";
    private const string KEY_RES_INDEX = "ResolutionIndex";

    void Start()
    {
        InitResolutions();
        LoadSettingsToUI();
        ApplySettings();
    }

    void OnEnable()
    {
        // Когда окно открывается (в меню или в паузе)
        LoadSettingsToUI();
    }

    // ---------- INIT ----------
    void InitResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }

        resolutionDropdown.AddOptions(options);
    }

    // ---------- LOAD ----------
    void LoadSettingsToUI()
    {
        isInitializing = true;

        int resIndex = PlayerPrefs.GetInt(KEY_RES_INDEX, GetCurrentResolutionIndex());
        float volume = PlayerPrefs.GetFloat(KEY_VOLUME, 1f);
        bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;

        resolutionDropdown.value = Mathf.Clamp(resIndex, 0, resolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();

        volumeSlider.SetValueWithoutNotify(volume);
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        isInitializing = false;
    }

    // ---------- APPLY ----------
    void ApplySettings()
    {
        SetResolution(resolutionDropdown.value);
        SetVolume(volumeSlider.value);
        SetFullscreen(fullscreenToggle.isOn);
    }

    // ---------- UI CALLBACKS ----------
    public void SetResolution(int index)
    {
        if (isInitializing) return;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt(KEY_RES_INDEX, index);
        PlayerPrefs.Save();
    }

    public void SetVolume(float vol)
    {
        if (isInitializing) return;

        AudioListener.volume = vol;
        PlayerPrefs.SetFloat(KEY_VOLUME, vol);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isInitializing) return;

        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ---------- UTILS ----------
    int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                return i;
        }
        return 0;
    }

    // ---------- CLOSE ----------
    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
