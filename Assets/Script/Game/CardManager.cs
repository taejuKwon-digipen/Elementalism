using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }

    private void Awake() => Inst = this;

    [SerializeField] CardStat cardstat;

    List<Card> CardBuffer;

    void SetCardBuffer()
    {
        CardBuffer = new List<Card>();

        for(int i =0; i< cardstat.cards.Length; i++  )
        {
            Card card = cardstat.cards[i]; ;
            for (int j = 0; j < card.Percent; j++)
            {
                CardBuffer.Add(card);
            }
        }

        for(int i = 0; i <CardBuffer.Count; i++)
        {
            int rand = Random.Range(i, CardBuffer.Count);
            Card temp = CardBuffer[i];
            CardBuffer[i] = CardBuffer[rand];
            CardBuffer[rand] = temp;
        }
    }
}
