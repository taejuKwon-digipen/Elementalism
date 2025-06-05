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
    public int Player_MaxHP;
    public GameObject gameOverPanel;
    [SerializeField]
    private int challengeLevel = 1;
    public int ChallengeLevel
    {
        get => challengeLevel;
        set => challengeLevel = value;
    }
    public bool IsFirstPlay { get; private set; } = true;  // 첫 플레이 여부
    public bool IsTutorialMode { get; private set; } = false;  // 튜토리얼 모드 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Player_HP = 200;
            Player_Gold = 100; // 초기 골드 설정
            Player_MaxHP = 250; // 초기 최대체력 설정
            LoadGameState(); // 게임 상태 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartTutorial(); // 게임 시작 시 튜토리얼 시작
    }

    // 게임 상태 저장
    private void SaveGameState()
    {
        PlayerPrefs.SetInt("IsFirstPlay", IsFirstPlay ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 게임 상태 로드
    private void LoadGameState()
    {
        // 튜토리얼 테스트를 위해 IsFirstPlay를 강제로 true로 설정
        IsFirstPlay = true;
        Debug.Log($"[GameManager] LoadGameState - IsFirstPlay: {IsFirstPlay}");
    }

    // 튜토리얼 시작
    public void StartTutorial()
    {
        Debug.Log($"[GameManager] StartTutorial 호출됨 - IsFirstPlay: {IsFirstPlay}, ChallengeLevel: {ChallengeLevel}");
        if (IsFirstPlay && ChallengeLevel == 1)
        {
            IsTutorialMode = true;
            Debug.Log($"[GameManager] 튜토리얼 모드 시작. IsTutorialMode: {IsTutorialMode}");
        }
        else
        {
            IsTutorialMode = false; // 명시적으로 false로 설정
            Debug.Log($"[GameManager] 튜토리얼 시작 조건 불만족 - IsFirstPlay: {IsFirstPlay}, ChallengeLevel: {ChallengeLevel}. IsTutorialMode: {IsTutorialMode}");
        }
    }

    // 튜토리얼 완료
    public void CompleteTutorial()
    {
        IsTutorialMode = false;
        IsFirstPlay = false;
        SaveGameState();
        Debug.Log("[GameManager] 튜토리얼 완료");
    }

    public void SetChallengeLevel(int level)
    {
        if (level >= 1)
        {
            ChallengeLevel = level;
            Debug.Log($"챌린지 레벨이 {level}로 설정되었습니다. ");
            
            // 첫 플레이이고 레벨 1이면 튜토리얼 시작
            if (IsFirstPlay && level == 1)
            {
                StartTutorial();
            }
        }
        else
        {
            Debug.LogWarning($"잘못된 챌린지 레벨입니다: {level}");
        }
    }

    // Element Type을 가져오는 메서드
    public ElementType GetModifiedElementType(ElementType originalType)
    {
        if (ChallengeLevel == 1 && originalType == ElementType.Earth)
        {
            return ElementType.Fire;
        }
        return originalType;
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
