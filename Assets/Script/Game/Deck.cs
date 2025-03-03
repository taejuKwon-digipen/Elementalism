using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private List<CardItem> deckCards = new List<CardItem>(); // 덱에 있는 카드들
    private List<CardItem> discardPile = new List<CardItem>(); // 사용한 카드들이 쌓이는 더미
    
    // 덱 초기화 (인벤토리에서 카드 가져오기)
    public void InitializeDeck(List<CardItem> inventoryCards)
    {
        deckCards.Clear();
        discardPile.Clear();
        
        // 인벤토리에서 카드 복사
        foreach (var card in inventoryCards)
        {
            deckCards.Add(card);
        }
        
        // 덱 섞기
        ShuffleDeck();
    }
    
    // 덱 섞기
    public void ShuffleDeck()
    {
        int n = deckCards.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            CardItem temp = deckCards[k];
            deckCards[k] = deckCards[n];
            deckCards[n] = temp;
        }
    }
    
    // 카드 뽑기
    public CardItem DrawCard()
    {
        // 덱이 비어있으면 버린 카드 더미를 덱으로 이동
        if (deckCards.Count == 0)
        {
            RefillDeckFromDiscard();
        }
        
        // 그래도 덱이 비어있으면 null 반환
        if (deckCards.Count == 0)
        {
            return null;
        }
        
        // 맨 위 카드 뽑기
        CardItem drawnCard = deckCards[0];
        deckCards.RemoveAt(0);
        return drawnCard;
    }
    
    // 카드를 버린 카드 더미에 추가
    public void AddToDiscard(CardItem card)
    {
        discardPile.Add(card);
    }
    
    // 버린 카드 더미를 덱으로 이동 (섞기)
    private void RefillDeckFromDiscard()
    {
        // 버린 카드 더미의 카드를 덱으로 이동
        deckCards.AddRange(discardPile);
        discardPile.Clear();
        
        // 덱 섞기
        ShuffleDeck();
    }
    
    // 덱에 남은 카드 수 반환
    public int GetDeckCount()
    {
        return deckCards.Count;
    }
    
    // 버린 카드 더미에 있는 카드 수 반환
    public int GetDiscardCount()
    {
        return discardPile.Count;
    }
}
