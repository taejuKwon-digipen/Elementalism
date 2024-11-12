using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    public int currentCardIndex = 0;

    [SerializeField] CardItemSO carditemso;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] List<Card> UsingCard;
    [SerializeField] List<Card> WaitingCard;
/*
    [SerializeField] private Transform centerPosition; // 반원의 중심 위치
    [SerializeField] private float radius = 3.0f; // 반원의 반지름*/

    public Transform Canvas2Transform;

    private List<bool> positionOccupied = new List<bool> { true, true, true }; //true = empty, false = ocuppied

    List<CardItem> ItemBuffer;

    int currenttrueindex = 0;
    bool isUse = false;
    Card Waitingcard_ = null;

    [SerializeField] private GameObject cardSelectionPanel;
    [SerializeField] private Transform selectionPanelContent; // 선택 패널의 콘텐츠 영역

    private Card clickedCard; // 클릭된 카드 참조

    private List<Vector3> cardPosition = new List<Vector3>
    {
        new Vector3(1746,850,0),
        new Vector3(1746,540,0),
        new Vector3(1746,230,0),
    };

    public int CurrCardIndexForSwitch = 0;



    private List<Vector3> WaitingCardPosition = new List<Vector3>
    {
        new Vector3(364,540,0),
        new Vector3(728, 540, 0),
        new Vector3(1092, 540, 0),
        new Vector3(1456, 540, 0),

    };

    int countwaitcard = 0;

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

        cardSelectionPanel.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j ++)
            {
                if (positionOccupied[j] == true)
                {
                    currenttrueindex = j;
                    break;
                }
            }
            print(PopCard().CardName + " - Using Card");
            AddCard(true);
            positionOccupied[currenttrueindex] = false;
        } //처음 3개 나오게
    }

    public void OnCardClicked(Card card)
    {
        if(card == UsingCard[0] || card == UsingCard[1] || card == UsingCard[2])
        {
            clickedCard = card; // 클릭된 카드 저장
            OpenCardSelectionPanel();
        }
    }

    private void OpenCardSelectionPanel()
    {
        // 패널 활성화
        cardSelectionPanel.SetActive(true);

        // 선택 가능한 카드 목록 생성
        GenerateSelectionCards();
    }

    private void GenerateSelectionCards()
    {
        // 이전에 생성된 카드가 있다면 모두 삭제
        foreach (Transform child in selectionPanelContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            print(PopCard().CardName + " - Waiting Card");
            countwaitcard = i;
            AddCard(false);
            
        } //처음 3개 나오게
        countwaitcard = 0;

    }

    public void Update()
    {
       
    }

    public void NotifyCardSelection(Card card)
    {
        //if card == using card[0]---이면 GenerateSelectionCards() panel true
        //아니고 waitingcard면 switch

        if(card == UsingCard[0] || card == UsingCard[1] || card == UsingCard[2] )
        {
            int index = cardPosition.FindIndex(pos => Vector3.Distance(pos, card.currentMousePosition) < 150f);
            CurrCardIndexForSwitch = index;
            cardSelectionPanel.SetActive(true);
            GenerateSelectionCards();
        }
        else
        {
            ToBeSwitchCard(card);
            SwitchCard(card);
        }
    }

    private Card ToBeSwitchCard(Card card)
    {
        int index = WaitingCardPosition.FindIndex(pos => Vector3.Distance(pos, card.currentMousePosition) < 150f);

        if( index >=0)
        {
           return Waitingcard_ = WaitingCard[index];
        }
        else
        {
            Debug.Log("index error");
            return null;
        }
    }

    private void SwitchCard(Card card)
    {
        Destroy(UsingCard[CurrCardIndexForSwitch]); //UsingCard 삭제
        currentCardIndex = CurrCardIndexForSwitch;
        Transform Canvas2Transform = GameObject.Find("Canvas2").transform;
        GameObject cardObject = Instantiate(cardPrefab, cardPosition[currenttrueindex], Quaternion.identity, Canvas2Transform);
        card = cardObject.GetComponent<Card>();
        card.Setup(PopCard(), true); // 필요에 따라 `isUse` 값을 조정
        UsingCard.Add(card); // 생성된 카드를 리스트에 추가

    }

    void AddCard(bool isUse)
    {
        if (isUse == true)
        {
            Transform Canvas2Transform = GameObject.Find("Canvas2").transform;
            GameObject cardObject = Instantiate(cardPrefab, cardPosition[currenttrueindex], Quaternion.identity, Canvas2Transform);
            var card = cardObject.GetComponent<Card>();
            card.Setup(PopCard(), true); // 필요에 따라 `isUse` 값을 조정
            UsingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
        else
        {
            Transform Canvas2Transform = GameObject.Find("CardSelectionPanel").transform;
            GameObject cardObject = Instantiate(cardPrefab, WaitingCardPosition[countwaitcard], Quaternion.identity, Canvas2Transform);
            var card = cardObject.GetComponent<Card>();
            card.Setup(PopCard(), true); // 필요에 따라 `isUse` 값을 조정
            WaitingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
       
    }


    /*private void DisplayCardInArc()
    {
        int cardCount = cards.Count;
        float angleStep = 120f / (cardCount - 1);
        float startAngle = -90f;

        for(int i = 0; i < cardCount; i++)
        {
            float angle = startAngle + i * angleStep;
            float radian = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환

            // 삼각함수를 이용해 x, y 좌표 계산
            float x = centerPosition.position.x + radius * Mathf.Cos(radian);
            float y = centerPosition.position.y + radius * Mathf.Sin(radian);

            Vector3 targetPosition = new Vector3(x, y, centerPosition.position.z);

            // 카드 위치와 회전 설정
            cards[i].transform.position = targetPosition;
            cards[i].transform.rotation = Quaternion.Euler(0, 0, angle); // 카드가 반원을 따라 회전하도록 설정
            cards[i].SetActive(true); // 카드 활성화
        }

    }*/
}
