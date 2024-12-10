using DG.Tweening; // 애니메이션 처리를 위한 DOTween 라이브러리
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting; // 유니티 비주얼 스크립팅 기능
using UnityEngine;
using UnityEngine.UI;// UI 관련 기능을 위한 네임스페이스

public class CardManager : MonoBehaviour
{
    // 싱글톤 패턴: CardManager의 인스턴스
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;// 싱글톤 초기화

    public int currentCardIndex = 0;// 현재 카드의 인덱스

    [SerializeField] CardItemSO carditemso; // 카드 데이터베이스 (ScriptableObject 사용)
    [SerializeField] GameObject cardPrefab; // 카드 생성에 사용될 프리팹

    [SerializeField] public List<Card> UsingCard; // 사용 중인 카드 리스트 (오른쪽 3개)
    [SerializeField] public List<Card> WaitingCard; // 대기 중인 카드 리스트 (왼쪽 4개)

    public Transform Canvas2Transform; // UI 캔버스의 트랜스폼

    // 카드 위치 상태 리스트 (false = 비어있음, true = 사용 중)
    private List<bool> positionOccupied;

    List<CardItem> ItemBuffer; // 카드 데이터를 저장하는 버퍼

    int currenttrueindex = 0; // 현재 비어있는 위치의 인덱스
    bool isUse = false; // 현재 사용 중인지 여부
    Card Waitingcard_ = null; // 대기 중인 카드 참조

    [SerializeField] private GameObject cardSelectionPanel; // 카드 선택 패널
    [SerializeField] private GameObject PanelBackground; // 패널 배경
    [SerializeField] private Transform selectionPanelContent; // 선택 패널의 콘텐츠 영역

    private Card clickedCard; // 클릭된 카드 참조
    int positionOCIndex = 0;

    //오른쪽 카드 3개 위치
    private List<Vector3> cardPosition = new List<Vector3>
    {
        new Vector3(7,3f,0),
        new Vector3(7,0,0),
        new Vector3(7,-3f, 0)
    };

    public int CurrCardIndexForSwitch = 0;

    //수정중 - 버튼 클릭 시 패널 닫히게
    public void XButtonClicked()
    {
       OpenCardSelectionPanel(false);
    }

    //왼쪽 대기 카드 4개 위치
    private List<Vector3> WaitingCardPosition = new List<Vector3> // Canvas로 옯기기
    {
        new Vector3(-6,0,0),
        new Vector3(-2, 0, 0),
        new Vector3(2, 0, 0),
        new Vector3(6, 0, 0),
    };

    int countwaitcard = 0; // 현재 대기 카드의 개수

    //카드 버퍼에서 카드 뽑아오기
    public CardItem PopCard()
    {
        if(ItemBuffer.Count <= 0)  // 버퍼가 비어있으면 새로 채움
        {
            SetCardBuffer(); // 카드 버퍼 설정
        }

        CardItem card = ItemBuffer[0]; // 첫 번째 카드 가져오기
        ItemBuffer.RemoveAt(0); // 가져온 카드는 버퍼에서 제거
        return card;
    }

    // 카드 버퍼 생성 메서드 (100개의 카드 생성)
    void SetCardBuffer()
    {
        ItemBuffer = new List<CardItem>();

        // 카드 데이터베이스에서 Percent 값에 따라 버퍼 생성
        for (int i = 0; i < carditemso.items.Length; i++)
        {
            CardItem cardditem = carditemso.items[i]; ;
            for (int j = 0; j < cardditem.Percent; j++)
            {
                ItemBuffer.Add(cardditem);
            }
        }

        // 버퍼 섞기 (랜덤 정렬)
        for (int i = 0; i <ItemBuffer.Count; i++)
        {
            int rand = Random.Range(i, ItemBuffer.Count);
            CardItem temp = ItemBuffer[i];
            ItemBuffer[i] = ItemBuffer[rand];
            ItemBuffer[rand] = temp;
        }
    }
    // 게임 시작 시 초기화
    public void Start()
    {
        SetCardBuffer(); // 카드 버퍼 설정

        cardSelectionPanel.SetActive(false); // 카드 선택 패널 비활성화
        PanelBackground.SetActive(false); // 배경 비활성화

        positionOccupied = new List<bool> { false, false, false }; //false = empty, true = ocuppied
        AddEmptyUsingCard(); // 사용 카드 3개 추가
   
    }


    // 오른쪽 사용 빈 카드 3개 추가 메서드
    public void AddEmptyUsingCard()
    {
        //Using Card 선택시 positionOccupied 가 false이면 비어있으니까 카드 넣기
        for (currenttrueindex = 0; currenttrueindex < 3; currenttrueindex++)
        {
            print(PopCard().CardName + " - Using Card");// 카드 이름 출력
            AddCard(true);// 사용 카드 추가
            positionOccupied[currenttrueindex] = false;  // 위치 점유 상태 업데이트
        }

        currenttrueindex = 0;
    }


    // 카드 선택 패널 열기/닫기
    private void OpenCardSelectionPanel(bool open)
    {
        if (open == true)
        {
            cardSelectionPanel.SetActive(true); // 패널 활성화
            PanelBackground.SetActive(true); // 배경 활성화
            GenerateSelectionCards();// 대기 카드 생성
        }
        else
        {
            cardSelectionPanel.SetActive(false); // 패널 비활성화
            PanelBackground.SetActive(false); // 배경 비활성화
        }
        
    }

    //패널이 열리면 카드 생성 (Waiting Card 4개)
    private void GenerateSelectionCards()
    {

        for (int i = 0; i < 4; i++)
        {
            countwaitcard = i; // 현재 대기 카드 인덱스
            AddCard(false); // 대기 카드로 추가

        } //처음 4개 나오게
        countwaitcard = 0; // 대기 카드 초기화

    }

    // 턴 종료 시 사용 카드 초기화 및 새 카드 추가
    public void TurnEndButton()
    {
        Debug.Log("TrunEndButton");

        for(int i = 0; i<4; i++)
        {
            if (WaitingCard[i].gameObject.activeSelf == false)
            {
                WaitingCard[i].gameObject.SetActive(true);
            }
            Destroy(WaitingCard[i].gameObject);
        }

        for(int i = 0; i < 3; i++)
        {
            Destroy(UsingCard[i].gameObject);
            if(positionOccupied[i] == true)
            {
                positionOccupied[i] = false;
            }
        }
        WaitingCard.Clear();
        UsingCard.Clear();
        AddEmptyUsingCard();
    }

    public void Update()
    {

    }

    //Using Card 3개 클릭 인식
    private int CalculateUsingCard(Card card)
    {
        float y = card.currentMousePosition.y;

        if (y >= 700f)
        {
            CurrCardIndexForSwitch = 0;
        }
        else if (y <= 410f)
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
            positionOCIndex = CurrCardIndexForSwitch;
            if (positionOccupied[positionOCIndex] == true)
            {
                ReChoose(positionOCIndex);
            }
            OpenCardSelectionPanel(true);
            Destroy(card.gameObject);
            //card.gameObject.SetActive(false);
        }
        else
        {
            ToBeSwitchCard(card); //일단 보류
            SwitchCard(Waitingcard_);
            card.gameObject.SetActive(false);
            Debug.Log("셋액티브 폴스로 됨");
        }
    }

    private void ReChoose(int PositionOCIndex)
    {
        for(int i = 0; i < 3; i++)
        {
            if(WaitingCard[i].gameObject.activeSelf == false && WaitingCard[i] == UsingCard[PositionOCIndex])
            {
                WaitingCard[i].gameObject.SetActive(true);
                Destroy(UsingCard[PositionOCIndex].gameObject);
            }
        }
    }

    //카드 바꾸기 위해서 WaitingCard포지션에서 마우스포인터와 가까운 카드를 찾아서 Card Waitingcard_를 넘겨줌 
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

    //ToBeSwitchCard에서 받은 카드를 UsingCard에 넣고 오브젝트 만들기
    private void SwitchCard(Card card) // card = Waitingcard_
    {
        currentCardIndex = CurrCardIndexForSwitch;
        Transform Canvas2Transform = GameObject.Find("Canvas/Background").transform;
        GameObject cardObject = Instantiate(cardPrefab, cardPosition[CurrCardIndexForSwitch], Quaternion.identity, Canvas2Transform);
        var newcard = cardObject.GetComponent<Card>();
        newcard.Setup(card.carditem, true); // 필요에 따라 `isUse` 값을 조정
        UsingCard[CurrCardIndexForSwitch] = newcard; // 생성된 카드를 리스트에 추가
        positionOccupied[CurrCardIndexForSwitch] = true;
        //WaitingCard.Clear(); 
        OpenCardSelectionPanel(false);
    }

    //카드 오브젝트 추가
    void AddCard(bool isUse)
    {
        if (isUse == true) //Using card
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
