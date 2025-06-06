using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsUI : MonoBehaviour
{
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button closeButton;

    [Header("Audio Controls")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;
    
    [Header("Audio Value Labels")]
    public TextMeshProUGUI masterVolumeLabel;
    public TextMeshProUGUI musicVolumeLabel;
    public TextMeshProUGUI sfxVolumeLabel;
    public TextMeshProUGUI uiVolumeLabel;

    [Header("Display Controls")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Language Controls")]
    public TMP_Dropdown languageDropdown;

    [Header("Action Buttons")]
    public Button applyButton;
    public Button resetButton;
    public Button saveButton;

    [Header("Text Labels for Localization")]
    public TextMeshProUGUI settingsTitleText;
    public TextMeshProUGUI soundSectionText;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI uiVolumeText;
    public TextMeshProUGUI displaySectionText;
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI fullscreenText;
    public TextMeshProUGUI languageSectionText;
    public TextMeshProUGUI saveButtonText;
    public TextMeshProUGUI resetButtonText;

    [Header("In-Game Settings (선택사항)")]
    public KeyCode settingsHotkey = KeyCode.Escape;
    public Button settingsButton;
    public bool pauseGameOnSettings = true;
    public bool enableHotkeyInGame = true;

    private bool isInitialized = false;
    private bool isSettingsOpen = false;
    private float originalTimeScale;

    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
        originalTimeScale = Time.timeScale;

        // Initialize SettingsManager if needed
        if (SettingsManager.Instance == null)
        {
            GameObject settingsManagerGO = new GameObject("SettingsManager");
            settingsManagerGO.AddComponent<SettingsManager>();
        }

        // Wait a bit for GoogleSheetLoader to load, then update text
        Invoke(nameof(UpdateLocalizedText), 1f);
    }

    private void Update()
    {
        // Handle hotkey input (only in game, not in main menu)
        if (enableHotkeyInGame && Input.GetKeyDown(settingsHotkey))
        {
            ToggleSettings();
        }
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += UpdateUI;
        // Update localized text when enabled
        if (GoogleSheetLoader.Instance != null && GoogleSheetLoader.Instance.IsLoaded)
        {
            UpdateLocalizedText();
        }
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= UpdateUI;
    }

    private void InitializeUI()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("[SettingsUI] SettingsManager not found!");
            return;
        }

        SetupResolutionDropdown();
        SetupLanguageDropdown();
        UpdateUI(SettingsManager.Instance.currentSettings);
        
        isInitialized = true;
    }

    private void SetupEventListeners()
    {
        // Close button
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);

        // Settings button (for in-game use)
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        // Audio sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);

        // Display controls
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        // Language control
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Action buttons
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSettings);
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null || SettingsManager.Instance == null) return;

        resolutionDropdown.ClearOptions();
        string[] resolutionOptions = SettingsManager.Instance.GetResolutionOptions();
        resolutionDropdown.AddOptions(resolutionOptions.ToList());
    }

    private void SetupLanguageDropdown()
    {
        if (languageDropdown == null || SettingsManager.Instance == null) return;

        languageDropdown.ClearOptions();
        SystemLanguage[] supportedLanguages = SettingsManager.Instance.GetSupportedLanguages();
        
        var languageOptions = supportedLanguages.Select(lang => 
            SettingsManager.Instance.GetLanguageDisplayName(lang)).ToList();
        
        languageDropdown.AddOptions(languageOptions);
    }

    private void UpdateUI(GameSettings settings)
    {
        if (!isInitialized) return;

        // Update audio sliders
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = settings.masterVolume;
            UpdateVolumeLabel(masterVolumeLabel, settings.masterVolume);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = settings.musicVolume;
            UpdateVolumeLabel(musicVolumeLabel, settings.musicVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = settings.sfxVolume;
            UpdateVolumeLabel(sfxVolumeLabel, settings.sfxVolume);
        }
        
        if (uiVolumeSlider != null)
        {
            uiVolumeSlider.value = settings.uiVolume;
            UpdateVolumeLabel(uiVolumeLabel, settings.uiVolume);
        }

        // Update display settings
        if (resolutionDropdown != null)
            resolutionDropdown.value = settings.resolutionIndex;
        
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = settings.isFullscreen;

        // Update language setting
        if (languageDropdown != null && SettingsManager.Instance != null)
        {
            SystemLanguage[] supportedLanguages = SettingsManager.Instance.GetSupportedLanguages();
            for (int i = 0; i < supportedLanguages.Length; i++)
            {
                if (supportedLanguages[i] == settings.language)
                {
                    languageDropdown.value = i;
                    break;
                }
            }
        }
    }

    private void UpdateVolumeLabel(TextMeshProUGUI label, float volume)
    {
        if (label != null)
        {
            label.text = Mathf.RoundToInt(volume * 100).ToString() + "%";
        }
    }

    private void UpdateLocalizedText()
    {
        if (GoogleSheetLoader.Instance == null || !GoogleSheetLoader.Instance.IsLoaded)
        {
            Debug.LogWarning("[SettingsUI] GoogleSheetLoader not ready yet, will try again later");
            Invoke(nameof(UpdateLocalizedText), 1f);
            return;
        }

        // Update all text elements with localized text
        if (settingsTitleText != null)
            settingsTitleText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_TITLE");

        if (soundSectionText != null)
            soundSectionText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_SOUND");

        if (masterVolumeText != null)
            masterVolumeText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_MASTER_VOLUME");

        if (musicVolumeText != null)
            musicVolumeText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_MUSIC_VOLUME");

        if (sfxVolumeText != null)
            sfxVolumeText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_SFX_VOLUME");

        if (uiVolumeText != null)
            uiVolumeText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_UI_VOLUME");

        if (displaySectionText != null)
            displaySectionText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_DISPLAY");

        if (resolutionText != null)
            resolutionText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_RESOLUTION");

        if (fullscreenText != null)
            fullscreenText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_FULLSCREEN");

        if (languageSectionText != null)
            languageSectionText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_LANGUAGE_SECTION");

        if (saveButtonText != null)
            saveButtonText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_SAVE");

        if (resetButtonText != null)
            resetButtonText.text = GoogleSheetLoader.Instance.GetText("SETTINGS_RESET");

        Debug.Log("[SettingsUI] Localized text updated");
    }

    #region Event Handlers

    private void OnMasterVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMasterVolume(value);
            UpdateVolumeLabel(masterVolumeLabel, value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicVolume(value);
            UpdateVolumeLabel(musicVolumeLabel, value);
        }
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSfxVolume(value);
            UpdateVolumeLabel(sfxVolumeLabel, value);
        }
    }

    private void OnUIVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetUIVolume(value);
            UpdateVolumeLabel(uiVolumeLabel, value);
        }
    }

    private void OnResolutionChanged(int index)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetResolution(index);
        }
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetFullscreen(isFullscreen);
        }
    }

    private void OnLanguageChanged(int index)
    {
        if (SettingsManager.Instance != null)
        {
            SystemLanguage[] supportedLanguages = SettingsManager.Instance.GetSupportedLanguages();
            if (index >= 0 && index < supportedLanguages.Length)
            {
                SettingsManager.Instance.SetLanguage(supportedLanguages[index]);
                
                // Update GoogleSheetLoader language and refresh UI text
                if (GoogleSheetLoader.Instance != null)
                {
                    // Convert SystemLanguage to GoogleSheetLoader.Language
                    GoogleSheetLoader.Language newLanguage = GoogleSheetLoader.Language.Korean;
                    switch (supportedLanguages[index])
                    {
                        case SystemLanguage.Korean:
                            newLanguage = GoogleSheetLoader.Language.Korean;
                            break;
                        case SystemLanguage.English:
                            newLanguage = GoogleSheetLoader.Language.English;
                            break;
                        case SystemLanguage.Japanese:
                            newLanguage = GoogleSheetLoader.Language.Japanese;
                            break;
                    }
                    
                    GoogleSheetLoader.Instance.ChangeLanguage(newLanguage);
                    
                    // Update localized text after language change
                    Invoke(nameof(UpdateLocalizedText), 1f);
                }
            }
        }
    }

    #endregion

    #region Public Methods - Main Interface

    public void ToggleSettings()
    {
        if (isSettingsOpen)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            isSettingsOpen = true;

            // Pause game if this is in-game settings
            if (pauseGameOnSettings)
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            
            // Refresh UI when opening
            if (SettingsManager.Instance != null)
            {
                UpdateUI(SettingsManager.Instance.currentSettings);
            }

            Debug.Log("[SettingsUI] Settings opened");
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isSettingsOpen = false;

            // Resume game if this was in-game settings
            if (pauseGameOnSettings)
            {
                // 안전한 timeScale 복원
                if (originalTimeScale <= 0f)
                {
                    originalTimeScale = 1f; // 기본값으로 복원
                    Debug.LogWarning("[SettingsUI] originalTimeScale이 비정상적입니다. 1.0으로 복원합니다.");
                }
                Time.timeScale = originalTimeScale;
                Debug.Log($"[SettingsUI] Time.timeScale을 {originalTimeScale}로 복원했습니다.");
            }

            Debug.Log("[SettingsUI] Settings closed");
        }
    }

    public void ApplySettings()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ApplyAllSettings();
            Debug.Log("[SettingsUI] Settings applied");
        }
    }

    public void SaveSettings()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
            Debug.Log("[SettingsUI] Settings saved");
        }
    }

    public void ResetSettings()
    {
        if (SettingsManager.Instance != null)
        {
            // Reset to default settings
            SettingsManager.Instance.currentSettings = new GameSettings();
            SettingsManager.Instance.ApplyAllSettings();
            Debug.Log("[SettingsUI] Settings reset to default");
        }
    }

    #endregion

    #region Configuration Methods

    public void ConfigureForMainMenu()
    {
        enableHotkeyInGame = false;
        pauseGameOnSettings = false;
        Debug.Log("[SettingsUI] Configured for Main Menu");
    }

    public void ConfigureForInGame()
    {
        enableHotkeyInGame = true;
        pauseGameOnSettings = true;
        Debug.Log("[SettingsUI] Configured for In-Game");
    }

    #endregion

    #region Auto-save handlers

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }
    }

    #endregion

    #region Test Methods (for debugging)

    [ContextMenu("Test Open Settings")]
    public void TestOpenSettings()
    {
        OpenSettings();
    }

    [ContextMenu("Test Close Settings")]
    public void TestCloseSettings()
    {
        CloseSettings();
    }

    [ContextMenu("Configure For Main Menu")]
    public void TestConfigureForMainMenu()
    {
        ConfigureForMainMenu();
    }

    [ContextMenu("Configure For In-Game")]
    public void TestConfigureForInGame()
    {
        ConfigureForInGame();
    }

    #endregion
} 