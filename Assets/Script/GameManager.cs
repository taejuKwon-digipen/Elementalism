using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Player_HP;
    public int Player_Gold;
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Player_Gold = 100; // 초기 골드 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Player.inst != null)
        {
            Player_HP = Player.inst.HP;
            Player_Gold = Player.inst.Gold;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("게임 종료! 맵으로 돌아갑니다.");

            if (MapManager.Instance != null)
            {
                Debug.Log("Main 씬에서 MapManager가 살아 있음!");
            }
            else
            {
                Debug.Log("Main 씬에서 MapManager가 사라짐!");
            }

            SceneManager.LoadScene("Map2");

            // 씬 변경 후 MapManager 상태 확인
            if (MapManager.Instance != null)
            {
                Debug.Log("씬 변경 후에도 MapManager가 유지됨!");
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // 게임 오버 UI 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        // 게임 일시 정지
        Time.timeScale = 0f;
    }
}
