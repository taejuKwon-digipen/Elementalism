using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChecker : MonoBehaviour
{
    public Grid grid; // �׸��� ��ũ��Ʈ �ν��Ͻ�
    public ShapeData cardShape; // ī�� ��� (ShapeData ���)

    // ��ư Ŭ�� �� ȣ���� �˻� �޼���
    public void OnCheckButtonPressed()
    {
        int totalBlockCount = 0;

        // �׸����� ũ��� ī���� ũ�⸦ �����ɴϴ�.
        int gridRows = grid.rows;
        int gridColumns = grid.columns;
        int cardRows = cardShape.rows;
        int cardColumns = cardShape.columns;

        // �����̵� ������ ������� �׸��带 ��ȸ�մϴ�.
        for (int row = 0; row <= gridRows - cardRows; row++)
        {
            for (int col = 0; col <= gridColumns - cardColumns; col++)
            {
                // ī�� ����� �׸����� ���� ���� �׸��忡 �´��� �˻��մϴ�.
                if (IsMatching(grid, cardShape, row, col))
                {
                    // ��ġ�ϴ� ��� �ش� ��� ���� �����մϴ�.
                    totalBlockCount += CountBlocksInShape(cardShape);
                }
            }
        }

        // �˻� ����� ����մϴ�.
        Debug.Log("ī��� ��ġ�ϴ� ����� �� ����: " + totalBlockCount);
    }

    // �׸���� ī�� ����� ��ġ�ϴ��� Ȯ���ϴ� �޼���
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

    // ī�� ����� ��� ���� ����ϴ� �޼���
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
