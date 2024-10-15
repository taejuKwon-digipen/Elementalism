// 이 스크립트는 게임 내의 그리드(격자)를 생성하고 관리합니다.
// 그리드 안에 원소 블록들을 생성하고 배치하며,
// 게임 이벤트에 따라 원소들의 상태를 업데이트합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // 그리드의 열과 행 수를 정의합니다.
    public int columns = 0;                       // 열의 수
    public int rows = 0;                          // 행의 수
    public float squaresGap = 0.1f;               // 블록 사이의 간격
    public GameObject block;                  // 원소 블록 프리팹
    public Vector2 startPosition = new Vector2(0, 0); // 그리드 시작 위치
    public float squareScale = 0.5f;              // 블록의 크기 스케일
    public float everySquareOffset = 0.0f;        // 각 블록의 오프셋

    private Vector2 _offset = new Vector2(0, 0);  // 블록 간의 위치 오프셋 계산용 변수
    private List<GameObject> _blocks = new List<GameObject>(); // 생성된 원소 블록들의 리스트

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

    void Start()
    {
        CreateGrid(); // 그리드를 생성합니다.
    }

    // 그리드를 생성하는 메서드
    private void CreateGrid()
    {
        SpawnBlocks();        // 원소 블록들을 생성합니다.
        SetBlocksPositions(); // 생성된 블록들의 위치를 설정합니다.
    }

    // 원소 블록들을 생성하는 메서드
    private void SpawnBlocks()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                // 원소 블록 프리팹을 인스턴스화하여 생성합니다.
                GameObject newBlock = Instantiate(block) as GameObject;
                newBlock.transform.SetParent(this.transform); // 그리드 오브젝트를 부모로 설정
                newBlock.transform.localScale = new Vector3(squareScale, squareScale, squareScale); // 스케일 설정

                // 랜덤으로 원소 타입을 선택하여 이미지 설정
                ElementType randomElement = (ElementType)Random.Range(1, 5);
                //newBlock.GetComponent<Block>().SetBlockImage(randomElement);
                newBlock.GetComponent<Block>().SetElementType(randomElement);

                // 생성된 원소 블록을 리스트에 추가합니다.
                _blocks.Add(newBlock);
            }
        }
    }

    public ElementType GetElementTypeAt(int row, int column)
    {
        // 그리드의 해당 위치에 있는 블록을 찾고, 그 블록의 ElementType을 반환합니다.
        int index = row * columns + column; // 1차원 리스트 인덱스 계산
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

    // 생성된 원소 블록들의 위치를 설정하는 메서드
    private void SetBlocksPositions()
    {
        int column_number = 0;    // 현재 열 번호
        int row_number = 0;       // 현재 행 번호
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f); // 블록 간의 추가 간격 계산용 변수
        bool row_moved = false;   // 행이 이동했는지 여부

        var square_rect = _blocks[0].GetComponent<RectTransform>(); // 블록의 RectTransform 가져오기

        // 블록 간의 오프셋 계산
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _blocks)
        {
            // 열 번호가 최대 열 수를 넘으면 행을 증가시키고 열 번호를 초기화합니다.
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            // 블록의 X, Y 위치 오프셋을 계산합니다.
            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            // 특정 열마다 추가 간격을 추가하여 블록들을 그룹화합니다.
            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            // 특정 행마다 추가 간격을 추가하여 블록들을 그룹화합니다.
            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
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

            column_number++; // 열 번호를 증가시킵니다.
        }
    }

    public void UpdateGridState(int row, int column, ElementType elementType)
    {
        int index = row * columns + column; // 인덱스 계산
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
        foreach (var square in _blocks)
        {
            var block = square.GetComponent<Block>();

            // 사용 가능한 블록이면 활성화합니다.
            if (block.CanWeUseThisSquare() == true)
            {
                block.ActivateSquare();
            }
        }
    }

}
