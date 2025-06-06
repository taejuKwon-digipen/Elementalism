using UnityEngine;
using UnityEngine.SceneManagement;

public class Map2Manager : MonoBehaviour
{
    [Header("Settings Integration")]
    public SettingsUI settingsUI;

    private void Start()
    {
        // Configure SettingsUI for in-game
        if (settingsUI != null)
        {
            settingsUI.ConfigureForInGame();
            Debug.Log("[Map2Manager] SettingsUI가 인게임 모드로 설정되었습니다.");
        }
        else
        {
            // 자동으로 찾기 시도
            settingsUI = FindObjectOfType<SettingsUI>();
            if (settingsUI != null)
            {
                settingsUI.ConfigureForInGame();
                Debug.Log("[Map2Manager] SettingsUI를 자동으로 찾아서 인게임 모드로 설정했습니다.");
            }
            else
            {
                Debug.LogWarning("[Map2Manager] SettingsUI를 찾을 수 없습니다!");
            }
        }

        // Initialize settings if not already done
        if (SettingsManager.Instance == null)
        {
            // Create SettingsManager if it doesn't exist
            GameObject settingsManagerGO = new GameObject("SettingsManager");
            settingsManagerGO.AddComponent<SettingsManager>();
        }
    }
} 