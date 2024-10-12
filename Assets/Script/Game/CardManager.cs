using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] CardStat cardstat;

    List<Card> CardBuffer;

    public Card PopCard()
    {
        if(CardBuffer.Count == 0)
        {
            SetCardBuffer();
        }

        Card card = CardBuffer[0];
        CardBuffer.RemoveAt(0);
        return card;
    }

    void SetCardBuffer()
    {
        CardBuffer = new List<Card>();

        for(int i =0; i< cardstat.cards.Length; i++)
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
    void Start()
    {
        SetCardBuffer();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            print(PopCard().CardName);
        }
    }
}
