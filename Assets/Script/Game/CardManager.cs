using DG.Tweening; // 애니메이션 처리를 위한 DOTween 라이브러리
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting; // 유니티 비주얼 스크립팅 기능
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;// UI 관련 기능을 위한 네임스페이스


public class CardManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spownpoints = new List<GameObject>(); //Using Card spawn points
    [SerializeField] List<GameObject> Wspownpoints = new List<GameObject>(); // Waiting Card spawn points

    // 싱글톤 패턴: CardManager의 인스턴스
    public static CardManager Inst { get; private set; }
    //void Awake() => Inst = this;// 싱글톤 초기화

    public int currentCardIndex = 0;// 현재 카드의 인덱스

    [SerializeField] CardItemSO carditemso; // 카드 데이터베이스 (ScriptableObject 사용)
    public GameObject cardPrefab;  // Inspector에서 할당

    [Header("References")]
    [SerializeField] private Inventory inventory; // 인벤토리 ScriptableObject 참조
    [SerializeField] private Deck deck; // 덱 참조

    [Header("Card Lists")]
    [SerializeField] public List<Card> UsingCard; // 사용 중인 카드 리스트 (오른쪽 3개)
    [SerializeField] public List<Card> WaitingCard; // 대기 중인 카드 리스트 (왼쪽 4개)
    private List<CardItem> sessionCards = new List<CardItem>(); // 현재 세션에서 사용 가능한 카드들

    private List<CardItem> UnlockedCards = new List<CardItem>(); //해금 카드 리스트
    public List<CardItem> Items = new List<CardItem>();  // 모든 카드 리스트

    public Transform Canvas2Transform; // UI 캔버스의 트랜스폼

    // 카드 위치 상태 리스트 (false = 비어있음, true = 사용 중)
    private List<bool> positionOccupied;

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
    Card OldCard = null;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private List<CardItem> debugWaitingCards = new List<CardItem>();
    [SerializeField] private List<CardItem> debugUsingCards = new List<CardItem>();
    [SerializeField] private List<CardItem> debugInventoryCards = new List<CardItem>();
    [SerializeField] private List<CardItem> debugDeckCards = new List<CardItem>();
    [SerializeField] private List<CardItem> debugUnlockedCards = new List<CardItem>();

    private void LoadCardItemSO()
    {
        carditemso = Resources.Load<CardItemSO>("ItemSO");
        inventory = Resources.Load<Inventory>("Inventory");
        deck = Resources.Load<Deck>("Deck");

        if (carditemso == null)
        {
            Debug.LogError("[CardManager] ItemSO를 찾을 수 없습니다!");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("[CardManager] Inventory를 찾을 수 없습니다!");
            return;
        }

        if (deck == null)
        {
            Debug.LogError("[CardManager] Deck을 찾을 수 없습니다!");
            return;
        }

        Debug.Log("[CardManager] ItemSO, Inventory, Deck 로드 완료");
        
        // 카드 상태 로깅
        foreach (var card in carditemso.items)
        {
            Debug.Log($"[CardManager] 카드 상태 확인: {card.CardName} (ID: {card.ID}, IsUnlocked: {card.IsUnlocked})");
        }
    }

    // 새로운 카드 언락
    public void UnlockCard(int cardID)
    {
        var card = carditemso.items.FirstOrDefault(c => c.ID == cardID);
        if (card != null && !card.IsUnlocked)
        {
            card.IsUnlocked = true;
            Debug.Log($"[CardManager] 새로운 카드 언락: {card.CardName} (ID: {card.ID})");
        }
    }

    // 특정 카드의 언락 상태 확인
    public bool IsCardUnlocked(int cardID)
    {
        var card = carditemso.items.FirstOrDefault(c => c.ID == cardID);
        return card != null && card.IsUnlocked;
    }

    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            LoadCardItemSO(); // SO와 Inventory 로드
            
            // inventory가 로드되었는지 확인
            if (inventory != null)
            {
                LoadInitialCards(); // 초기 카드 로드
            }
            else
            {
                Debug.LogError("[CardManager] Inventory가 로드되지 않아 초기 카드를 로드할 수 없습니다.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int CurrCardIndexForSwitch = 0;

    [SerializeField] private GameObject UsingCardPanel;  // 사용 중인 카드 패널


    //수정중 - 버튼 클릭 시 패널 닫히게
    public void XButtonClicked()
    {
        Debug.Log("X버튼 클릭");
        OpenCardSelectionPanel(false);
    }

    int countwaitcard = 0; // 현재 대기 카드의 개수

    //카드 버퍼에서 카드 뽑아오기
    public CardItem PopCard(bool isSelection = false)
    {
        // deck이 null인지 확인
        if (deck == null)
        {
            Debug.Log("Deck이 할당되지 않았습니다.");
            return null;
        }
        
        // 덱에서 카드 뽑기
        CardItem drawnCard = deck.DrawCard();
        
        // 덱이 비어있으면 버린 카드 더미에서 덱 리필
        if (drawnCard == null)
        {
            //Debug.LogWarning("카드를 뽑을 수 없습니다: 카드가 없음");
            return null;
        }
        
        return drawnCard;
    }

    private void SaveScriptableObject()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(carditemso);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    private void CheckSOData()
    {
        foreach (var card in carditemso.items)
        {
            Debug.Log($"📌 카드 ID: {card.ID}, IsUnlocked: {card.IsUnlocked}");
        }
    }

    private void SetUnlockedCard()
    {
        // 세션 카드만 사용
        UnlockedCards = new List<CardItem>(sessionCards);
        ShuffleList(UnlockedCards);
    }

    void SetCardBuffer()
    {

        //ItemBuffer = new List<CardItem>();

        if (UnlockedCards == null)
        {
            Debug.Log("Unlocked Card List is null, Line 119");
        }

    }
    // 게임 시작 시 초기화
    void Start()
    {
        Debug.Log($"SO 확인: {carditemso.name}");
        
        // 현재 세션에서 사용할 카드 설정
        SetSessionCards();
        
        // sessionCards가 비어있는지 확인
        if (sessionCards.Count == 0)
        {
            Debug.LogWarning("세션 카드가 비어 있습니다. 기본 카드를 추가합니다.");
            LoadInitialCards(); // 기본 카드 로드
            SetSessionCards(); // 세션 카드 다시 설정
        }
        
        // deck이 null인지 확인하고 처리
        if (deck == null)
        {
            Debug.LogWarning("Deck이 할당되지 않았습니다. 새로 생성합니다.");
            GameObject deckObject = new GameObject("Deck");
            deck = deckObject.AddComponent<Deck>();
            deckObject.transform.SetParent(transform);
        }
        
        // 덱 초기화
        deck.InitializeDeck(sessionCards);
        
        RegisterCardMouseHandlers();
        
        // cardSelectionPanel이 null인지 확인
        if (cardSelectionPanel == null)
        {
            Debug.Log("cardSelectionPanel이 할당되지 않았습니다.");
            // 필요한 경우 여기서 생성할 수 있습니다.
        }
        else
        {
            cardSelectionPanel.SetActive(false);
        }
        
        // PanelBackground가 null인지 확인
        if (PanelBackground == null)
        {
            Debug.Log("PanelBackground가 할당되지 않았습니다.");
        }
        else
        {
            PanelBackground.SetActive(false);
        }

        positionOccupied = new List<bool> { false, false, false };
        
        // UsingCard와 WaitingCard 초기화
        UsingCard = new List<Card>();
        WaitingCard = new List<Card>();
        
        // spownpoints와 Wspownpoints가 null인지 확인
        if (spownpoints == null || spownpoints.Count < 3)
        {
            Debug.Log($"spownpoints가 null이거나 요소가 부족합니다. 현재 크기: {(spownpoints != null ? spownpoints.Count : 0)}");
        }
        
        if (Wspownpoints == null || Wspownpoints.Count < 6)
        {
            Debug.Log($"Wspownpoints가 null이거나 요소가 부족합니다. 현재 크기: {(Wspownpoints != null ? Wspownpoints.Count : 0)}");
        }
        
        AddEmptyUsingCard(); // 빈 UsingCard 추가
        
        // 초기 WaitingCard 6장 뽑기
        DrawInitialWaitingCards();
    }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
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
        // spownpoints가 null이거나 충분한 요소가 없는지 확인
        if (spownpoints == null || spownpoints.Count < 3)
        {
            Debug.Log($"spownpoints가 null이거나 요소가 부족합니다. 현재 크기: {(spownpoints != null ? spownpoints.Count : 0)}");
            return;
        }
        
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
        // 덱에 카드가 있는지 확인
        if (deck != null && deck.GetDeckCount() == 0)
        {
            Debug.LogWarning("덱에 카드가 없습니다. 세션 카드를 다시 덱에 추가합니다.");
            deck.InitializeDeck(sessionCards);
        }

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

        // 사용한 카드를 버린 카드 더미로 이동
        for (int i = 0; i < 3; i++)
        {
            if (UsingCard[i] != null && UsingCard[i].gameObject.activeSelf && UsingCard[i].carditem != null)
            {
                // deck이 null인지 확인
                if (deck != null)
                {
                    // 사용한 카드를 버린 카드 더미로 이동
                    deck.AddToDiscard(UsingCard[i].carditem);
                }
            }
            
            if (UsingCard[i] != null)
            {
                Destroy(UsingCard[i].gameObject);
            }
            
            if (positionOccupied[i] == true)
            {
                positionOccupied[i] = false;
            }
        }

        // WaitingCard 처리
        for (int i = 0; i < 4; i++)
        {
            if (WaitingCard[i] != null)
            {
                if (WaitingCard[i].gameObject.activeSelf == false)
                {
                    WaitingCard[i].gameObject.SetActive(true);
                }
                Destroy(WaitingCard[i].gameObject);
            }
        }

        WaitingCard.Clear();
        UsingCard.Clear();
        countwaitcard = 0;
        
        Debug.Log("Clear Complete");
        AddEmptyUsingCard(); // 빈 UsingCard 추가
        
        // 새로운 WaitingCard 4장 뽑기
        DrawInitialWaitingCards();
    }

    public void Update()
    {
        UpdateDebugInfo();
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
        //CurrCardIndexForSwitch = WaitingCardIndex
        //currenttrueindex = old usingcard index

        int cardIndex = GetUsingCardIndex(card); // 클릭한 카드의 인덱스 가져오기
        if (cardIndex != -1 && positionOccupied[cardIndex] == true) //UsingCard 선택되고 카드가 이미 선택 -> Switch 준비
        {
            OldCard = UsingCard[cardIndex];
            currenttrueindex = cardIndex;
            OpenCardSelectionPanel(true);
        }
        else if (cardIndex != -1 && positionOccupied[cardIndex] == false) //UsingCard선택되고 카드 아무것도없을때 -> AddCard준비
        {
            OpenCardSelectionPanel(true);
            currenttrueindex = cardIndex;
        }
        else if (cardIndex == -1)  //WaitingCard 선택됨
        {
            CurrCardIndexForSwitch = FindWaitingCard(card);
            if (OldCard == null)
            {
                AddUsingCard(card);
            }
            else if (OldCard != null)
            {
                SwitchCard(card);
                OldCard = null;
            }
            card.gameObject.SetActive(false);
            OpenCardSelectionPanel(false);
        }
    }

    public void AddUsingCard(Card waitingcard)
    {
        //CurrCardIndexForSwitch = WaitingCardIndex
        //currenttrueindex = old usingcard index

        Destroy(UsingCard[currenttrueindex].gameObject);

        Transform Canvas2Transform = GameObject.Find("Canvas/Background/Cards").transform;
        GameObject newCardObject = Instantiate(cardPrefab, spownpoints[currenttrueindex].transform.position/*cardPosition[currenttrueindex]*/, Quaternion.identity, Canvas2Transform);

        var newCard = newCardObject.GetComponent<Card>();
        newCard.Setup(waitingcard.carditem, true);

        UsingCard[currenttrueindex] = newCard;
        positionOccupied[currenttrueindex] = true;

        Debug.Log($"UsingCard[{RechooseIndex}]에 새 카드 {newCard.name} 추가됨");
    }

    public int FindWaitingCard(Card card)
    {
        for (int i = 0; i < WaitingCard.Count; i++)
        {
            if (WaitingCard[i] == card && WaitingCard[i].gameObject.activeSelf == true)
            {
                return i;
            }
        }
        return -1;
    }

    private void SwitchCard(Card waitingcard) // 이 카드는 WaitingCard
    {
        //바꾼 카드 다시 나오게 하면 됨
        if (waitingcard == null)
        {
            Debug.Log("SwitchCard() 호출 시 WaitingCard가 NULL임!");
            return;
        }
        //CurrCardIndexForSwitch = WaitingCardIndex
        //currenttrueindex = old usingcard index


        //1. 기존카드 존재 -> OldCard 에 카드 저장 후 삭제
        Card oldcard = UsingCard[currenttrueindex];
        Debug.Log($"이전 카드: {oldcard.name} (UsingCard[{RechooseIndex}])");

        Destroy(UsingCard[currenttrueindex].gameObject);


        // 2️. 새로운 카드 생성 후 삽입
        Transform Canvas2Transform = GameObject.Find("Canvas/Background/Cards").transform;
        GameObject newCardObject = Instantiate(cardPrefab, spownpoints[currenttrueindex].transform.position, Quaternion.identity, Canvas2Transform); //여기서 에러뜸

        var newCard = newCardObject.GetComponent<Card>();
        newCard.Setup(waitingcard.carditem, true);

        UsingCard[currenttrueindex] = newCard;
        positionOccupied[currenttrueindex] = true;

        Debug.Log($"UsingCard[{RechooseIndex}]에 새 카드 {newCard.name} 추가됨");

        if (newCard.TryGetComponent<CardMouseHandler>(out var newCardHandler))
        {
            newCardHandler.ResetScale();  // 새 카드 크기 초기화
        }

        // 3️. 기존 oldCard를 WaitingCard에서 활성화 
        if (oldcard != null)
        {
            for (int i = 0; i < WaitingCard.Count; i++)
            {
                if (oldcard.carditem.CardName == WaitingCard[i].carditem.CardName && !WaitingCard[i].gameObject.activeSelf)
                {
                    WaitingCard[i].gameObject.SetActive(true);
                    break;
                }
            }
            oldcard = null;
        }

    }



    //카드 오브젝트 추가
    void AddCard(bool isUse)
    {
        if (isUse == true) //Using card
        {
            // Canvas/Background/Cards를 찾을 수 없는 경우 처리
            GameObject cardsParent = GameObject.Find("Canvas/Background/Cards");
            if (cardsParent == null)
            {
                Debug.Log("Canvas/Background/Cards를 찾을 수 없습니다.");
                return;
            }
            
            Transform Canvas2Transform = cardsParent.transform;
            GameObject cardObject = Instantiate(cardPrefab, spownpoints[currenttrueindex].transform.position, Quaternion.identity, Canvas2Transform);
            var card = cardObject.GetComponent<Card>();
            
            // PopCard에서 카드 가져오기
            CardItem cardItem = PopCard(false);
            
            // cardItem이 null인지 확인
            if (cardItem == null)
            {
                Debug.Log("카드를 생성할 수 없습니다: PopCard가 null을 반환했습니다.");
                Destroy(cardObject); // 카드 오브젝트 삭제
                return;
            }
            
            card.Setup(cardItem, false);
            UsingCard.Add(card); // 생성된 카드를 리스트에 추가
        }
        else //얘가 뒤에 나오는 4개
        {
            if (WaitingCard.Count >= 4)
            {
                return;
            }

            // CardSelectionPanel을 찾을 수 없는 경우 처리
            GameObject cardSelectionPanelObj = GameObject.Find("CardSelectionPanel");
            if (cardSelectionPanelObj == null)
            {
                Debug.Log("CardSelectionPanel을 찾을 수 없습니다.");
                return;
            }
            
            Transform Canvas2Transform = cardSelectionPanelObj.transform;
            
            // Wspownpoints가 null이거나 충분한 요소가 없는지 확인
            if (Wspownpoints == null || Wspownpoints.Count <= countwaitcard)
            {
                Debug.Log($"Wspownpoints가 null이거나 인덱스({countwaitcard})가 범위를 벗어납니다. 현재 크기: {(Wspownpoints != null ? Wspownpoints.Count : 0)}");
                return;
            }
            
            GameObject cardObject = Instantiate(cardPrefab, Wspownpoints[countwaitcard].transform.position, Quaternion.identity, Canvas2Transform);
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
            
            // PopCard에서 카드 가져오기
            CardItem cardItem = PopCard(true);
            
            // cardItem이 null인지 확인
            if (cardItem == null)
            {
                Debug.Log("카드를 생성할 수 없습니다: PopCard가 null을 반환했습니다.");
                Destroy(cardObject); // 카드 오브젝트 삭제
                return;
            }
            
            card.Setup(cardItem, true);
            WaitingCard.Add(card); // 생성된 카드를 리스트에 추가
        }

    }

    // 카드를 덱에 추가하는 메서드
    public void AddCardToBuffer(CardItem cardItem)
    {
        // 비어있는 위치 찾기
        for (int i = 0; i < 3; i++)
        {
            if (!positionOccupied[i])
            {
                // 새 카드 생성 및 설정
                GameObject newCard = Instantiate(cardPrefab, UsingCardPanel.transform);
                Card cardComponent = newCard.GetComponent<Card>();
                cardComponent.Setup(cardItem, true);

                // 카드 위치 설정
                RectTransform rectTransform = newCard.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = spownpoints[i].transform.position;

                // 카드 리스트에 추가
                UsingCard[i] = cardComponent;
                positionOccupied[i] = true;

                break;
            }
        }
    }

    private void LoadInitialCards()
    {
        Debug.Log("[CardManager] LoadInitialCards 시작");
        
        // 인벤토리가 비어있을 때만 초기화
        if (inventory.unlockedCards.Count == 0)
        {
            // 기본 카드 (ID: 1,2,3,4)만 인벤토리에 추가
            for (int i = 1; i <= 4; i++)
            {
                var card = carditemso.items.FirstOrDefault(x => x.ID == i);
                if (card != null)
                {
                    inventory.AddCard(card);
                    Debug.Log($"[CardManager] 기본 카드 추가됨: {card.CardName} (ID: {card.ID})");
                }
            }
            
            SaveScriptableObject();
            Debug.Log($"[CardManager] 초기 카드 로드 완료. 인벤토리 카드 수: {inventory.unlockedCards.Count}");
        }
        else
        {
            Debug.Log($"[CardManager] 이미 인벤토리에 {inventory.unlockedCards.Count}개의 카드가 있습니다.");
        }
    }

    // 현재 세션에서 사용할 카드 설정
    private void SetSessionCards()
    {
        sessionCards.Clear();
        
        // 인벤토리에 있는 카드만 세션 카드에 추가
        foreach (var card in inventory.unlockedCards)
        {
            sessionCards.Add(card);
            Debug.Log($"세션에 카드 추가: {card.CardName} (ID: {card.ID})");
        }
        
        // 덱 초기화
        if (deck != null)
        {
            deck.InitializeDeck(sessionCards);
            Debug.Log($"[CardManager] 덱이 초기화됨. 카드 수: {sessionCards.Count}");
        }
    }

    private void RefreshCardBuffer()
    {
        Items.Clear();
        Items.AddRange(sessionCards); // 인벤토리 카드가 아닌 세션 카드 사용
        Debug.Log($"카드 버퍼 새로고침. 총 카드 수: {Items.Count}");
    }

    public void AddCardToInventory(CardItem cardItem)
    {
        Debug.Log($"[CardManager] AddCardToInventory 호출됨: {cardItem.CardName} (ID: {cardItem.ID})");
        
        // 인벤토리에 카드가 없을 때만 추가
        if (!inventory.unlockedCards.Exists(x => x.ID == cardItem.ID))
        {
            // 인벤토리에 카드 추가
            inventory.AddCard(cardItem);
            Debug.Log($"[CardManager] 인벤토리에 카드 추가됨. 현재 인벤토리 카드 수: {inventory.unlockedCards.Count}");
            
            // 현재 세션에도 카드 추가
            sessionCards.Add(cardItem);
            Debug.Log($"[CardManager] 세션에 카드 추가됨. 현재 세션 카드 수: {sessionCards.Count}");
            
            // SO에서 카드 상태 업데이트
            var cardInSO = carditemso.items.FirstOrDefault(x => x.ID == cardItem.ID);
            if (cardInSO != null)
            {
                cardInSO.IsUnlocked = true;
                Debug.Log($"[CardManager] SO에서 카드 상태 업데이트됨: {cardItem.CardName}");
                SaveScriptableObject();
            }
            else
            {
                Debug.LogWarning($"[CardManager] SO에서 카드를 찾을 수 없음: {cardItem.CardName} (ID: {cardItem.ID})");
            }
            
            // 덱 초기화
            if (deck != null)
            {
                deck.InitializeDeck(sessionCards);
                Debug.Log($"[CardManager] 덱이 새로 초기화됨");
            }
            
            Debug.Log($"[CardManager] 카드 추가 완료: {cardItem.CardName} (ID: {cardItem.ID})");
        }
        else
        {
            Debug.Log($"[CardManager] 카드가 이미 인벤토리에 존재함: {cardItem.CardName} (ID: {cardItem.ID})");
        }
    }

    private void DrawInitialWaitingCards()
    {
        if (deck == null)
        {
            Debug.Log("Deck이 할당되지 않았습니다.");
            return;
        }
        
        // 덱에 카드가 있는지 확인
        if (deck.GetDeckCount() == 0)
        {
            Debug.LogWarning("덱에 카드가 없습니다. 세션 카드를 다시 덱에 추가합니다.");
            
            // sessionCards가 비어있는지 확인
            if (sessionCards.Count == 0)
            {
                Debug.LogWarning("세션 카드가 비어 있습니다. 기본 카드를 추가합니다.");
                LoadInitialCards(); // 기본 카드 로드
                SetSessionCards(); // 세션 카드 다시 설정
            }
            
            deck.InitializeDeck(sessionCards);
            
            // 그래도 덱이 비어있으면 오류 출력 후 종료
            if (deck.GetDeckCount() == 0)
            {
                Debug.Log("덱을 초기화했지만 여전히 카드가 없습니다.");
                return;
            }
        }
        
        // 대기 카드 6장 뽑기
        for (int i = 0; i < 6; i++)
        {
            CardItem cardItem = deck.DrawCard();
            if (cardItem != null)
            {
                AddWaitingCard(cardItem);
            }
            else
            {
                Debug.Log("카드를 뽑을 수 없습니다: 덱이 비었습니다.");
                break;
            }
        }
    }

    private void AddWaitingCard(CardItem cardItem)
    {
        if (WaitingCard.Count >= 6)
        {
            return;
        }

        // Wspownpoints가 null이거나 충분한 요소가 없는지 확인
        if (Wspownpoints == null || Wspownpoints.Count <= countwaitcard)
        {
            Debug.Log($"Wspownpoints가 null이거나 인덱스({countwaitcard})가 범위를 벗어납니다. 현재 크기: {(Wspownpoints != null ? Wspownpoints.Count : 0)}");
            return;
        }

        // CardSelectionPanel을 찾을 수 없는 경우 처리
        GameObject cardSelectionPanelObj = GameObject.Find("CardSelectionPanel");
        if (cardSelectionPanelObj == null)
        {
            Debug.Log("CardSelectionPanel을 찾을 수 없습니다.");
            return;
        }
        
        Transform Canvas2Transform = cardSelectionPanelObj.transform;
        
        // cardPrefab이 null인지 확인
        if (cardPrefab == null)
        {
            Debug.Log("cardPrefab이 할당되지 않았습니다.");
            return;
        }
        
        GameObject cardObject = Instantiate(cardPrefab, Wspownpoints[countwaitcard].transform.position, Quaternion.identity, Canvas2Transform);
        Vector3 nowLocalScale = cardObject.transform.localScale;
        cardObject.transform.localScale = new Vector3(nowLocalScale.x * 0.014f, nowLocalScale.x * 0.014f, 1);

        BoxCollider collider = cardObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (!cardObject.TryGetComponent<CardMouseHandler>(out var cardHandler))
        {
            cardHandler = cardObject.AddComponent<CardMouseHandler>();
            cardHandler.Initialize(1.2f, 0.2f); // 배율과 지속 시간 설정
        }

        var card = cardObject.GetComponent<Card>();
        card.Setup(cardItem, true);
        WaitingCard.Add(card);
        countwaitcard++;
    }

    private void UpdateDebugInfo()
    {
        if (!showDebugInfo) return;

        // Waiting Cards 정보 업데이트
        debugWaitingCards.Clear();
        foreach (var card in WaitingCard)
        {
            if (card != null && card.carditem != null)
            {
                debugWaitingCards.Add(card.carditem);
            }
        }

        // Using Cards 정보 업데이트
        debugUsingCards.Clear();
        foreach (var card in UsingCard)
        {
            if (card != null && card.carditem != null)
            {
                debugUsingCards.Add(card.carditem);
            }
        }

        // Inventory Cards 정보 업데이트
        debugInventoryCards.Clear();
        if (inventory != null)
        {
            foreach (var card in inventory.unlockedCards)
            {
                if (card != null)
                {
                    debugInventoryCards.Add(card);
                }
            }
        }

        // Deck Cards 정보 업데이트
        debugDeckCards.Clear();
        if (deck != null)
        {
            var deckCards = deck.GetDeckCards();
            foreach (var card in deckCards)
            {
                if (card != null)
                {
                    debugDeckCards.Add(card);
                }
            }
        }

        // Unlocked Cards 정보 업데이트
        debugUnlockedCards.Clear();
        foreach (var card in carditemso.items)
        {
            if (card != null && card.IsUnlocked)
            {
                debugUnlockedCards.Add(card);
            }
        }
    }
}

