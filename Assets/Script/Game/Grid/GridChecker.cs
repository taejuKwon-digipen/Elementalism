﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // ToList() 메서드를 사용하기 위해 추가

public class GridChecker : MonoBehaviour
{
    public Grid grid; // 그리드 스크립트 인스턴스
    public Player player;
    public static GridChecker inst;
    private List<Card> activeCards = new List<Card>();

    private void Awake()
    {
        inst = this;
    }

    public void AddActiveCard(Card card)
    {
        if (!activeCards.Contains(card))
        {
            activeCards.Add(card);
            Debug.Log($"GridChecker: Added card {card.carditem.CardName} to active cards");
        }
    }

    public void RemoveActiveCard(Card card)
    {
        if (activeCards.Contains(card))
        {
            activeCards.Remove(card);
            Debug.Log($"GridChecker: Removed card {card.carditem.CardName} from active cards");
        }
    }

    public List<Card> GetActiveCards()
    {
        return activeCards;
    }

    // 버튼 클릭 시 호출할 검사 메서드
    public void OnCheckButtonPressed()
    {
        StartCoroutine(ProcessCardsSequentially());
    }

    // 직접 호출할 수 있는 검사 메서드
    public void CheckGrid()
    {
        StartCoroutine(ProcessCardsSequentially());
    }

    public void OnResetButtonPressed()
    {
        // Grid의 모든 블록을 원래 상태로 복원
        int totalBlocks = grid.currentShape.rows * grid.currentShape.columns;
        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = grid.GetBlockAt(i / grid.currentShape.columns, i % grid.currentShape.columns);
            if (block != null)
            {
                Block blockScript = block.GetComponent<Block>();
                if (blockScript != null)
                {
                    blockScript.RestoreState();
                }
            }
        }

        // Shape들을 초기 상태로 복원
        foreach (var shape in FindObjectsOfType<Shape>())
        {
            shape.ResetToStartPosition();
        }

        // ShapeStorage에서 현재 Shape 데이터로 재설정
        ShapeStorage shapeStorage = FindObjectOfType<ShapeStorage>();
        if (shapeStorage != null)
        {
            shapeStorage.ResetToCurrentShapes();
        }
    }

    // 버튼 클릭 시 모든 ActiveImage를 비활성화하는 메서드
    private void DisableAllActiveImages()
    {
        int totalBlocks = grid.currentShape.rows * grid.currentShape.columns;

        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = grid.GetBlockAt(i / grid.currentShape.columns, i % grid.currentShape.columns);
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
        int totalBlocks = grid.currentShape.rows * grid.currentShape.columns;
        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = grid.GetBlockAt(i / grid.currentShape.columns, i % grid.currentShape.columns);
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
            GameObject block = grid.GetBlockAt(index / grid.currentShape.columns, index % grid.currentShape.columns);
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
        int gridRows = grid.currentShape.rows;
        int gridColumns = grid.currentShape.columns;
        int addDamage = 0;

        // activeCards의 복사본을 만들어서 순회
        var cardsToProcess = new List<Card>(activeCards);

        foreach (var card in cardsToProcess)
        {
            if (card == null || card.carditem == null)
            {
                Debug.LogWarning($"GridChecker: Card is null or has no carditem");
                continue;
            }

            int totalDamage = 0;
            int matchedBlockCount = 0;
            int oraBlockCount = 0;
            ShapeData cardShape = card.carditem.cardShape;
            
            if (cardShape == null)
            {
                Debug.LogWarning($"GridChecker: Card has no shape data");
                continue;
            }

            int cardDamage = card.carditem.PowerLeft;
            int cardCritDamage = card.carditem.PowerRight;
            int cardID = card.carditem.ID;
            ElementType createdElementType = card.carditem.CreatedElementType;

            // 그리드 순회하며 매칭 확인
            for (int row = 0; row <= gridRows - cardShape.rows; row++)
            {
                for (int col = 0; col <= gridColumns - cardShape.columns; col++)
                {
                    if (CheckIfBlocksMatch(grid, cardShape, row, col))
                    {
                        yield return StartCoroutine(HighlightMatchedBlocks(grid, cardShape, row, col));
                        
                        // 매칭된 블록 수 계산
                        int currentMatchedBlocks = CountMatchedBlocks(cardShape);
                        matchedBlockCount += currentMatchedBlocks;
                        
                        // 오라 블록 수 계산
                        oraBlockCount += CountOraBlocks(grid, cardShape, row, col);
                        
                        // 블록 변경
                        ChangeBlocksAfterMatch(grid, cardShape, row, col, createdElementType, cardID);
                    }
                }
            }

            // ID 9 카드의 특수 효과
            if (cardID == 9)
                addDamage = matchedBlockCount;

            // 데미지 계산
            if (oraBlockCount > 0)
            {
                // 오라 블록이 있으면 크리티컬 데미지
                totalDamage = cardCritDamage * matchedBlockCount;
                Debug.Log($"GridChecker : Critical Hit! Blocks: {matchedBlockCount}, Ora blocks: {oraBlockCount}, Damage: {totalDamage}");
            }
            else
            {
                // 일반 데미지
                totalDamage = cardDamage * matchedBlockCount;
                Debug.Log($"GridChecker : Normal Hit! Blocks: {matchedBlockCount}, Damage: {totalDamage}");
            }

            // 공격 실행
            if (totalDamage > 0)
            {
                var ability = CardAbilityManager.GetAbility(cardID);
                ability.ExecuteAbility(player, totalDamage + addDamage, oraBlockCount);
            }

            // 카드를 discardPile로 이동하고 파괴
            if (Deck.Inst != null)
            {
                Deck.Inst.AddToDiscard(card.carditem);
                Debug.Log($"GridChecker: Card {card.carditem.CardName} moved to discard pile");
                Destroy(card.gameObject);
            }

            yield return new WaitForSeconds(0.5f);
        }
        
        DisableAllActiveImages();
        ReplaceOraImages();
        Debug.Log("[GridChecker] 모든 카드 처리가 완료되었습니다.");

        // 카드 처리가 완료되면 상호작용 다시 활성화
        if (CardManager.Inst != null)
        {
            CardManager.Inst.SetInteractionsEnabled(true);
        }
    }

    // 턴이 끝날 때 호출할 메서드
    public void OnTurnEnd()
    {
        activeCards.Clear();
        Debug.Log("[GridChecker] 턴이 끝났습니다. activeCards 리스트를 비웠습니다.");
    }

    // 매칭된 블록 수를 계산하는 메서드
    private int CountMatchedBlocks(ShapeData cardShape)
    {
        int count = 0;
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                if (cardShape.board[row].colum[col] != ElementType.None)
                {
                    count++;
                }
            }
        }
        return count;
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

        // 현재 처리 중인 카드 찾기
        Card currentCard = null;
        for (int i = 0; i < activeCards.Count; i++)
        {
            if (activeCards[i] != null && activeCards[i].carditem != null && activeCards[i].carditem.cardShape == cardShape)
            {
                currentCard = activeCards[i];
                break;
            }
        }

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

            // 현재 처리 중인 카드만 반투명으로 설정
            if (currentCard != null && currentCard.gameObject != null)
            {
                Image cardImage = currentCard.GetComponent<Image>();
                if (cardImage != null)
                {
                    Color currentColor = cardImage.color;
                    currentColor.a = 0.5f;
                    cardImage.color = currentColor;
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
                    
                    if (blockImage != null)
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = 1.0f;  // 알파 값을 1.0으로 설정 (완전히 불투명)
                        blockImage.color = currentColor;
                    }
                }
            }

            // 현재 처리 중인 카드만 다시 불투명으로 설정
            if (currentCard != null && currentCard.gameObject != null)
            {
                Image cardImage = currentCard.GetComponent<Image>();
                if (cardImage != null)
                {
                    Color currentColor = cardImage.color;
                    currentColor.a = 1.0f;
                    cardImage.color = currentColor;
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
                        grid.SetElementTypeAt(startRow + row, startCol + col, (ElementType)Random.Range(1, (int)ElementType.Void));
                    }
                }
            }
        }
    }
}

