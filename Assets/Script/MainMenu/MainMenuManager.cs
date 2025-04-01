using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
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
        // 설정 씬이 준비되면 여기에 구현
        Debug.Log("설정 메뉴는 아직 준비 중입니다.");
    }

    public void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 