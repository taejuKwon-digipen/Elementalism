using UnityEngine;
using UnityEngine.EventSystems; // 마우스 클릭 및 이벤트 처리를 위한 네임스페이스
using DG.Tweening; // 애니메이션 처리를 위한 DOTween 라이브러리
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using System.Collections;
using UnityEngine.UI; // UI 관련 기능을 위한 네임스페이스
using System.Collections.Generic;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler // 드래그 앤 드롭 기능을 위한 인터페이스 구현
{
    // UI 요소 연결 (카드 정보를 화면에 표시)
    [SerializeField] TMP_Text nameTMP; // 카드 이름 텍스트
    [SerializeField] TMP_Text PowerLeftTMP; // 일반 공격력 텍스트
    [SerializeField] TMP_Text PowerRightTMP; // 크리티컬 공격력 텍스트
    [SerializeField] TMP_Text CardDescriptionTMP; // 카드 설명 텍스트
    //[SerializeField] TMP_Text IDTMP; // 카드 ID 표시 (현재 주석 처리됨)
    [SerializeField] RawImage rawImage; // 카드 이미지를 표시하는 UI 컴포넌트

    // 각 원소에 해당하는 스프라이트들
    public Sprite fireSprite;    // 불 스프라이트
    public Sprite waterSprite;   // 물 스프라이트
    public Sprite airSprite;     // 공기 스프라이트
    public Sprite earthSprite;   // 흙 스프라이트

    public CardItem carditem; // 카드 데이터 (카드의 이름, 공격력, 설명 등을 포함)
    public bool isFront = true; // 카드가 앞면인지 여부
    public bool IsUsingCard = false; // 카드가 현재 사용 중인지 여부
    public bool isUsingImage = false; // 카드가 이미지 사용 중인지 여부

    private const float deleteThresholdY = -.0f; // 카드가 삭제될 기준 Y 위치
    public Vector3 currentMousePosition; // 현재 마우스 위치를 저장하는 변수

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalLocalPosition;
    private Transform originalParent;
    private Camera mainCamera;

    // 클래스 변수 추가
    private bool hasSetupPosition = false;
    private Vector3 dragStartPosition; // 드래그 시작 위치 저장

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>();
        mainCamera = Camera.main;
    }

    // 카드 초기화 및 데이터와 UI 연결
    public void Setup(CardItem carditem_, bool isFront_)
    {
        var cardSO = Resources.Load<CardItemSO>("ItemSO");
        if (cardSO != null) cardSO.UpdateFromSheet();

        // 원본 CardItem을 기반으로 변환된 CardItem 생성
        CardItem itemToUse = carditem_.Clone(); // 먼저 복제
        if (GameManager.Instance != null)
        {
            itemToUse.CreatedElementType = GameManager.Instance.GetModifiedElementType(carditem_.CreatedElementType);
            if (itemToUse.cardShape != null && itemToUse.cardShape.board != null)
            {
                // cardShape도 변환해야 하므로, cardShape의 복제본에 작업
                itemToUse.cardShape = carditem_.cardShape.Clone(); // cardShape도 복제
                for (int r = 0; r < itemToUse.cardShape.rows; r++)
                {
                    for (int c = 0; c < itemToUse.cardShape.columns; c++)
                    {
                        itemToUse.cardShape.board[r].colum[c] = 
                            GameManager.Instance.GetModifiedElementType(carditem_.cardShape.board[r].colum[c]);
                    }
                }
            }
        }
        
        this.carditem = itemToUse; // 클래스 멤버인 carditem을 변환된 버전으로 교체!
        isFront = isFront_;

        if (isFront)
        {
            nameTMP.text = this.carditem.CardName; // 이제 this.carditem은 변환된 데이터를 가짐 (단, CardName 등은 원본 유지)
            // PowerLeftTMP.text = this.carditem.PowerLeft.ToString();
            // PowerRightTMP.text = this.carditem.PowerRight.ToString();

            // 현재 레벨에 맞는 공격력 표시 (리스트 인덱스 주의: CurrentLevel은 1부터 시작)
            if (this.carditem.PowerLeftByLevel != null && this.carditem.PowerLeftByLevel.Count >= this.carditem.CurrentLevel)
            {
                PowerLeftTMP.text = this.carditem.PowerLeftByLevel[this.carditem.CurrentLevel - 1].ToString();
            }
            else
            {
                PowerLeftTMP.text = "N/A"; // 데이터 오류 또는 레벨 범위 초과
                Debug.LogWarning($"[Card] {this.carditem.CardName} (Level {this.carditem.CurrentLevel}): PowerLeftByLevel 데이터 오류 또는 범위 초과.");
            }

            if (this.carditem.PowerRightByLevel != null && this.carditem.PowerRightByLevel.Count >= this.carditem.CurrentLevel)
            {
                PowerRightTMP.text = this.carditem.PowerRightByLevel[this.carditem.CurrentLevel - 1].ToString();
            }
            else
            {
                PowerRightTMP.text = "N/A"; // 데이터 오류 또는 레벨 범위 초과
                Debug.LogWarning($"[Card] {this.carditem.CardName} (Level {this.carditem.CurrentLevel}): PowerRightByLevel 데이터 오류 또는 범위 초과.");
            }

            CardDescriptionTMP.text = this.carditem.CardDescription;
            isUsingImage = this.carditem.UseImage;

            if (isUsingImage)
            {
                rawImage = transform.Find("Border/ImageBorder/Image").GetComponent<RawImage>();
                if (rawImage != null && this.carditem.cardImage != null)
                {
                    rawImage.texture = this.carditem.cardImage;
                }
                else
                {
                    Debug.LogWarning($"[Card] {this.carditem.CardName}의 이미지 설정 실패");
                }
            }
            else if (this.carditem.cardShape != null) 
            {
                GenerateShapeFromData(this.carditem.cardShape); // 변환된 cardShape 사용
            }
            else
            {
                Debug.LogWarning($"[Card] {this.carditem.CardName}의 cardShape가 null입니다.");
            }
        }
        
        if (!hasSetupPosition)
        {
            originalLocalPosition = transform.localPosition;
            originalParent = transform.parent;
            hasSetupPosition = true;
            Debug.Log($"Card Setup: 초기 위치 = {originalLocalPosition}, 부모 = {originalParent?.name}");
        }
    }

    // ShapeData를 기반으로 원소 스프라이트로 카드 모양 생성
    private void GenerateShapeFromData(ShapeData shapeData)
    {
        if (shapeData == null)
        {
            Debug.LogWarning($"[Card] ShapeData가 null입니다. (GenerateShapeFromData)");
            return;
        }
        Transform imageBorderTransform = transform.Find("Border/ImageBorder");
        if (imageBorderTransform == null)
        {
            Debug.LogError($"[Card] {this.carditem.CardName}: Border/ImageBorder transform을 찾을 수 없습니다.");
            return;
        }
        foreach (Transform child in imageBorderTransform) Destroy(child.gameObject);

        var blockPrefab = Resources.Load<GameObject>("BlockPrefab");
        if (blockPrefab == null)
        {
            Debug.LogError("[Card] BlockPrefab을 Resources에서 로드할 수 없습니다.");
            return;
        }
        List<Vector2> blockPositions = new List<Vector2>();
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int column = 0; column < shapeData.columns; column++)
            {
                ElementType elementType = shapeData.board[row].colum[column]; // 이미 변환된 타입
                if (elementType != ElementType.None)
                {
                    var block = Instantiate(blockPrefab, imageBorderTransform);
                    var rectTransform = block.GetComponent<RectTransform>();
                    Vector2 position = new Vector2(column * 45, -row * 45);
                    rectTransform.anchoredPosition = position;
                    blockPositions.Add(position);
                    var image = block.GetComponent<Image>();
                    if (image == null) continue;
                    switch (elementType)
                    {
                        case ElementType.Fire: image.sprite = fireSprite; break;
                        case ElementType.Water: image.sprite = waterSprite; break;
                        case ElementType.Air: image.sprite = airSprite; break;
                        case ElementType.Earth: image.sprite = earthSprite; break; // 규칙에 따라 Fire로 변환되었어야 함
                    }
                }
            }
        }
        if (blockPositions.Count > 0)
        {
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var pos in blockPositions) { minX = Mathf.Min(minX, pos.x); maxX = Mathf.Max(maxX, pos.x); minY = Mathf.Min(minY, pos.y); maxY = Mathf.Max(maxY, pos.y); }
            Vector2 center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            foreach (Transform block in imageBorderTransform) { var rt = block.GetComponent<RectTransform>(); if (rt != null) rt.anchoredPosition -= center; }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 카드 처리 중이면 드래그 시작을 막음
        if (CardManager.Inst != null && CardManager.Inst.isProcessingCard)
        {
            Debug.Log("[Card] 카드가 처리 중이어서 새로운 카드를 사용할 수 없습니다.");
            return;
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 현재 위치와 부모 저장 (월드 위치가 아닌 로컬 위치 사용)
        originalLocalPosition = transform.localPosition;
        originalParent = transform.parent;
        
        // 드래그 시작 위치 저장
        dragStartPosition = transform.position;
        
        // 카드를 캔버스의 최상위로 이동
        transform.SetParent(canvas.transform);
        
        // 마우스 위치를 월드 위치로 변환
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // z 위치만 카드의 원래 z 위치로 설정
        mouseWorldPos.z = canvas.transform.position.z;
        
        // 카드의 위치를 마우스 위치로 설정 (월드 위치)
        transform.position = mouseWorldPos;
        
        Debug.Log($"드래그 시작: 원래 로컬 위치 = {originalLocalPosition}, 부모 = {originalParent?.name}, 마우스 월드 위치 = {mouseWorldPos}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 위치를 월드 위치로 변환
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // z 위치만 카드의 원래 z 위치로 설정
        mouseWorldPos.z = canvas.transform.position.z;
        
        // 카드의 위치를 마우스 위치로 설정 (월드 위치)
        transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 카드 처리 중이면 카드 사용을 막음
        if (CardManager.Inst != null && CardManager.Inst.isProcessingCard)
        {
            Debug.Log("[Card] 카드가 처리 중이어서 새로운 카드를 사용할 수 없습니다.");
            // 원래 위치로 복귀
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
                transform.localPosition = originalLocalPosition;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
            }
            return;
        }

        // 드래그 거리 계산
        float dragDistance = Vector3.Distance(dragStartPosition, transform.position);

        // Y 위치가 -170보다 위이고, 드래그 거리가 1.0 이상인지 확인
        if (transform.position.y > -1f && dragDistance > 1.0f)
        {
            // 활성화된 카드 수 체크
            int currentActiveCards = GridChecker.inst.GetActiveCards().Count;
            Debug.Log($"[Card] 현재 활성화된 카드 수: {currentActiveCards}");
            
            if (currentActiveCards >= 3)
            {
                Debug.Log("[Card] 한 턴에 최대 3장까지만 카드를 사용할 수 있습니다!");
                // 원래 위치로 복귀
                if (originalParent != null)
                {
                    transform.SetParent(originalParent);
                    transform.localPosition = originalLocalPosition;
                    transform.localScale = Vector3.one;
                    transform.localRotation = Quaternion.identity;
                }
                return;
            }

            // 카드를 활성화된 카드 목록에 추가
            GridChecker.inst.AddActiveCard(this);
            
            // 카드 사용 카운트 증가
            if (CardManager.Inst != null && CardManager.Inst.cardUsageUI != null)
            {
                CardManager.Inst.cardUsageUI.OnCardUsed();
                Debug.Log("[Card] 카드 사용 카운트 증가");
            }
            
            Debug.Log($"[Card] 카드 추가됨. 현재 활성화된 카드 수: {GridChecker.inst.GetActiveCards().Count}");
            
            // 상호작용 비활성화
            CardManager.Inst.SetInteractionsEnabled(false);
            
            // 튜토리얼 매니저가 존재하고, 특정 단계(CardLimit=2, Shop=5, ShopPurchase=6)가 아닐 때만 다음으로 진행
            var tutorialStep = TutorialManager.Instance?.CurrentStep ?? TutorialManager.TutorialStep.None;
            if (TutorialManager.Instance != null &&
                tutorialStep != TutorialManager.TutorialStep.CardLimit &&
                tutorialStep != TutorialManager.TutorialStep.BlockCheck &&
                tutorialStep != TutorialManager.TutorialStep.Shop)
            {
                TutorialManager.Instance.NextStep();
            }

            // 카드를 지정된 사용 위치로 이동
            Vector3 targetPosition = CardManager.Inst.cardUsePoint.position;
            transform.DOMove(targetPosition, 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    // 크기를 1.2배로 키우기
                    transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // 그리드 체크 실행
                            GridChecker.inst.CheckGrid();
                        });
                });
        }
        else
        {
            // 원래 위치로 복귀
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
                transform.localPosition = originalLocalPosition;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
                
                Debug.Log($"카드 복귀 후 - 로컬 위치: {transform.localPosition}, 부모: {transform.parent.name}");
            }
            else
            {
                Debug.LogError("원래 부모가 null입니다!");
            }
        }
    }
}
