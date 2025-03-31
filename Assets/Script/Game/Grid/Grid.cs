// 이 스크립트는 게임 내의 그리드(격자)를 생성하고 관리합니다.
// 그리드 안에 원소 블록들을 생성하고 배치하며,
// 게임 이벤트에 따라 원소들의 상태를 업데이트합니다.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public ShapeData currentShape;  // 현재 사용할 ShapeData
    public float squaresGap = 0.1f;               // 블록 사이의 간격
    public GameObject block;                  // 원소 블록 프리팹
    public Vector2 startPosition = new Vector2(0, 0); // 그리드 시작 위치
    public float squareScale = 0.5f;              // 블록의 크기 스케일
    public float everySquareOffset = 0.0f;        // 각 블록의 오프셋
    public bool turnEndButton = false;
    private Vector2 _offset = new Vector2(0, 0);  // 블록 간의 위치 오프셋 계산용 변수
    private List<GameObject> _blocks = new List<GameObject>(); // 생성된 원소 블록들의 리스트

    // OraImage 활성화를 위한 추가 필드
    private List<int> oraIndices = new List<int>();

    private bool interactionsEnabled = true;

    // 게임 이벤트에 메서드를 등록합니다.
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }
    // 게임 이벤트에서 메서드를 해제합니다.
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    // 특정 행과 열에 해당하는 블록을 반환하는 메서드
    public GameObject GetBlockAt(int row, int column)
    {
        int index = row * currentShape.columns + column; // 1차원 리스트 인덱스 계산
        if (index >= 0 && index < _blocks.Count)
        {
            return _blocks[index]; // 인덱스에 해당하는 블록 반환
        }
        return null; // 유효하지 않은 위치인 경우 null 반환
    }

    public void OnTurnEndButton()
    {
        turnEndButton = true;
        // 퍼즐 재생성 기능 구현
        var shapeLeft = 0;
        Shape remainingShape = null;

        foreach (var shape in shapeStorage.shapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shapeLeft++;
                remainingShape = shape;
            }
        }

        if (shapeLeft == 0 && turnEndButton)
        {
            GameEvents.RequestNewShapes();
        }
        else if (shapeLeft > 0)
        {
            // 남은 Shape가 있으면 다른 모든 Shape를 제거하고 새로운 Shape 생성
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape != remainingShape)
                {
                    shape.DisableAllSquares();
                }
            }
            GameEvents.RequestNewShapes();
        }
        else
        {
            GameEvents.SetShapeInactive();
        }
    }

    void Start()
    {
        if (currentShape == null)
        {
            Debug.LogError("Grid: currentShape이 할당되지 않았습니다!");
            return;
        }
        CreateGrid(); // 그리드를 생성합니다.
    }

    // 그리드를 생성하는 메서드
    private void CreateGrid()
    {
        ClearGrid();  // 기존 그리드 제거
        SpawnBlocks();        // 원소 블록들을 생성합니다.
        SetBlocksPositions(); // 생성된 블록들의 위치를 설정합니다.
        InitializeOra(); // Ora 초기화
    }

    // 기존 그리드 제거
    private void ClearGrid()
    {
        foreach (var block in _blocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        _blocks.Clear();
    }

    // 랜덤 Ora 초기화
    private void InitializeOra()
    {
        oraIndices = GetRandomIndices(2); // 랜덤한 두 위치를 선택
        foreach (var index in oraIndices)
        {
            var block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                block.ActivateOraImage(); // Ora 활성화
            }
        }
    }

    // 랜덤한 블록 인덱스 리스트 반환
    private List<int> GetRandomIndices(int count)
    {
        return Enumerable.Range(0, _blocks.Count).OrderBy(x => Random.value).Take(count).ToList();
    }

    // 원소 블록들을 생성하는 메서드
    private void SpawnBlocks()
    {

        for (var row = 0; row < currentShape.rows; row++)
        {
            for (var column = 0; column < currentShape.columns; column++)
            {
                ElementType shapeElementType = currentShape.board[row].colum[column];
                if (shapeElementType != ElementType.None)
                {
                    GameObject newBlock = Instantiate(block) as GameObject;
                    newBlock.GetComponent<Block>().BlockIndex = _blocks.Count;
                    newBlock.transform.SetParent(this.transform);
                    newBlock.transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                    ElementType elementType = shapeElementType;
                    if (elementType == ElementType.Random)
                    {
                        elementType = (ElementType)Random.Range(1, (int)ElementType.Void);
                    }

                    newBlock.GetComponent<Block>().SetElementType(elementType);
                    _blocks.Add(newBlock);
                }
                else
                {
                }
            }
        }
    }

    public ElementType GetElementTypeAt(int row, int column)
    {
        // 그리드의 해당 위치에 있는 블록을 찾고, 그 블록의 ElementType을 반환합니다.
        int index = row * currentShape.columns + column; // 1차원 리스트 인덱스 계산
        if (index >= 0 && index < _blocks.Count)
        {
            Block block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                return block.elementType;
            }
        }
        return ElementType.None; // 유효하지 않은 위치인 경우 None 반환
    }

    public void SetElementTypeAt(int row, int column, ElementType newType)
    {
        int index = row * currentShape.columns + column; // 1차원 리스트 인덱스 계산
        if (index >= 0 && index < _blocks.Count)
        {
            Block block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                block.SetElementType(newType);
            }
        }
    }

    // 생성된 원소 블록들의 위치를 설정하는 메서드
    private void SetBlocksPositions()
    {
        int column_number = 0;    // 현재 열 번호
        int row_number = 0;       // 현재 행 번호
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f); // 블록 간의 추가 간격 계산용 변수
        bool row_moved = false;   // 행이 이동했는지 여부
        int blockIndex = 0;       // 현재 처리 중인 블록의 인덱스

        var square_rect = _blocks[0].GetComponent<RectTransform>(); // 블록의 RectTransform 가져오기

        // 블록 간의 오프셋 계산
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        // ShapeData를 순회하면서 None이 아닌 위치에만 블록 배치
        for (var row = 0; row < currentShape.rows; row++)
        {
            square_gap_number.x = 0; // 각 행이 시작될 때 x 간격 초기화
            for (var column = 0; column < currentShape.columns; column++)
            {
                if (currentShape.board[row].colum[column] != ElementType.None)
                {
                    if (blockIndex >= _blocks.Count) break;

                    GameObject square = _blocks[blockIndex];

                    // 블록의 X, Y 위치 오프셋을 계산합니다.
                    var pos_x_offset = _offset.x * column + (square_gap_number.x * squaresGap);
                    var pos_y_offset = _offset.y * row + (square_gap_number.y * squaresGap);

                    // 특정 열마다 추가 간격을 추가하여 블록들을 그룹화합니다.
                    if (column > 0 && column % 3 == 0)
                    {
                        square_gap_number.x++;
                        pos_x_offset += squaresGap;
                    }

                    // 특정 행마다 추가 간격을 추가하여 블록들을 그룹화합니다.
                    if (row > 0 && row % 3 == 0 && row_moved == false)
                    {
                        row_moved = true;
                        square_gap_number.y++;
                        pos_y_offset += squaresGap;
                    }

                    // 블록의 위치를 설정합니다.
                    square.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        startPosition.x + pos_x_offset,
                        startPosition.y - pos_y_offset);

                    square.GetComponent<RectTransform>().localPosition = new Vector3(
                        startPosition.x + pos_x_offset,
                        startPosition.y - pos_y_offset, 0.0f);

                    blockIndex++;    // 다음 블록으로 이동
                }
            }
            row_moved = false;
        }
    }

    public void UpdateGridState(int row, int column, ElementType elementType)
    {
        int index = row * currentShape.columns + column; // 인덱스 계산
        if (index >= 0 && index < _blocks.Count)
        {
            Block block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                block.elementType = elementType; // 새로운 ElementType으로 업데이트
                block.SetBlockImage(elementType); // 이미지 업데이트
            }
        }
    }

    // 게임 이벤트에 따라 블록을 활성화하는 메서드
    private void CheckIfShapeCanBePlaced()
    {
        if (!interactionsEnabled) return;

        var squareIndexes = new List<int>();

        foreach (var square in _blocks)
        {
            var block = square.GetComponent<Block>();

            if(block.Selected) 
            {
                squareIndexes.Add(block.BlockIndex);
                block.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if(currentSelectedShape.TotalSquareNumber == squareIndexes.Count) 
        {
            foreach (var squareIndex  in squareIndexes) 
            {
                _blocks[squareIndex].GetComponent<Block>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            GameEvents.SetShapeInactive();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    public void SetInteractionsEnabled(bool enabled)
    {
        interactionsEnabled = enabled;
        Debug.Log($"[Grid] 상호작용 {(enabled ? "활성화" : "비활성화")} 완료");
    }
}
