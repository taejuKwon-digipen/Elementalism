using UnityEngine;
using UnityEngine.UI; // UI 버튼, 텍스트 사용 시
using TMPro; // TextMeshPro 사용 시

public class BattleSpeedManager : MonoBehaviour
{
    public static BattleSpeedManager Instance { get; private set; }

    [SerializeField] private Button speedButton; // 배속 버튼 (Inspector에서 연결)
    [SerializeField] private TextMeshProUGUI speedText; // 현재 배속 표시 텍스트 (선택 사항)

    private float[] speedLevels = { 1f, 2f, 3f }; // 배속 단계 (1배, 2배, 3배)
    private int currentSpeedIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(ToggleSpeed);
        }
        ApplySpeed(); // 초기 배속 적용
    }

    public void ToggleSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedLevels.Length;
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        Time.timeScale = speedLevels[currentSpeedIndex];
        Debug.Log($"[BattleSpeedManager] 현재 배속: {Time.timeScale}x");

        if (speedText != null)
        {
            speedText.text = $"{Time.timeScale}x";
        }

        // 오디오 피치 조절 (선택 사항)
        // AudioListener.pitch = Time.timeScale; // 전체 오디오 피치 변경
        // 또는 각 AudioSource의 pitch를 개별적으로 조절: audioSource.pitch = Time.timeScale;
    }

    // 씬 전환 또는 배틀 종료 시 정상 속도로 복원
    // 만약 씬 간에 DontDestroyOnLoad를 사용한다면, 씬 전환 시점에 직접 호출해야 할 수 있습니다.
    private void OnDisable() // 또는 OnDestroy()
    {
        if (Instance == this) // 자기 자신이 Instance일 때만 초기화 (다른 인스턴스에 의해 파괴되는 경우 제외)
        {
             Time.timeScale = 1f; // 정상 속도로 복원
             Debug.Log("[BattleSpeedManager] 배속 초기화 (1x)");
        }
    }
}