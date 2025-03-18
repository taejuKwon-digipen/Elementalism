using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Button healButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healCostText;
    [SerializeField] private Button worldMapButton;  // 월드맵 버튼
    
    [Header("Settings")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private int healCost = 50;
    [SerializeField] private int cardCost = 100;
    
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardItemSO cardDatabase;
    [SerializeField] private EnemyManager enemyManager;

    private List<Card> shopCards = new List<Card>();
    private bool isShopOpen = false;  // 상점 열림 상태 추적

    private void Start()
    {
        // CardManager 참조 확인
        if (cardManager == null)
        {
            cardManager = FindObjectOfType<CardManager>();
            if (cardManager != null)
            {
                Debug.Log("[ShopManager] CardManager 참조를 찾았습니다.");
            }
            else
            {
                Debug.LogError("[ShopManager] CardManager를 찾을 수 없습니다!");
            }
        }

        shopPanel.SetActive(false);
        healButton.onClick.AddListener(OnHealButtonClicked);
        UpdateUI();

        if (worldMapButton != null)
        {
            worldMapButton.onClick.AddListener(OnWorldMapButtonClick);
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        GenerateShopCards();
        UpdateUI();
    }

    private void Update()
    {
        if(!isShopOpen && enemyManager.IsAllEnemiesDefeated())
        {
            OpenShop();
            isShopOpen = true;  // 상점이 열렸음을 표시
        }
    }

    private void GenerateShopCards()
    {
        // 기존 카드 제거
        foreach (var card in shopCards)
        {
            Destroy(card.gameObject);
        }
        shopCards.Clear();

        // 언락된 카드만 필터링
        var unlockedCards = cardDatabase.items.Where(card => card.IsUnlocked).ToList();
        
        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[ShopManager] 언락된 카드가 없습니다!");
            return;
        }

        // 새로운 카드 3개 생성 (언락된 카드 중에서만)
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, unlockedCards.Count);
            CardItem cardItem = unlockedCards[randomIndex];
            
            var cardObj = Instantiate(cardManager.cardPrefab, cardContainer);
            var card = cardObj.GetComponent<Card>();
            
            // 상점용 카드 설정
            card.Setup(cardItem, true);
            card.enabled = false;  // Card 컴포넌트 비활성화하여 OnPointerDown 이벤트 방지
            
            // 구매 버튼 추가
            var button = cardObj.AddComponent<Button>();
            int cardIndex = i; // 클로저를 위한 변수
            button.onClick.AddListener(() => {
                Debug.Log($"[ShopManager] 상점 카드 클릭됨: {cardItem.CardName}");
                PurchaseCard(cardIndex);
            });
            
            shopCards.Add(card);
        }
    }

    private void PurchaseCard(int index)
    {
        Debug.Log($"[ShopManager] 카드 구매 시도: 인덱스 {index}");
        
        if (player.Gold >= cardCost && index < shopCards.Count)
        {
            CardItem purchasedCard = shopCards[index].carditem;
            Debug.Log($"[ShopManager] 카드 구매 시작: {purchasedCard.CardName} (ID: {purchasedCard.ID})");
            
            // 골드 차감
            player.Gold -= cardCost;
            Debug.Log($"[ShopManager] 골드 차감 완료. 남은 골드: {player.Gold}");
            
            // 카드를 인벤토리에 추가
            if (cardManager != null)
            {
                Debug.Log($"[ShopManager] CardManager 참조 확인됨. AddCardToInventory 호출 전");
                cardManager.AddCardToInventory(purchasedCard);
                Debug.Log($"[ShopManager] AddCardToInventory 호출 완료");
            }
            else
            {
                Debug.LogError("[ShopManager] CardManager 참조가 null입니다! Start 메서드에서 초기화되지 않았을 수 있습니다.");
                return;
            }
            
            // UI 정리
            Destroy(shopCards[index].gameObject);
            shopCards.RemoveAt(index);
            UpdateUI();
            
            Debug.Log($"[ShopManager] 카드 구매 프로세스 완료: {purchasedCard.CardName}");
        }
        else
        {
            Debug.Log($"[ShopManager] 골드 부족 또는 잘못된 인덱스! 필요: {cardCost}, 현재: {player.Gold}, 인덱스: {index}");
        }
    }

    private void OnHealButtonClicked()
    {
        if (player.Gold >= healCost)
        {
            player.Gold -= healCost;
            player.HealHP(healAmount);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        goldText.text = $"Gold: {player.Gold}";
        healCostText.text = $"Heal ({healAmount} HP) - {healCost} Gold";
    }

    // 상점을 닫을 때 isShopOpen 리셋
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;  // 다음 라운드를 위해 리셋
    }

    private void OnWorldMapButtonClick()
    {
        Debug.Log("Loading World Map scene...");
        SceneManager.LoadScene("Map2");  // "WorldMap"은 맵 씬의 이름입니다
    }
}