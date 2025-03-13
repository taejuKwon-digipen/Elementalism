using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory")]
public class Inventory : ScriptableObject
{
    public List<CardItem> unlockedCards = new List<CardItem>();

    private void OnEnable()
    {
        if (unlockedCards.Count == 0)
        {
            // 기본 카드 4개 추가 (ID: 1,2,3,4)
            for (int i = 1; i <= 4; i++)
            {
                unlockedCards.Add(new CardItem { ID = i, IsUnlocked = true });
            }
        }
    }

    public void AddCard(CardItem card)
    {
        if (!unlockedCards.Exists(x => x.ID == card.ID))
        {
            unlockedCards.Add(card);
            Debug.Log($"[Inventory] 카드 추가됨: {card.CardName} (ID: {card.ID})");
        }
        else
        {
            Debug.Log($"[Inventory] 카드가 이미 존재함: {card.CardName} (ID: {card.ID})");
        }
    }

    public void RemoveCard(CardItem card)
    {
        if (unlockedCards.RemoveAll(x => x.ID == card.ID) > 0)
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
        return unlockedCards.Exists(x => x.ID == cardId);
    }

    public void ClearInventory()
    {
        unlockedCards.Clear();
        Debug.Log("[Inventory] 인벤토리가 초기화되었습니다.");
    }
} 