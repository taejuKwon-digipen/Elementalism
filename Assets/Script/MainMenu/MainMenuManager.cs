using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Integration")]
    public SettingsUI settingsUI;

    private void Start()
    {
        // Configure SettingsUI for main menu
        if (settingsUI != null)
        {
            settingsUI.ConfigureForMainMenu();
        }

        // Initialize settings if not already done
        if (SettingsManager.Instance == null)
        {
            // Create SettingsManager if it doesn't exist
            GameObject settingsManagerGO = new GameObject("SettingsManager");
            settingsManagerGO.AddComponent<SettingsManager>();
        }
    }

    public void OnNewGameButtonClick()
    {
        SceneManager.LoadScene("Map2");
    }

    public void OnContinueButtonClick()
    {
        SceneManager.LoadScene("Map2");
    }

    public void OnSettingButtonClick()
    {
        if (settingsUI != null)
        {
            settingsUI.OpenSettings();
            Debug.Log("[MainMenuManager] Settings opened");
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] SettingsUI not assigned!");
        }
    }

    public void OnQuitButtonClick()
    {
        // Save settings before quitting
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 