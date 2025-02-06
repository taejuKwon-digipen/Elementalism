using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Button healButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healCostText;
    
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
        shopPanel.SetActive(false);
        healButton.onClick.AddListener(OnHealButtonClicked);
        UpdateUI();
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

        // 새로운 카드 3개 생성
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, cardDatabase.items.Length);
            CardItem cardItem = cardDatabase.items[randomIndex];
            
            var cardObj = Instantiate(cardManager.cardPrefab, cardContainer);
            var card = cardObj.GetComponent<Card>();
            card.Setup(cardItem, true);
            
            // 구매 버튼 추가
            var button = cardObj.AddComponent<Button>();
            int cardIndex = i; // 클로저를 위한 변수
            button.onClick.AddListener(() => PurchaseCard(cardIndex));
            
            shopCards.Add(card);
        }
    }

    private void PurchaseCard(int index)
    {
        if (player.Gold >= cardCost)
        {
            player.Gold -= cardCost;
            cardManager.AddCardToBuffer(shopCards[index].carditem);
            Destroy(shopCards[index].gameObject);
            shopCards.RemoveAt(index);
            UpdateUI();
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
}