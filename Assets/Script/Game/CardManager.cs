using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    public int currentCardIndex = 0;

    [SerializeField] CardItemSO carditemso;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] List<Card> UsingCard;
    [SerializeField] List<Card> WaitingCard;


    List<CardItem> ItemBuffer;

    private List<Vector3> cardposition = new List<Vector3>
    {
        new Vector3(1746,850,0),
        new Vector3(1746,540,0),
        new Vector3(1746,230,0),
    };

    public CardItem PopCard()
    {
        if(ItemBuffer.Count == 0)
        {
            SetCardBuffer();
        }

        CardItem card = ItemBuffer[0];
        ItemBuffer.RemoveAt(0);
        return card;
    }

    void SetCardBuffer()
    {
        ItemBuffer = new List<CardItem>();

        for(int i = 0; i < carditemso.items.Length; i++)
        {
            CardItem cardditem = carditemso.items[i]; ;
            for (int j = 0; j < cardditem.Percent; j++)
            {
                ItemBuffer.Add(cardditem);
            }
        }

        for(int i = 0; i <ItemBuffer.Count; i++)
        {
            int rand = Random.Range(i, ItemBuffer.Count);
            CardItem temp = ItemBuffer[i];
            ItemBuffer[i] = ItemBuffer[rand];
            ItemBuffer[rand] = temp;
        }
    }
    public void Start()
    {
        SetCardBuffer(); 
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && currentCardIndex < cardposition.Count)
        {
            print(PopCard().CardName + " - Using Card");
            AddCard(true);
            currentCardIndex++;
            //print(PopCard().CardName);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            print(PopCard().CardName + " - Waiting Card");
            //AddCard(false);
            //print(PopCard().CardName);
        }
    }


    void AddCard(bool isUse) 
    {

        Transform canvasTransform = GameObject.Find("Canvas").transform;
        GameObject cardObject = Instantiate(cardPrefab, cardposition[currentCardIndex], Quaternion.identity, canvasTransform);
        var card = cardObject.GetComponent<Card>();
        card.Setup(PopCard(), true); // 필요에 따라 `isUse` 값을 조정
        UsingCard.Add(card); // 생성된 카드를 리스트에 추가
    }

    void SetOriginOrder(bool isuse)
    {
        int count = isuse ? UsingCard.Count : WaitingCard.Count; 

        for (int i = 0; i < count; i++)
        {
            var targetcard = isuse? UsingCard[i] : WaitingCard[i];
            targetcard?.GetComponent<OrderManager>().SetOriginOrder(i);
        }
    }

}
