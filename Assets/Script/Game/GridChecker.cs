using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChecker : MonoBehaviour
{
    public Grid grid; // 그리드 스크립트 인스턴스
    public ShapeData cardShape; // 카드 모양 (ShapeData 사용)

    // 버튼 클릭 시 호출할 검사 메서드
    public void OnCheckButtonPressed()
    {
        int totalBlockCount = 0;

        // 그리드의 크기와 카드의 크기를 가져옵니다.
        int gridRows = grid.rows;
        int gridColumns = grid.columns;
        int cardRows = cardShape.rows;
        int cardColumns = cardShape.columns;

        // 슬라이딩 윈도우 방식으로 그리드를 순회합니다.
        for (int row = 0; row <= gridRows - cardRows; row++)
        {
            for (int col = 0; col <= gridColumns - cardColumns; col++)
            {
                // 카드 모양이 그리드의 현재 서브 그리드에 맞는지 검사합니다.
                if (IsMatching(grid, cardShape, row, col))
                {
                    // 일치하는 경우 해당 블록 수를 누적합니다.
                    totalBlockCount += CountBlocksInShape(cardShape);
                }
            }
        }

        // 검사 결과를 출력합니다.
        Debug.Log("카드와 일치하는 블록의 총 개수: " + totalBlockCount);
    }

    // 그리드와 카드 모양이 일치하는지 확인하는 메서드
    private bool IsMatching(Grid grid, ShapeData cardShape, int startRow, int startCol)
    {
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                ElementType cardElementType = cardShape.board[row].colum[col];
                ElementType gridElementType = grid.GetElementTypeAt(startRow + row, startCol + col);

                if (cardElementType != ElementType.None && cardElementType != gridElementType)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 카드 모양의 블록 수를 계산하는 메서드
    private int CountBlocksInShape(ShapeData shape)
    {
        int count = 0;
        for (int row = 0; row < shape.rows; row++)
        {
            for (int col = 0; col < shape.columns; col++)
            {
                if (shape.board[row].colum[col] != ElementType.None)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
