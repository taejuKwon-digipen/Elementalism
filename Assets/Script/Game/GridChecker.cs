using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChecker : MonoBehaviour
{
    public Grid grid; // 그리드 스크립트 인스턴스

    // 버튼 클릭 시 호출할 검사 메서드
    public void OnCheckButtonPressed()
    {
        int totalBlockCount = 0;
        int damage = 0;

        // 그리드의 크기와 카드의 크기를 가져옵니다.
        int gridRows = grid.rows;
        int gridColumns = grid.columns;



        for (int i = 0; i < 3; i++)
        {
            int cardRows = CardManager.Inst.UsingCard[i].carditem.cardShape.rows;
            int cardColumns = CardManager.Inst.UsingCard[i].carditem.cardShape.columns;
            int cardDamage = CardManager.Inst.UsingCard[i].carditem.PowerLeft;
            int cardCritDamage = CardManager.Inst.UsingCard[i].carditem.PowerRight;
            ElementType CreatedElementType = CardManager.Inst.UsingCard[i].carditem.CreatedElementType;

            // 슬라이딩 윈도우 방식으로 그리드를 순회합니다.
            for (int row = 0; row <= gridRows - cardRows; row++)
            {
                for (int col = 0; col <= gridColumns - cardColumns; col++)
                {
                    // 카드 모양이 그리드의 현재 서브 그리드에 맞는지 검사합니다.
                    if (IsMatching(grid, CardManager.Inst.UsingCard[i].carditem.cardShape, row, col, CreatedElementType))
                    {
                        // 일치하는 경우 해당 블록 수를 누적합니다.
                        totalBlockCount += CountBlocksInShape(CardManager.Inst.UsingCard[i].carditem.cardShape);
                    }
                }
            }

            damage = cardDamage * totalBlockCount;
            // 검사 결과를 출력합니다.
            Debug.Log("카드와 일치하는 블록의 총 개수: " + totalBlockCount + "\n카드 데미지:" + damage);
            totalBlockCount = 0;
            damage = 0;
        }

    }

    // 그리드와 카드 모양이 일치하는지 확인하는 메서드
    private bool IsMatching(Grid grid, ShapeData cardShape, int startRow, int startCol, ElementType createdType)
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

        // 카드와 일치하는 블록을 찾았으면, 그 위치의 블록을 None으로 설정

        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                if (cardShape.board[row].colum[col] != ElementType.None)
                {
                    if(createdType != ElementType.None)
                    { 
                        grid.SetElementTypeAt(startRow + row, startCol + col, createdType);
                    }
                    else
                    {
                        grid.SetElementTypeAt(startRow + row, startCol + col, (ElementType)Random.Range(1, 5));
                    }
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
