// 이 스크립트는 게임에서 사용되는 퍼즐 조각(Shape)의 동작을 관리합니다.
// 마우스 입력과 드래그 이벤트를 처리하여 퍼즐 조각을 생성, 이동, 배치할 수 있게 합니다.
// ShapeData를 기반으로 퍼즐 조각의 모양과 위치를 설정합니다.

using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
{
    public GameObject squareShapeImage;      // 퍼즐 조각의 개별 블록 이미지
    public Vector3 shapeSelectedScale;       // 선택된 상태의 스케일
    public Vector2 offset = new Vector2(0f, 700f);  // 드래그 시 위치 보정용 오프셋

    [HideInInspector]
    public ShapeData CurrentShapeData;       // 현재 Shape의 데이터

    private List<GameObject> _currentShape = new List<GameObject>();  // 현재 Shape를 구성하는 블록들의 리스트
    private Vector3 _shapeStartScale;        // 초기 스케일 값
    private RectTransform _transform;        // RectTransform 컴포넌트
    private bool _shapeDraggable = true;     // 드래그 가능 여부
    private bool _isDragging = false;        // 현재 드래그 중인지 여부
    private Canvas _canvas;                  // 부모 캔버스

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale; // 초기 스케일 저장
        _transform = this.GetComponent<RectTransform>();                  // RectTransform 가져오기
        _canvas = GetComponentInParent<Canvas>();                         // 부모 캔버스 가져오기
        _shapeDraggable = true;                                           // 드래그 가능 설정
    }

    void Start()
    {
        // 초기화가 필요한 내용이 있으면 여기에 작성
    }
    void Update()
    {
        // 현재 드래그 중일 때만 회전
        if (_isDragging)
        {
            // Q 키가 눌리면 왼쪽으로 회전
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateShape(-90); // 왼쪽으로 90도 회전
            }
            // E 키가 눌리면 오른쪽으로 회전
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateShape(90);  // 오른쪽으로 90도 회전
            }
        }
    }

    // 새로운 Shape 요청을 처리하는 메서드
    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData); // 새로운 Shape 생성
    }

    // Shape를 생성하는 메서드
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;  // 현재 ShapeData 설정
        var totalSquareNumber = GetNumberOfSquares(shapeData); // 필요한 블록 수 계산

        // 필요한 블록 수만큼 리스트에 추가
        while (_currentShape.Count <= totalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        // 블록 초기화
        foreach (var square in _currentShape)
        {
            square.gameObject.transform.localPosition = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);
        int currentIndexInList = 0;

        // ShapeData를 기반으로 블록들의 위치 설정
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                ElementType elementType = shapeData.board[row].colum[column];
                if (elementType != ElementType.None)
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                        GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    var shapeSquare = _currentShape[currentIndexInList].GetComponent<ShapeSquare>();
                    shapeSquare.SetElementType(elementType);

                    currentIndexInList++;
                    
                }
            }
        }

    }

    // Shape의 블록들의 Y 위치를 계산하는 메서드
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0) // 행의 수가 홀수인 경우
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = row - middleSquareIndex;
                shiftOnY = -moveDistance.y * multiplier;
            }
            else // 행의 수가 짝수인 경우
            {
                var middleSquareIndex = shapeData.rows / 2;
                var multiplier = row - (middleSquareIndex - 0.5f);
                shiftOnY = -moveDistance.y * multiplier;
            }
        }
        return shiftOnY;
    }

    // Shape의 블록들의 X 위치를 계산하는 메서드
    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0) // 열의 수가 홀수인 경우
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = column - middleSquareIndex;
                shiftOnX = moveDistance.x * multiplier;
            }
            else // 열의 수가 짝수인 경우
            {
                var middleSquareIndex = shapeData.columns / 2;
                var multiplier = column - (middleSquareIndex - 0.5f);
                shiftOnX = moveDistance.x * multiplier;
            }
        }
        return shiftOnX;
    }

    // ShapeData에서 활성화된 블록의 수를 계산하는 메서드
    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (var elementType in rowData.colum)
            {
                if (elementType != ElementType.None)
                    number++;
            }
        }
        return number;
    }

    // 마우스 클릭 이벤트 처리 (필요 시 구현)
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // 마우스 버튼을 놓을 때 이벤트 처리 (필요 시 구현)
    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (var square in _currentShape)
        {
            Block block = square.GetComponent<Block>();

            if (block != null && block.currentCollidedBlock != ElementType.None)
            {
                // 충돌된 원소 타입으로 이미지 변경
                //block.SetBlockImage(block.currentCollidedBlock);
                block.SetElementType(block.currentCollidedBlock);
            }
        }

        this.GetComponent<RectTransform>().localScale = _shapeStartScale; // 스케일을 원래대로 변경
        GameEvents.CheckIfShapeCanBePlaced(); // Shape를 배치할 수 있는지 체크하는 이벤트 호출
    }

    // 드래그 시작 시 호출되는 메서드
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale; // 선택된 상태로 스케일 변경
        _isDragging = true; // 드래그 중 상태 설정
    }

    // 드래그 중에 호출되는 메서드
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        // 화면 좌표를 캔버스의 로컬 좌표로 변환하여 위치 설정
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    // 드래그 종료 시 호출되는 메서드
    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale; // 스케일을 원래대로 변경
        _isDragging = false; // 드래그 중 상태 해제
        GameEvents.CheckIfShapeCanBePlaced(); // Shape를 배치할 수 있는지 체크하는 이벤트 호출
    }
    
    // 모양을 회전시키는 메서드
    private void RotateShape(float angle)
    {
        _transform.Rotate(0, 0, angle); // RectTransform을 기준으로 Z축 회전
    }

    // 마우스 버튼을 눌렀을 때 이벤트 처리 (필요 시 구현)
    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
