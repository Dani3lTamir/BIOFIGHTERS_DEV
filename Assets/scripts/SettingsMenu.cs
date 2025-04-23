using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; 
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string QualityKey = "QualityLevel";
    private const string FullscreenKey = "IsFullscreen";

    void Start()
    {
        // Load saved settings or use defaults
        LoadSettings();

        // Add listeners
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
    }

    void LoadSettings()
    {
        // Audio Settings
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 0f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        // Quality Settings
        int qualityLevel = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        qualityDropdown.value = qualityLevel;
        SetQuality(qualityLevel);

        // Fullscreen Setting
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = isFullscreen;
        ToggleFullscreen(isFullscreen);
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        SaveSettings();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QualityKey, qualityIndex);
        SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved");
    }

    public void ExitSettingsMenu()
    {
        SaveSettings();
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        // Clean up listeners
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        fullscreenToggle.onValueChanged.RemoveListener(ToggleFullscreen);
    }
}