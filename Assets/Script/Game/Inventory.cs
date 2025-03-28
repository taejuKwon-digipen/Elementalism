using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory")]
public class Inventory : ScriptableObject
{
    public List<CardItem> inventoryCards = new List<CardItem>();
    [SerializeField] private CardItemSO cardItemSO; // CardItemSO 참조 추가

    // 잠금 해제된 카드만 필터링하여 반환하는 프로퍼티
    public List<CardItem> unlockedCards => inventoryCards.Where(card => card.IsUnlocked).ToList();

    private void OnEnable()
    {
        // cardItemSO가 null이면 Resources에서 로드
        if (cardItemSO == null)
        {
            cardItemSO = Resources.Load<CardItemSO>("ItemSO");
            if (cardItemSO == null)
            {
                Debug.LogError("[Inventory] Resources/ItemSO를 찾을 수 없습니다!");
                return;
            }
        }

        ClearInventory();
        if (inventoryCards.Count == 0)
        {
            // 기본 카드 4개 추가 (ID: 1,2,3,4)
            for (int i = 1; i <= 4; i++)
            {
                // CardItemSO에서 해당 ID의 카드 정보 찾기
                var cardData = cardItemSO.items.FirstOrDefault(card => card.ID == i);
                if (cardData != null)
                {
                    inventoryCards.Add(new CardItem 
                    { 
                        ID = cardData.ID,
                        CardName = cardData.CardName,
                        CardDescription = cardData.CardDescription,
                        cardImage = cardData.cardImage,
                        PowerLeft = cardData.PowerLeft,
                        PowerRight = cardData.PowerRight,
                        CreatedElementType = cardData.CreatedElementType,
                        UseMagic = cardData.UseMagic,
                        Percent = cardData.Percent,
                        UseProp = cardData.UseProp,
                        UseBuff = cardData.UseBuff,
                        UseDraw = cardData.UseDraw,
                        UseImage = cardData.UseImage,
                        IsUnlocked = true,
                        cardShape = cardData.cardShape
                    });
                    Debug.Log($"[Inventory] 기본 카드 추가됨: {cardData.CardName} (ID: {cardData.ID})");
                }
                else
                {
                    Debug.LogError($"[Inventory] ID {i}에 해당하는 카드 데이터를 찾을 수 없습니다!");
                }
            }
        }
    }

    public void AddCard(CardItem card)
    {
        if (!inventoryCards.Exists(x => x.ID == card.ID))
        {
            inventoryCards.Add(card);
            Debug.Log($"[Inventory] 카드 추가됨: {card.CardName} (ID: {card.ID})");
        }
        else
        {
            Debug.Log($"[Inventory] 카드가 이미 존재함: {card.CardName} (ID: {card.ID})");
        }
    }

    public void RemoveCard(CardItem card)
    {
        if (inventoryCards.RemoveAll(x => x.ID == card.ID) > 0)
        {
            Debug.Log($"[Inventory] 카드 제거됨: {card.CardName} (ID: {card.ID})");
        }
        else
        {
            Debug.Log($"[Inventory] 제거할 카드를 찾을 수 없음: {card.CardName} (ID: {card.ID})");
        }
    }

    public bool HasCard(int cardId)
    {
        return inventoryCards.Exists(x => x.ID == cardId);
    }

    public void ClearInventory()
    {
        inventoryCards.Clear();
        Debug.Log("[Inventory] 인벤토리가 초기화되었습니다.");
    }
} 