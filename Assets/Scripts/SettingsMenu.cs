using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private const string KEY_W = "SET_W";
    private const string KEY_H = "SET_H";
    private const string KEY_FS = "SET_FS";
    private const string KEY_VOL = "SET_VOL";

    private bool isInitializingUI;

    void Awake()
    {
        float vol = PlayerPrefs.GetFloat(KEY_VOL, 1f);
        AudioListener.volume = vol;

        ApplySavedScreenSettings();
    }

    void Start()
    {
        BuildResolutionDropdown();
        LoadSettingsToUI();
        HookUIEvents();
    }

    void OnEnable()
    {
        if (resolutionDropdown != null && resolutionDropdown.options.Count == 0)
            BuildResolutionDropdown();

        LoadSettingsToUI();
    }

    private void HookUIEvents()
    {
        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void BuildResolutionDropdown()
    {
        resolutions = Screen.resolutions;

        var unique = new List<Resolution>();
        var seen = new HashSet<string>();

        foreach (var r in resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (seen.Add(key))
                unique.Add(r);
        }

        resolutions = unique.ToArray();

        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadSettingsToUI()
    {
        isInitializingUI = true;

        float vol = PlayerPrefs.GetFloat(KEY_VOL, AudioListener.volume);
        volumeSlider.SetValueWithoutNotify(vol);

        bool fs = PlayerPrefs.GetInt(KEY_FS, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.SetIsOnWithoutNotify(fs);

        int savedW = PlayerPrefs.GetInt(KEY_W, Screen.width);
        int savedH = PlayerPrefs.GetInt(KEY_H, Screen.height);

        int index = GetResolutionIndex(savedW, savedH);
        resolutionDropdown.SetValueWithoutNotify(index);
        resolutionDropdown.RefreshShownValue();

        isInitializingUI = false;
    }

    private int GetResolutionIndex(int w, int h)
    {
        if (resolutions == null || resolutions.Length == 0)
            return 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == w && resolutions[i].height == h)
                return i;
        }

        int best = 0;
        int bestDiff = int.MaxValue;

        for (int i = 0; i < resolutions.Length; i++)
        {
            int diff = Mathf.Abs(resolutions[i].width - w) + Mathf.Abs(resolutions[i].height - h);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = i;
            }
        }

        return best;
    }

    private void ApplySavedScreenSettings()
    {
        int w = PlayerPrefs.GetInt(KEY_W, Screen.width);
        int h = PlayerPrefs.GetInt(KEY_H, Screen.height);
        bool fs = PlayerPrefs.GetInt(KEY_FS, Screen.fullScreen ? 1 : 0) == 1;

        FullScreenMode mode = fs ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Screen.SetResolution(w, h, mode);
    }

    public void SetResolution(int index)
    {
        if (isInitializingUI) return;
        if (resolutions == null || resolutions.Length == 0) return;
        index = Mathf.Clamp(index, 0, resolutions.Length - 1);

        int w = resolutions[index].width;
        int h = resolutions[index].height;

        bool fs = fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen;
        FullScreenMode mode = fs ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Screen.SetResolution(w, h, mode);

        PlayerPrefs.SetInt(KEY_W, w);
        PlayerPrefs.SetInt(KEY_H, h);
        PlayerPrefs.Save();
    }

    public void SetVolume(float vol)
    {
        if (isInitializingUI) return;

        AudioListener.volume = vol;
        PlayerPrefs.SetFloat(KEY_VOL, vol);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isInitializingUI) return;

        int w = PlayerPrefs.GetInt(KEY_W, Screen.width);
        int h = PlayerPrefs.GetInt(KEY_H, Screen.height);

        FullScreenMode mode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(w, h, mode);

        PlayerPrefs.SetInt(KEY_FS, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
