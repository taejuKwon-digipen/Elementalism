using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Object/Inventory")]
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
            Debug.Log($"카드 추가됨: {card.CardName} (ID: {card.ID})");
        }
    }
} 