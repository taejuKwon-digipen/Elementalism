using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    public int currentCardIndex = 0;

    [SerializeField] CardItemSO carditemso;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] public List<Card> UsingCard;
    [SerializeField] public List<Card> WaitingCard;
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
    [SerializeField] private GameObject PanelBackground;
    [SerializeField] private Transform selectionPanelContent; // 선택 패널의 콘텐츠 영역

    private Card clickedCard; // 클릭된 카드 참조

    private List<Vector3> cardPosition = new List<Vector3>
    {
        new Vector3(7,3f,0),
        new Vector3(7,0,0),
        new Vector3(7,-3f, 0)
    };

    public int CurrCardIndexForSwitch = 0;

    public void XButtonClicked()
    {
        cardSelectionPanel.SetActive(false);
        PanelBackground.SetActive(false);
    }

    private List<Vector3> WaitingCardPosition = new List<Vector3> // Canvas로 옯기기
    {
        new Vector3(-6,0,0),
        new Vector3(-2, 0, 0),
        new Vector3(2, 0, 0),
        new Vector3(6, 0, 0),
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
        PanelBackground.SetActive(false);

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
        PanelBackground.SetActive(true);

        // 선택 가능한 카드 목록 생성
        GenerateSelectionCards();
    }

    private void GenerateSelectionCards()
    {
        if(WaitingCard.Count > 0)
        {
            return;
        }

        //선택가능카드목록생성
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

    private int CalculateUsingCard(Card card)
    {
        float y = card.currentMousePosition.y;

       if( y >= 650f )
        {
            CurrCardIndexForSwitch = 0;
        }
       else if( y <= 300f)
        {
            CurrCardIndexForSwitch = 2;
        }
        else
        {
            CurrCardIndexForSwitch = 1;
        }
        return CurrCardIndexForSwitch;
    }

    public void NotifyCardSelection(Card card)
    {
        //if card == using card[0]---이면 GenerateSelectionCards() panel true
        //아니고 waitingcard면 switch

        if(card == UsingCard[0] || card == UsingCard[1] || card == UsingCard[2] )
        {
            CalculateUsingCard(card);
            //int index = cardPosition.FindIndex(pos => Vector3.Distance(pos, card.currentMousePosition) < 150f); //여기가 문제임 포지션이 바뀌어서 인식을 못함
            cardSelectionPanel.SetActive(true);
            PanelBackground.SetActive(true);
            GenerateSelectionCards();
            Destroy(card.gameObject); //UsingCard 삭제
        }
        else
        {
            ToBeSwitchCard(card); //일단 보류
            SwitchCard(Waitingcard_);
            //WaitingCard.Remove(card);
            Destroy(card.gameObject);
        }
    }

    private Card ToBeSwitchCard(Card card)
    {

        Vector3 mousePosition = card.currentMousePosition; // 화면 좌표
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        Debug.Log("WC- WorldPosition :" + worldPosition);

        int index = WaitingCardPosition.FindIndex(pos => Vector3.Distance(pos, worldPosition) < 1.5f);

        if (index >= 0)
        {
            Debug.Log("Waitingcard Index: " + index);
            Waitingcard_ = WaitingCard[index];
        }
        else
        {
            Debug.Log("웨이팅카드 ToBeSwitchCard에서 에러");
            return null;
        }

        return null;
      
    }

    private void SwitchCard(Card card) // card = Waitingcard_
    {
        currentCardIndex = CurrCardIndexForSwitch;
        Transform Canvas2Transform = GameObject.Find("Canvas/Background").transform;
        GameObject cardObject = Instantiate(cardPrefab, cardPosition[CurrCardIndexForSwitch], Quaternion.identity, Canvas2Transform);
        var newcard = cardObject.GetComponent<Card>();
        newcard.Setup(card.carditem, true); // 필요에 따라 `isUse` 값을 조정
        UsingCard[CurrCardIndexForSwitch] = newcard; // 생성된 카드를 리스트에 추가
        //WaitingCard.Clear();
        cardSelectionPanel.SetActive(false);
        PanelBackground.SetActive(false);
    }

    void AddCard(bool isUse)
    {
        if (isUse == true) //얘가 오른쪽 3개
        {
            Transform Canvas2Transform = GameObject.Find("Canvas/Background").transform;
            GameObject cardObject = Instantiate(cardPrefab, cardPosition[currenttrueindex], Quaternion.identity, Canvas2Transform);
            var card = cardObject.GetComponent<Card>();
            card.Setup(PopCard(), false); // 필요에 따라 `isUse` 값을 조정
            UsingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
        else //얘가 뒤에 나오는 4개
        {
            if(WaitingCard.Count >=4)
            {
                return;
            }

            Transform Canvas2Transform = GameObject.Find("CardSelectionPanel").transform;
            GameObject cardObject = Instantiate(cardPrefab, WaitingCardPosition[countwaitcard], Quaternion.identity, Canvas2Transform);
            Vector3 nowLocalScale = cardObject.transform.localScale;
            cardObject.transform.localScale = new Vector3(nowLocalScale.x * 0.014f, nowLocalScale.x * 0.014f, 1);
            var card = cardObject.GetComponent<Card>();
            card.Setup(PopCard(), true); // 필요에 따라 `isUse` 값을 조정
            WaitingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
       
    }
}
