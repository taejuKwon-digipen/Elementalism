using UnityEngine;
using TMPro;

public class CardUsageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usageText;
    private const int MAX_CARDS_PER_TURN = 3;
    private int currentUsedCards = 0;

    private void Start()
    {
        if (usageText == null)
        {
            usageText = GetComponentInChildren<TextMeshProUGUI>();
            if (usageText == null)
            {
                Debug.LogError("[CardUsageUI] UsageText 컴포넌트를 찾을 수 없습니다!");
                return;
            }
            Debug.Log("[CardUsageUI] UsageText 컴포넌트 연결 완료");
        }
        UpdateUsageText();
    }

    public void OnCardUsed()
    {
        Debug.Log($"[CardUsageUI] 카드 사용 호출됨. 현재 사용 카드: {currentUsedCards}/{MAX_CARDS_PER_TURN}");
        if (currentUsedCards < MAX_CARDS_PER_TURN)
        {
            currentUsedCards++;
            UpdateUsageText();
            Debug.Log($"[CardUsageUI] 카드 사용 카운트 증가: {currentUsedCards}/{MAX_CARDS_PER_TURN}");
        }
        else
        {
            Debug.Log("[CardUsageUI] 최대 카드 사용 횟수에 도달했습니다!");
        }
    }

    public void ResetUsage()
    {
        Debug.Log("[CardUsageUI] 카드 사용 카운트 초기화");
        currentUsedCards = 0;
        UpdateUsageText();
    }

    private void UpdateUsageText()
    {
        if (usageText != null)
        {
            usageText.text = $"{currentUsedCards}/{MAX_CARDS_PER_TURN}";
            
            // 제한에 도달하면 텍스트 색상 변경
            if (currentUsedCards >= MAX_CARDS_PER_TURN)
            {
                usageText.color = Color.red;
                Debug.Log("[CardUsageUI] 최대 사용 횟수 도달 - 텍스트 색상 빨간색으로 변경");
            }
            else
            {
                usageText.color = Color.white;
            }
        }
        else
        {
            Debug.LogError("[CardUsageUI] UsageText가 null입니다!");
        }
    }
} 