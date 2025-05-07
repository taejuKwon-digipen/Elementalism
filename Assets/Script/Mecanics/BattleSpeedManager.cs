using UnityEngine;
using UnityEngine.UI; // UI 버튼, 텍스트 사용 시
using TMPro; // TextMeshPro 사용 시
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public class BattleSpeedManager : MonoBehaviour
{
    public static BattleSpeedManager Instance { get; private set; }

    // [SerializeField] private Button speedButton; // Inspector에서 연결 -> 동적으로 찾도록 변경
    // [SerializeField] private TextMeshProUGUI speedText; // 현재 배속 표시 텍스트 (선택 사항) -> 동적으로 찾도록 변경
    private Button speedButton;
    private TextMeshProUGUI speedText;

    private float[] speedLevels = { 1f, 2f, 3f }; // 배속 단계 (1배, 2배, 3배)
    private int currentSpeedIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴하지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 구독 해제
    }

    void Start()
    {
        Debug.Log("[BattleSpeedManager] Start() 호출됨");
        SetupUIAndApplySpeed();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[BattleSpeedManager] OnSceneLoaded: {scene.name} 씬 로드됨");
        SetupUIAndApplySpeed();
    }

    void SetupUIAndApplySpeed()
    {
        Debug.Log("[BattleSpeedManager] SetupUIAndApplySpeed() 호출 시도");
        
        GameObject speedButtonObj = GameObject.Find("SpeedButton");
        if (speedButtonObj != null)
        {
            speedButton = speedButtonObj.GetComponent<Button>();
            if (speedButton != null)
            {
                Debug.Log("[BattleSpeedManager] SpeedButton 찾음 및 할당됨");
                speedButton.onClick.RemoveAllListeners();
                speedButton.onClick.AddListener(ToggleSpeed);
                Debug.Log("[BattleSpeedManager] SpeedButton 리스너 설정 완료");
            }
            else
            {
                Debug.LogWarning("[BattleSpeedManager] SpeedButton 오브젝트에서 Button 컴포넌트를 찾을 수 없습니다.");
                speedButton = null; // 명시적으로 null 처리
            }
        }
        else
        {
            speedButton = null;
            Debug.LogWarning("[BattleSpeedManager] SpeedButton 오브젝트를 찾을 수 없습니다. 현재 씬에 해당 이름의 활성화된 버튼이 있는지 확인하세요.");
        }

        GameObject speedTextObj = GameObject.Find("SpeedText");
        if (speedTextObj != null)
        {
            Debug.Log($"[BattleSpeedManager] SpeedText 오브젝트 '{speedTextObj.name}' 찾음");
            speedText = speedTextObj.GetComponent<TextMeshProUGUI>();
            if (speedText != null)
            {
                Debug.Log("[BattleSpeedManager] SpeedText (TextMeshProUGUI) 컴포넌트 찾음 및 할당됨");
            }
            else
            {
                Debug.LogWarning($"[BattleSpeedManager] SpeedText 오브젝트 '{speedTextObj.name}'에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
                speedText = null; // 명시적으로 null 처리
            }
        }
        else
        {
            speedText = null;
            Debug.LogWarning("[BattleSpeedManager] SpeedText 오브젝트를 찾을 수 없습니다. 현재 씬에 'SpeedText' 이름의 활성화된 TextMeshPro 오브젝트가 있는지 확인하세요.");
        }
        
        ApplySpeed(); // 항상 ApplySpeed를 호출하여 Time.timeScale은 적용되도록 함
    }

    public void ToggleSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedLevels.Length;
        Debug.Log("[BattleSpeedManager] ToggleSpeed() 호출됨. 새 배속 인덱스: " + currentSpeedIndex);
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        Time.timeScale = speedLevels[currentSpeedIndex];
        Debug.Log($"[BattleSpeedManager] ApplySpeed() 호출됨. Time.timeScale: {Time.timeScale}x");

        if (speedText != null)
        {
            speedText.text = $"{Time.timeScale}x";
            Debug.Log($"[BattleSpeedManager] SpeedText UI 업데이트: {speedText.text}");
        }
        else
        {
            Debug.LogWarning("[BattleSpeedManager] SpeedText 참조가 null이므로 UI 텍스트를 업데이트할 수 없습니다.");
        }
        // 오디오 피치 조절 (선택 사항)
        // AudioListener.pitch = Time.timeScale;
    }

    // OnDisable 대신 OnDestroy 사용 고려
    private void OnDestroy()
    {
        if (Instance == this) // 자기 자신이 Instance일 때만 초기화
        {
            if (Time.timeScale != 1f) // 불필요한 초기화를 방지하기 위해 현재 배속이 1이 아닐 때만 실행
            {
                Time.timeScale = 1f; // 정상 속도로 복원
                Debug.Log("[BattleSpeedManager] 배속 초기화 (1x) - OnDestroy");
            }
        }
    }
}