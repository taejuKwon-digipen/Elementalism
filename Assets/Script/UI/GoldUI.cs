using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    private Player player;
    private int lastGold;

    private void Start()
    {
        // GoldText 컴포넌트를 자동으로 찾기
        if (goldText == null)
        {
            goldText = GetComponentInChildren<TextMeshProUGUI>();
            if (goldText == null)
            {
                Debug.LogError("[GoldUI] GoldText 컴포넌트를 찾을 수 없습니다!");
                return;
            }
        }

        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("[GoldUI] Player를 찾을 수 없습니다!");
            return;
        }
        lastGold = player.Gold;
        UpdateGoldText();
    }

    private void Update()
    {
        if (player != null && player.Gold != lastGold)
        {
            lastGold = player.Gold;
            UpdateGoldText();
        }
    }

    private void UpdateGoldText()
    {
        if (player != null && goldText != null)
        {
            goldText.text = $"{player.Gold}G";
            Debug.Log($"[GoldUI] 골드 업데이트: {player.Gold}G");
        }
    }
} 