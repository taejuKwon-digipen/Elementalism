using DG.Tweening; // 애니메이션 처리를 위한 DOTween 라이브러리
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting; // 유니티 비주얼 스크립팅 기능
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private Button Xbutton;
    [SerializeField] private Transform selectionPanelContent; // 선택 패널의 콘텐츠 영역

    private List<CardMouseHandler> cardMouseHandlers = new List<CardMouseHandler>(); // 카드 마우스 핸들러 리스트

    private Card clickedCard; // 클릭된 카드 참조
    int RechooseIndex = 0;



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
        Debug.Log("X버튼 클릭");
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
    public CardItem PopCard(bool IsFront)
    {
        if (ItemBuffer.Count <= 0)  // 버퍼가 비어있으면 새로 채움
        {
            SetCardBuffer(); // 카드 버퍼 설정
        }

        if (IsFront == true)
        {
            CardItem card = ItemBuffer[0]; // 첫 번째 카드 가져오기
            ItemBuffer.RemoveAt(0); // 가져온 카드는 버퍼에서 제거
            return card;
        }
        else
        {
            CardItem card = null;
            return card;
        }
    }

    //사용하는 카드 버퍼 따로 만들기

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
        for (int i = 0; i < ItemBuffer.Count; i++)
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
        RegisterCardMouseHandlers();

        SetCardBuffer(); // 카드 버퍼 설정

        cardSelectionPanel.SetActive(false); // 카드 선택 패널 비활성화
        PanelBackground.SetActive(false); // 배경 비활성화

        positionOccupied = new List<bool> { false, false, false }; //false = empty, true = ocuppied
        AddEmptyUsingCard(); // 사용 카드 3개 추가

    }
    private void RegisterCardMouseHandlers()
    {
        CardMouseHandler[] handlers = FindObjectsOfType<CardMouseHandler>();

        foreach (var handler in handlers)
        {
            cardMouseHandlers.Add(handler);
            handler.SetCardManager(this); // CardMouseHandler에 CardManager 연결
        }
    }

    // 오른쪽 사용 빈 카드 3개 추가 메서드
    public void AddEmptyUsingCard()
    {
        //Using Card 선택시 positionOccupied 가 false이면 비어있으니까 카드 넣기
        for (currenttrueindex = 0; currenttrueindex < 3; currenttrueindex++)
        {
            //print(PopCard(false).CardName + " - Using Card");// 카드 이름 출력
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
            Xbutton.interactable = true;
            cardSelectionPanel.SetActive(true); // 패널 활성화
            PanelBackground.SetActive(true); // 배경 활성화
            GenerateSelectionCards();// 대기 카드 생성
        }
        else
        {
            Xbutton.interactable = false;
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
        StartCoroutine(ExecuteTurnEndButton());
    }

    private IEnumerator ExecuteTurnEndButton()
    {
        Debug.Log("Waiting for 3 seconds before executing TurnEndButton...");
        yield return new WaitForSeconds(3.0f); // 3초 딜레이

        Debug.Log("Executing TurnEndButton...");

        // WaitingCard 처리
        for (int i = 0; i < 4; i++)
        {
            if (WaitingCard[i].gameObject.activeSelf == false)
            {
                WaitingCard[i].gameObject.SetActive(true);
            }
            Destroy(WaitingCard[i].gameObject);
        }

        // UsingCard 처리
        for (int i = 0; i < 3; i++)
        {
            Destroy(UsingCard[i].gameObject);
            if (positionOccupied[i] == true)
            {
                positionOccupied[i] = false;
            }
        }
        WaitingCard.Clear();
        UsingCard.Clear();
        Debug.Log("Clear Complete");
        AddEmptyUsingCard(); // 빈 UsingCard 추가
    }

    public void Update()
    {

    }

    public int GetUsingCardIndex(Card clickedCard) //CurrCardSwitchindex엿나 여튼 그걸위해서 있음
    {
        for (int i = 0; i < UsingCard.Count; i++)
        {
            if (UsingCard[i] == clickedCard)
            {
                return i; // 클릭한 카드의 인덱스 반환 (0, 1, 2 중 하나)
            }
        }
        return -1; // 클릭한 카드가 UsingCard 리스트에 없으면 -1 반환
    }

    public void NotifyCardSelection(Card card)
    {
        int cardIndex = GetUsingCardIndex(card); // 클릭한 카드의 인덱스 가져오기

        if (cardIndex != -1) // 클릭한 카드가 UsingCard 리스트 안에 있다면
        {
            Debug.Log($"Clicked card is UsingCard[{cardIndex}]");

            CurrCardIndexForSwitch = cardIndex; // 선택한 카드의 위치 저장
            RechooseIndex = cardIndex;  // 같은 인덱스로 갱신

            OpenCardSelectionPanel(true);
        }
        else  // WaitingCard를 클릭한 경우
        {
            Debug.Log("Clicked card is NOT in UsingCard, switching...");

            ToBeSwitchCard(card);
            SwitchCard(Waitingcard_);

            card.gameObject.SetActive(false);
        }
    }



    //카드 바꾸기 위해서 WaitingCard포지션에서 마우스포인터와 가까운 카드를 찾아서 Card Waitingcard_를 넘겨줌 
    private Card ToBeSwitchCard(Card card)
    {
        Vector3 mousePosition = card.currentMousePosition; // 화면 좌표
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        Debug.Log("WC- WorldPosition :" + worldPosition);

        int index = WaitingCardPosition.FindIndex(pos => Vector3.Distance(pos, worldPosition) < 1.5f);


        //이것도 마우스핸들러로 바꿀수있을듯?
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

        return Waitingcard_;

    }

    private void SwitchCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError("SwitchCard() 호출 시 WaitingCard가 NULL임!");
            return;
        }

        Debug.Log($"SwitchCard() 실행 - UsingCard[{RechooseIndex}] 위치 변경");

        Card oldCard = null;

        // 1️. 기존 UsingCard 삭제
        if (positionOccupied[RechooseIndex] == true)
        {
            oldCard = UsingCard[RechooseIndex];
            UsignCard[RechooseIndex].Destory();
            Debug.Log($"이전 카드 {oldCard.name} 비활성화됨 (UsingCard[{RechooseIndex}])");
        }
        else
        {
            positionOccupied[RechooseIndex] = true;
        }

        // 2️⃣ WaitingCard에서 같은 카드 찾아서 활성화
        Card existingWaitingCard = WaitingCard.FirstOrDefault(wc => !wc.gameObject.activeSelf && wc.carditem.ID == card.carditem.ID);

        if (existingWaitingCard != null)
        {
            existingWaitingCard.gameObject.SetActive(true);
            Debug.Log($"WaitingCard에서 {existingWaitingCard.name} 활성화됨");
        }
        else
        {
            Debug.LogWarning($"비활성화된 WaitingCard 중 {card.name}을 찾을 수 없음!");
        }

        // 3️⃣ 새로운 UsingCard 생성
        Transform canvasTransform = GameObject.Find("Canvas/Background").transform;
        GameObject newCardObject = Instantiate(cardPrefab, cardPosition[RechooseIndex], Quaternion.identity, canvasTransform);

        var newCard = newCardObject.GetComponent<Card>();
        newCard.Setup(card.carditem, true);
        UsingCard[RechooseIndex] = newCard; // 리스트 업데이트

        Debug.Log($"UsingCard[{RechooseIndex}]에 새 카드 {newCard.name} 추가됨");

        // 4️⃣ 선택된 WaitingCard 비활성화
        int waitingIndex = WaitingCard.IndexOf(card);
        if (waitingIndex != -1)
        {
            WaitingCard[waitingIndex].gameObject.SetActive(false);
            Debug.Log($"WaitingCard[{waitingIndex}] 비활성화됨");
        }
        else
        {
            Debug.LogWarning("SwitchCard() - 선택한 카드가 WaitingCard 리스트에 없음");
        }

        // 5️⃣ 패널 닫기
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
            card.Setup(PopCard(false), false); // 필요에 따라 `isUse` 값을 조정
            UsingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
        else //얘가 뒤에 나오는 4개
        {
            if (WaitingCard.Count >= 4)
            {
                return;
            }

            Transform Canvas2Transform = GameObject.Find("CardSelectionPanel").transform;
            GameObject cardObject = Instantiate(cardPrefab, WaitingCardPosition[countwaitcard], Quaternion.identity, Canvas2Transform);
            Vector3 nowLocalScale = cardObject.transform.localScale;
            cardObject.transform.localScale = new Vector3(nowLocalScale.x * 0.014f, nowLocalScale.x * 0.014f, 1);

            BoxCollider collider = cardObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            if (!cardObject.TryGetComponent<CardMouseHandler>(out var cardHandler))
            {
                cardHandler = cardObject.AddComponent<CardMouseHandler>();
                cardHandler.Initialize(1.2f, 0.2f); // 배율과 지속 시간 설정
            }

            Debug.Log($"Card Created: {cardObject.name} with MouseHandler");

            var card = cardObject.GetComponent<Card>();
            card.Setup(PopCard(true), true); // 필요에 따라 `isUse` 값을 조정
            WaitingCard.Add(card); // 생성된 카드를 리스트에 추가
        }

    }
}

