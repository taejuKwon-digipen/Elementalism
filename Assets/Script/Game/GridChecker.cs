using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridChecker : MonoBehaviour
{
    public Grid grid; // 그리드 스크립트 인스턴스
    public Player player;
    public List<Card> UsingCard_;

    // 버튼 클릭 시 호출할 검사 메서드
    public void OnCheckButtonPressed()
    {
        UsingCard_.Clear();
        for (int i = 0; i < 3; i++)
        {
            UsingCard_.Add((CardManager.Inst.UsingCard[i]));
        }
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

    // 버튼 클릭 시 OraImage를 재설정하는 메서드
    private void ReplaceOraImages()
    {
        // 기존 Ora 비활성화
        int totalBlocks = grid.rows * grid.columns;
        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = grid.GetBlockAt(i / grid.columns, i % grid.columns);
            if (block != null)
            {
                Block blockScript = block.GetComponent<Block>();
                if (blockScript != null)
                {
                    blockScript.DisactivateOraImage(); // 기존 Ora 비활성화
                }
            }
        }

        // 새로운 Ora 위치를 랜덤으로 설정
        List<int> randomIndices = GetRandomIndices(totalBlocks, 2); // 랜덤한 2개의 인덱스 가져오기
        foreach (int index in randomIndices)
        {
            GameObject block = grid.GetBlockAt(index / grid.columns, index % grid.columns);
            if (block != null)
            {
                Block blockScript = block.GetComponent<Block>();
                if (blockScript != null)
                {
                    blockScript.ActivateOraImage(); // 새로운 Ora 활성화
                }
            }
        }
    }

    // 랜덤한 블록 인덱스를 생성하는 헬퍼 메서드
    private List<int> GetRandomIndices(int totalBlocks, int count)
    {
        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            int randomIndex = Random.Range(0, totalBlocks);
            if (!indices.Contains(randomIndex)) // 중복 방지
            {
                indices.Add(randomIndex);
            }
        }
        return indices;
    }

    // 카드들을 순차적으로 검사하는 코루틴
    private IEnumerator ProcessCardsSequentially()
    {
        int gridRows = grid.rows;
        int gridColumns = grid.columns;
        int addDamage = 0;

        for (int i = 0; i < 3; i++)
        {
            int totalDamage = 0;
            int blockCount = 0;
            int oraBlockCount = 0;
            ShapeData cardShape = UsingCard_[i].carditem.cardShape;
            int cardRows = UsingCard_[i].carditem.cardShape.rows;
            int cardColumns = UsingCard_[i].carditem.cardShape.columns;
            int cardDamage = UsingCard_[i].carditem.PowerLeft;
            int cardCritDamage = UsingCard_[i].carditem.PowerRight;
            int cardID = UsingCard_[i].carditem.ID;
            ElementType createdElementType = UsingCard_[i].carditem.CreatedElementType;


            // 슬라이딩 윈도우 방식으로 그리드를 순회합니다.
            for (int row = 0; row <= gridRows - cardRows; row++)
            {
                for (int col = 0; col <= gridColumns - cardColumns; col++)
                {
                    // 카드 모양이 그리드의 현재 서브 그리드에 맞는지 검사합니다.
                    if (CheckIfBlocksMatch(grid, cardShape, row, col))
                    {
                        // 매칭된 블록을 강조하는 코루틴을 실행합니다.
                        yield return StartCoroutine(HighlightMatchedBlocks(grid, cardShape, row, col));
                        // 강조 후 블록 변경
                        ChangeBlocksAfterMatch(grid, cardShape, row, col, createdElementType, cardID);

                        // 카드 블럭 수 더하기
                        blockCount += CountBlocksInShape(cardShape);

                        oraBlockCount = CountOraBlocks(grid, cardShape, row, col);

                    }
                }
            }
            //Debug.Log("count : " + totalDamage);

            if (cardID == 9)
                addDamage = blockCount;

            if(oraBlockCount > 0)
            {
                totalDamage = cardDamage * blockCount;
            }
            else
            {
                totalDamage = cardCritDamage * blockCount;
            }

            //Debug.Log("total damage: " + totalDamage);

            // 공격 실행
            if (totalDamage != 0)
                player.AttackWithDamage(totalDamage + addDamage);

            // 다음 카드로 넘어가기 전에 약간의 딜레이를 줍니다.
            yield return new WaitForSeconds(0.5f);
        }
        DisableAllActiveImages();
        ReplaceOraImages();
    }

    // Ora 활성화된 블록 수 계산
    private int CountOraBlocks(Grid grid, ShapeData shape, int startRow, int startCol)
    {
        int oraCount = 0;

        for (int row = 0; row < shape.rows; row++)
        {
            for (int col = 0; col < shape.columns; col++)
            {
                if (shape.board[row].colum[col] != ElementType.None)
                {
                    var block = grid.GetBlockAt(startRow + row, startCol + col).GetComponent<Block>();
                    if (block != null && block.IsOraActive())
                    {
                        oraCount++;
                    }
                }
            }
        }

        return oraCount;
    }

    private IEnumerator HighlightMatchedBlocks(Grid grid, ShapeData cardShape, int startRow, int startCol)
    {
        List<GameObject> matchedBlocks = new List<GameObject>();

        // 매칭된 블록들을 찾아 리스트에 추가
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                GameObject block = grid.GetBlockAt(startRow + row, startCol + col);
                if (block != null)
                {
                    // cardShape에 그 부분이 None이 아닐시 추가
                    if (cardShape.board[row].colum[col] != ElementType.None)
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
                   
                    if (blockImage != null )
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = 0.5f;  // 알파 값을 0.5로 설정 (반투명)
                        blockImage.color = currentColor;
                    }
                }
            }

            if (matchedBlocks.Count != 1)
                yield return new WaitForSeconds(blinkDuration);

            // 블록을 다시 불투명으로 설정
            foreach (var block in matchedBlocks)
            {
                Transform normalImageTransform = block.transform.Find("NormalImage");
                if (normalImageTransform != null)
                {
                    Image blockImage = normalImageTransform.GetComponent<Image>();
                    
                    if (blockImage != null )
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = 1.0f;  // 알파 값을 1.0으로 설정 (완전히 불투명)
                        blockImage.color = currentColor;
                    }
                }
            }

            if (matchedBlocks.Count != 1)
                yield return new WaitForSeconds(blinkDuration);
        }
    }


    // 그리드와 카드 모양이 일치하는지 확인하는 메서드
    private bool CheckIfBlocksMatch(Grid grid, ShapeData cardShape, int startRow, int startCol)
    {
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                ElementType cardElementType = cardShape.board[row].colum[col];
                ElementType gridElementType = grid.GetElementTypeAt(startRow + row, startCol + col);

                if (cardElementType != ElementType.None && cardElementType != gridElementType)
                {
                    return false; // 카드와 그리드의 원소가 일치하지 않으면 false 반환
                }
            }
        }
        return true; // 모든 원소가 일치하면 true 반환
    }

    // 블록을 변경하는 함수
    private void ChangeBlocksAfterMatch(Grid grid, ShapeData cardShape, int startRow, int startCol, ElementType createdElementType, int cardID)
    {
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                if (cardID == 15 || cardShape.board[row].colum[col] != ElementType.None) { 
                    if (createdElementType != ElementType.None)
                    {
                        grid.SetElementTypeAt(startRow + row, startCol + col, createdElementType);
                    }
                    else
                    {
                        grid.SetElementTypeAt(startRow + row, startCol + col, (ElementType)Random.Range(1, 5));
                    }
                }
            }
        }
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

