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
        carditem = carditem_;
        isFront = isFront_;

        if (isFront)
        {
            nameTMP.text = carditem.CardName;
            PowerLeftTMP.text = carditem.PowerLeft.ToString();
            PowerRightTMP.text = carditem.PowerRight.ToString();
            CardDescriptionTMP.text = carditem.CardDescription;

            isUsingImage = carditem.UseImage;

            if (isUsingImage)
            {
                rawImage = transform.Find("Border/ImageBorder/Image").GetComponent<RawImage>();
                if (rawImage != null && carditem.cardImage != null)
                {
                    rawImage.texture = carditem.cardImage;
                }
                else
                {
                    Debug.LogWarning($"[Card] {carditem.CardName}의 이미지 설정 실패");
                }
            }
            else if (carditem.cardShape != null)
            {
                GenerateShapeFromData(carditem.cardShape);
            }
            else
            {
                Debug.LogWarning($"[Card] {carditem.CardName}의 cardShape가 null입니다.");
            }
        }
        
        // 위치 초기화를 한 번만 하도록 수정
        if (!hasSetupPosition)
        {
            // 처음 위치 정보 저장 (localPosition 사용)
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
            Debug.LogWarning($"[Card] {carditem.CardName}의 ShapeData가 null입니다.");
            return;
        }

        // 카드의 모양 영역 초기화 (이미 생성된 블록이 있다면 삭제)
        foreach (Transform child in transform.Find("Border/ImageBorder"))
        {
            Destroy(child.gameObject);
        }

        // ShapeData를 순회하며 원소에 따라 블록 생성
        var parentTransform = transform.Find("Border/ImageBorder"); // 부모 오브젝트
        var blockPrefab = Resources.Load<GameObject>("BlockPrefab"); // 블록 프리팹 로드

        List<Vector2> blockPositions = new List<Vector2>(); // 블록 위치를 저장할 리스트

        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int column = 0; column < shapeData.columns; column++)
            {
                ElementType elementType = shapeData.board[row].colum[column];
                if (elementType != ElementType.None)
                {
                    // 블록 인스턴스 생성
                    var block = Instantiate(blockPrefab, parentTransform);
                    var rectTransform = block.GetComponent<RectTransform>();
                    Vector2 position = new Vector2(column * 45, -row * 45); // 블록 위치 계산
                    rectTransform.anchoredPosition = position; // 블록 위치 설정
                    blockPositions.Add(position); // 블록 위치 리스트에 추가

                    var image = block.GetComponent<Image>();

                    // 원소 타입에 따라 스프라이트 설정
                    switch (elementType)
                    {
                        case ElementType.Fire:
                            image.sprite = fireSprite;
                            break;
                        case ElementType.Water:
                            image.sprite = waterSprite;
                            break;
                        case ElementType.Air:
                            image.sprite = airSprite;
                            break;
                        case ElementType.Earth:
                            image.sprite = earthSprite;
                            break;
                    }
                }
            }
        }

        // 배열 중심 계산
        if (blockPositions.Count > 0)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var pos in blockPositions)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }

            Vector2 center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);

            // 블록들의 중심이 (0,0)이 되도록 이동
            foreach (Transform block in parentTransform)
            {
                var rectTransform = block.GetComponent<RectTransform>();
                rectTransform.anchoredPosition -= center; // 중심점 만큼 이동
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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

        // 드래그 거리 계산
        float dragDistance = Vector3.Distance(dragStartPosition, transform.position);

        // Y 위치가 -170보다 위이고, 드래그 거리가 1.0 이상인지 확인
        if (transform.position.y > -1f && dragDistance > 1.0f)
        {
            // 카드를 활성화된 카드 목록에 추가
            GridChecker.inst.AddActiveCard(this);
            
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
                // 원래 부모로 변경
                transform.SetParent(originalParent);
                
                // 로컬 위치 설정 (무조건 원래 위치로)
                transform.localPosition = originalLocalPosition;
                
                // 스케일과 회전 초기화
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
