using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject elemental;
    public Vector2 startPosition = new Vector2 (0, 0);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2 (0, 0);
    private List<GameObject> _elementals = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnElementals();
        SetElementalsPositions();
    }

    private void SpawnElementals()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                // Element 프리팹 생성
                GameObject newElemental = Instantiate(elemental) as GameObject;
                newElemental.transform.SetParent(this.transform);
                newElemental.transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                // 랜덤으로 원소 타입을 선택하여 이미지 설정
                ElementType randomElement = (ElementType)Random.Range(0, 4);
                newElemental.GetComponent<Elemental>().SetRandomElementImage(randomElement);

                // 생성된 Element를 리스트에 추가
                _elementals.Add(newElemental);
            }
        }
    }
    private void SetElementalsPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _elementals[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset; 
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset; 

        foreach (GameObject square in _elementals)
        {
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;

                column_number = 0 ;
                row_number++;
                row_moved = false;  
            }

            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            if(row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset);

            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset, 0.0f);
            column_number++;
        }
    }
}
