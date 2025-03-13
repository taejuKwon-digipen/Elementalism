using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour
{
    private List<CardItem> deckCards = new List<CardItem>(); // 덱에 있는 카드들
    private List<CardItem> discardPile = new List<CardItem>(); // 사용한 카드들이 쌓이는 더미
    [SerializeField] private int maxDeckSize = 20; // 덱의 최대 크기
    
    // 덱 초기화 (인벤토리에서 카드 가져오기)
    public void InitializeDeck(List<CardItem> availableCards)
    {
        Debug.Log("[Deck] 덱 초기화 시작");
        deckCards.Clear();

        // 사용 가능한 카드가 없으면 초기화 중단
        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogWarning("[Deck] 사용 가능한 카드가 없습니다!");
            return;
        }

        // 랜덤하게 카드 선택하여 덱 구성
        int cardsToAdd = Mathf.Min(maxDeckSize, availableCards.Count);
        List<CardItem> shuffledCards = availableCards.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < cardsToAdd; i++)
        {
            deckCards.Add(shuffledCards[i]);
            Debug.Log($"[Deck] 카드 추가됨: {shuffledCards[i].CardName} (ID: {shuffledCards[i].ID})");
        }

        Debug.Log($"[Deck] 덱 초기화 완료. 총 {deckCards.Count}장의 카드가 덱에 있습니다.");
    }
    
    // 덱 섞기
    public void ShuffleDeck()
    {
        deckCards = deckCards.OrderBy(x => Random.value).ToList();
        Debug.Log("[Deck] 덱이 섞였습니다.");
    }
    
    // 카드 뽑기
    public CardItem DrawCard()
    {
        if (deckCards.Count == 0)
        {
            Debug.LogWarning("[Deck] 덱에 카드가 없습니다!");
            return null;
        }

        CardItem drawnCard = deckCards[0];
        deckCards.RemoveAt(0);
        Debug.Log($"[Deck] 카드를 뽑았습니다: {drawnCard.CardName} (ID: {drawnCard.ID})");
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

    public List<CardItem> GetDeckCards()
    {
        return new List<CardItem>(deckCards);
    }
}
