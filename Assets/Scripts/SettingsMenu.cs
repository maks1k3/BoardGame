using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    void Start()
    {
        // ----- Заполняем Dropdown доступными разрешениями -----
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // ----- Громкость -----
        volumeSlider.value = AudioListener.volume; // от 0 до 1

        // ----- Полноэкранный режим -----
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    // Dropdown OnValueChanged(int)
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    // Slider OnValueChanged(float)

    void OnEnable()
    {
        // ВАЖНО: не триггерит OnValueChanged
        volumeSlider.SetValueWithoutNotify(AudioListener.volume);
    }
    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    // Toggle OnValueChanged(bool)
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // Кнопка Back
    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
