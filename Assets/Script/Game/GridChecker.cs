using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridChecker : MonoBehaviour
{
    public Grid grid; // 그리드 스크립트 인스턴스
    public Player player;

    // 버튼 클릭 시 호출할 검사 메서드
    public void OnCheckButtonPressed()
    {
        StartCoroutine(ProcessCardsSequentially());

    }
    // 버튼 클릭 시 모든 ActiveImage를 비활성화하는 메서드

    private void DisableAllActiveImages()
    {
        int totalBlocks = grid.rows * grid.columns;

        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = grid.GetBlockAt(i / grid.columns, i % grid.columns);
            if (block != null)
            {
                Block blockScript = block.GetComponent<Block>();
                if (blockScript != null && blockScript.activeImage != null)
                {
                    blockScript.DisactivateActiveImage(); // ActiveImage 비활성화
                }
            }
        }
    }

    // 카드들을 순차적으로 검사하는 코루틴
    private IEnumerator ProcessCardsSequentially()
    {
        int gridRows = grid.rows;
        int gridColumns = grid.columns;

        for (int i = 0; i < 3; i++)
        {
            int totalDamage = 0;
            ShapeData cardShape = CardManager.Inst.UsingCard[i].carditem.cardShape;
            int cardRows = CardManager.Inst.UsingCard[i].carditem.cardShape.rows;
            int cardColumns = CardManager.Inst.UsingCard[i].carditem.cardShape.columns;
            int cardDamage = CardManager.Inst.UsingCard[i].carditem.PowerLeft;
            ElementType CreatedElementType = CardManager.Inst.UsingCard[i].carditem.CreatedElementType;


            // 슬라이딩 윈도우 방식으로 그리드를 순회합니다.
            for (int row = 0; row <= gridRows - cardRows; row++)
            {
                for (int col = 0; col <= gridColumns - cardColumns; col++)
                {
                    // 카드 모양이 그리드의 현재 서브 그리드에 맞는지 검사합니다.
                    if (IsMatching(grid, CardManager.Inst.UsingCard[i].carditem.cardShape, row, col, CreatedElementType))
                    {
                        // 매칭된 블록을 강조하는 코루틴을 실행합니다.
                        yield return StartCoroutine(HighlightMatchedBlocks(grid, cardRows, cardColumns, row, col));
                        totalDamage += CountBlocksInShape(cardShape);
                    }
                }
            }
            //Debug.Log("count : " + totalDamage);

            totalDamage *= cardDamage;

            //Debug.Log("total damage: " + totalDamage);

            // 공격 실행
            if (totalDamage != 0)
                player.AttackWithDamage(totalDamage);

            // 다음 카드로 넘어가기 전에 약간의 딜레이를 줍니다.
            yield return new WaitForSeconds(0.5f);
        }
        DisableAllActiveImages();
    }

    private IEnumerator HighlightMatchedBlocks(Grid grid, int cardRows, int cardColumns, int startRow, int startCol)
    {
        List<GameObject> matchedBlocks = new List<GameObject>();

        // 매칭된 블록들을 찾아 리스트에 추가
        for (int row = 0; row < cardRows; row++)
        {
            for (int col = 0; col < cardColumns; col++)
            {
                GameObject block = grid.GetBlockAt(startRow + row, startCol + col);
                if (block != null)
                {
                    matchedBlocks.Add(block);
                }
            }
        }

        // 3번 빠르게 점멸
        int blinkCount = 3;
        float blinkDuration = 0.1f; // 각 점멸의 지속 시간

        for (int i = 0; i < blinkCount; i++)
        {
            // 블록을 반투명으로 설정
            foreach (var block in matchedBlocks)
            {
                Transform normalImageTransform = block.transform.Find("NormalImage");
                if (normalImageTransform != null)
                {
                    Image blockImage = normalImageTransform.GetComponent<Image>();
                    if (blockImage != null)
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = 0.5f;  // 알파 값을 0.5로 설정 (반투명)
                        blockImage.color = currentColor;
                    }
                }
            }

            yield return new WaitForSeconds(blinkDuration);

            // 블록을 다시 불투명으로 설정
            foreach (var block in matchedBlocks)
            {
                Transform normalImageTransform = block.transform.Find("NormalImage");
                if (normalImageTransform != null)
                {
                    Image blockImage = normalImageTransform.GetComponent<Image>();
                    if (blockImage != null)
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = 1.0f;  // 알파 값을 1.0으로 설정 (완전히 불투명)
                        blockImage.color = currentColor;
                    }
                }
            }

            yield return new WaitForSeconds(blinkDuration);
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
                    if (createdType != ElementType.None)
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

