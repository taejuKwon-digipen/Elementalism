using System;
using UnityEngine;
using MoreMountains.Tools;

[System.Serializable]
public class GameSettings
{
    [Header("Audio Settings")]
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public float uiVolume = 1.0f;

    [Header("Display Settings")]
    public int resolutionIndex = 0;
    public bool isFullscreen = true;

    [Header("Localization")]
    public SystemLanguage language = SystemLanguage.Korean;
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Settings Data")]
    public GameSettings currentSettings = new GameSettings();

    [Header("Resolution Options")]
    public Resolution[] availableResolutions;

    // Events for UI updates
    public static event Action<GameSettings> OnSettingsChanged;

    private const string SETTINGS_SAVE_KEY = "GameSettings";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSettings()
    {
        // Get available resolutions
        availableResolutions = Screen.resolutions;
        
        // Load saved settings
        LoadSettings();
        
        // Apply loaded settings
        ApplyAllSettings();
    }

    #region Settings Management

    public void SaveSettings()
    {
        string settingsJson = JsonUtility.ToJson(currentSettings);
        PlayerPrefs.SetString(SETTINGS_SAVE_KEY, settingsJson);
        PlayerPrefs.Save();
        
        Debug.Log("[SettingsManager] Settings saved: " + settingsJson);
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SETTINGS_SAVE_KEY))
        {
            string settingsJson = PlayerPrefs.GetString(SETTINGS_SAVE_KEY);
            currentSettings = JsonUtility.FromJson<GameSettings>(settingsJson);
            Debug.Log("[SettingsManager] Settings loaded: " + settingsJson);
        }
        else
        {
            // First time setup - detect current settings
            DetectCurrentSettings();
            Debug.Log("[SettingsManager] First time setup - using default settings");
        }
    }

    private void DetectCurrentSettings()
    {
        // Detect current resolution
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentSettings.resolutionIndex = i;
                break;
            }
        }

        currentSettings.isFullscreen = Screen.fullScreen;
        currentSettings.language = Application.systemLanguage;
    }

    public void ApplyAllSettings()
    {
        ApplyAudioSettings();
        ApplyDisplaySettings();
        ApplyLanguageSettings();
        
        // Notify UI that settings have changed
        OnSettingsChanged?.Invoke(currentSettings);
    }

    #endregion

    #region Audio Settings

    public void SetMasterVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    public void SetMusicVolume(float volume)
    {
        currentSettings.musicVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    public void SetSfxVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    public void SetUIVolume(float volume)
    {
        currentSettings.uiVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    private void ApplyAudioSettings()
    {
        // Use MMSoundManager for audio control
        if (MMSoundManager.Instance != null && MMSoundManager.Instance.settingsSo != null)
        {
            MMSoundManager.Instance.settingsSo.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Master, currentSettings.masterVolume);
            MMSoundManager.Instance.settingsSo.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music, currentSettings.musicVolume);
            MMSoundManager.Instance.settingsSo.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Sfx, currentSettings.sfxVolume);
            MMSoundManager.Instance.settingsSo.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.UI, currentSettings.uiVolume);
            
            Debug.Log($"[SettingsManager] Audio settings applied - Master: {currentSettings.masterVolume}, Music: {currentSettings.musicVolume}, SFX: {currentSettings.sfxVolume}, UI: {currentSettings.uiVolume}");
        }
    }

    #endregion

    #region Display Settings

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < availableResolutions.Length)
        {
            currentSettings.resolutionIndex = resolutionIndex;
            ApplyDisplaySettings();
            OnSettingsChanged?.Invoke(currentSettings);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        currentSettings.isFullscreen = isFullscreen;
        ApplyDisplaySettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    private void ApplyDisplaySettings()
    {
        if (currentSettings.resolutionIndex >= 0 && currentSettings.resolutionIndex < availableResolutions.Length)
        {
            Resolution selectedResolution = availableResolutions[currentSettings.resolutionIndex];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, currentSettings.isFullscreen);
            
            Debug.Log($"[SettingsManager] Display settings applied - Resolution: {selectedResolution.width}x{selectedResolution.height}, Fullscreen: {currentSettings.isFullscreen}");
        }
    }

    public string[] GetResolutionOptions()
    {
        string[] options = new string[availableResolutions.Length];
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            options[i] = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
        }
        return options;
    }

    #endregion

    #region Language Settings

    public void SetLanguage(SystemLanguage language)
    {
        currentSettings.language = language;
        ApplyLanguageSettings();
        OnSettingsChanged?.Invoke(currentSettings);
    }

    private void ApplyLanguageSettings()
    {
        // TODO: Implement localization system
        // For now, just log the change
        Debug.Log($"[SettingsManager] Language changed to: {currentSettings.language}");
        
        // Here you would typically:
        // 1. Update UI text components
        // 2. Reload localized strings
        // 3. Update any language-dependent game elements
    }

    public SystemLanguage[] GetSupportedLanguages()
    {
        // Define supported languages
        return new SystemLanguage[]
        {
            SystemLanguage.Korean,
            SystemLanguage.English,
            SystemLanguage.Japanese,
            SystemLanguage.Chinese
        };
    }

    public string GetLanguageDisplayName(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean: return "한국어";
            case SystemLanguage.English: return "English";
            case SystemLanguage.Japanese: return "日本語";
            default: return language.ToString();
        }
    }

    #endregion

    #region Public Getters

    public float GetMasterVolume() => currentSettings.masterVolume;
    public float GetMusicVolume() => currentSettings.musicVolume;
    public float GetSfxVolume() => currentSettings.sfxVolume;
    public float GetUIVolume() => currentSettings.uiVolume;
    public int GetResolutionIndex() => currentSettings.resolutionIndex;
    public bool GetFullscreen() => currentSettings.isFullscreen;
    public SystemLanguage GetLanguage() => currentSettings.language;

    #endregion

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSettings();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveSettings();
        }
    }

    private void OnDestroy()
    {
        SaveSettings();
    }
} 