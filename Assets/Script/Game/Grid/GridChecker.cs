using System.Collections;
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
        var cardsToProcess = new List<Card>(activeCards); // 안전한 순회를 위해 복사본 사용
        // activeCards.Clear(); // 카드를 처리하는 동안에는 activeCards를 유지하고, 모든 처리가 끝난 후 PostProcessGridActions에서 OnTurnEnd를 통해 비우도록 합니다.

        foreach (var card in cardsToProcess)
        {
            if (card == null || card.carditem == null)
            {
                Debug.LogWarning("GridChecker: Card is null or has no carditem. 리스트에서 제거합니다.");
                if (card != null) activeCards.Remove(card); // 원본 리스트에서도 제거 시도
                if (card != null && card.gameObject != null) Destroy(card.gameObject);
                continue;
            }
            yield return StartCoroutine(ProcessSingleCard(card));
        }
        
        // 모든 카드 처리 후, 모든 공격 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() => BallBehavior.activeAttackAnimations == 0);

        PostProcessGridActions();
    }

    private IEnumerator ProcessSingleCard(Card card)
    {
        ShapeData cardShape = card.carditem.cardShape;
        if (cardShape == null)
        {
            Debug.LogWarning($"GridChecker: 카드 '{card.carditem.CardName}'에 ShapeData가 없습니다. 이 카드를 건너뜁니다.");
            DiscardAndDestroyCard(card, false); // 매칭 없이 카드 제거
            yield break; 
        }

        int cardID = card.carditem.ID;
        ElementType createdElementType = card.carditem.CreatedElementType;
        int gridRows = grid.currentShape.rows;
        int gridColumns = grid.currentShape.columns;

        int totalMatchedBlocksThisCard = 0;
        int totalOraBlocksThisCard = 0;
        
        // 그리드를 순회하며 매칭 확인 및 즉시 처리
        for (int r = 0; r <= gridRows - cardShape.rows; r++)
        {
            for (int c = 0; c <= gridColumns - cardShape.columns; c++)
            {
                if (CheckIfBlocksMatch(grid, cardShape, r, c))
                {
                    // 매치 발견!
                    // HighlightMatchedBlocks 호출 시 currentCard 전달
                    yield return StartCoroutine(HighlightMatchedBlocks(grid, cardShape, r, c, card)); 
                    
                    int currentShapeMatchedBlocks = CountMatchedBlocksInShape(cardShape);
                    totalMatchedBlocksThisCard += currentShapeMatchedBlocks;
                    totalOraBlocksThisCard += CountOraBlocksInMatch(grid, cardShape, r, c);

                    // 그리드 상태 변경 (즉시 적용)
                    ChangeBlocksAfterMatch(grid, cardShape, r, c, createdElementType, cardID);
                }
            }
        }

        // 해당 카드의 모든 매칭 처리 후 효과 적용
        if (totalMatchedBlocksThisCard > 0)
        {
            ApplyCardEffect(card, totalMatchedBlocksThisCard, totalOraBlocksThisCard);
        }
        
        DiscardAndDestroyCard(card, true); // 카드 사용 처리

        yield return new WaitForSeconds(0.5f); // 카드 처리 간 딜레이
    }
    
    // FindAllMatchLocationsForCard 메소드는 ProcessSingleCard에서 직접 사용되지 않으므로,
    // 다른 곳에서 필요하지 않다면 제거하거나 주석 처리할 수 있습니다.
    private List<(int r, int c)> FindAllMatchLocationsForCard(Grid currentGrid, ShapeData cardShape, int gridRows, int gridColumns)
    {
        List<(int r, int c)> locations = new List<(int r, int c)>();
        for (int r = 0; r <= gridRows - cardShape.rows; r++)
        {
            for (int c = 0; c <= gridColumns - cardShape.columns; c++)
            {
                if (CheckIfBlocksMatch(currentGrid, cardShape, r, c))
                {
                    locations.Add((r, c));
                }
            }
        }
        return locations; // 항상 리스트를 반환하도록 수정
    }

    private IEnumerator HighlightMatchedBlocks(Grid currentGrid, ShapeData cardShape, int startRow, int startCol, Card currentCard)
    {
        List<GameObject> matchedBlockObjects = new List<GameObject>();

        // 매칭된 블록들을 찾아 리스트에 추가
        for (int row = 0; row < cardShape.rows; row++)
        {
            for (int col = 0; col < cardShape.columns; col++)
            {
                GameObject block = currentGrid.GetBlockAt(startRow + row, startCol + col);
                if (block != null)
                {
                    // cardShape에 그 부분이 None이 아닐시 추가
                    if (cardShape.board[row].colum[col] != ElementType.None)
                        matchedBlockObjects.Add(block);                    
                }
            }
        }

        // 3번 빠르게 점멸
        int blinkCount = 3;
        float blinkDuration = 0.1f; // 각 점멸의 지속 시간

        for (int i = 0; i < blinkCount; i++)
        {
            // 블록을 반투명으로 설정
            foreach (var block in matchedBlockObjects)
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

            if (matchedBlockObjects.Count != 1)
                yield return new WaitForSeconds(blinkDuration);

            // 블록을 다시 불투명으로 설정
            foreach (var block in matchedBlockObjects)
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

            if (matchedBlockObjects.Count != 1)
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
                        // ChallengeLevel이 1일 때 Earth를 Fire로 변경
                        ElementType modifiedType = createdElementType;
                        if (GameManager.Instance != null && GameManager.Instance.ChallengeLevel == 1 && createdElementType == ElementType.Earth)
                        {
                            modifiedType = ElementType.Fire;
                            Debug.Log($"[GridChecker] ChallengeLevel 1: Earth Type이 Fire Type으로 변경됨 (위치: {startRow + row}, {startCol + col})");
                        }
                        grid.SetElementTypeAt(startRow + row, startCol + col, modifiedType);
                    }
                    else
                    {
                        // 랜덤 생성 시에도 Earth가 나오면 Fire로 변경
                        ElementType randomType = (ElementType)Random.Range(1, (int)ElementType.Void);
                        if (GameManager.Instance != null && GameManager.Instance.ChallengeLevel == 1 && randomType == ElementType.Earth)
                        {
                            randomType = ElementType.Fire;
                            Debug.Log($"[GridChecker] ChallengeLevel 1: 랜덤 생성된 Earth Type이 Fire Type으로 변경됨 (위치: {startRow + row}, {startCol + col})");
                        }
                        grid.SetElementTypeAt(startRow + row, startCol + col, randomType);
                    }
                }
            }
        }
    }

    private void SetAlphaForGameObjects(List<GameObject> gameObjects, float alpha, Card cardToAffect)
    {
        foreach (var blockObj in gameObjects)
        {
            // Block.cs에 public Image normalImage; 가 있고 연결되어 있다고 가정.
            // Block blockScript = blockObj.GetComponent<Block>();
            // if (blockScript != null && blockScript.normalImage != null) 
            // {
            //     Color color = blockScript.normalImage.color;
            //     color.a = alpha;
            //     blockScript.normalImage.color = color;
            // }
            // else 
            // {
                // 임시로 Find 사용 (Block.cs 수정 권장)
                Transform normalImageTransform = blockObj.transform.Find("NormalImage");
                if (normalImageTransform != null)
                {
                    Image blockImage = normalImageTransform.GetComponent<Image>();
                    if (blockImage != null)
                    {
                        Color currentColor = blockImage.color;
                        currentColor.a = alpha;
                        blockImage.color = currentColor;
                    }
                }
            // }
        }

        if (cardToAffect != null && cardToAffect.gameObject != null)
        {
            Image cardImageComponent = cardToAffect.GetComponent<Image>(); 
            // if (cardImageComponent == null) cardImageComponent = cardToAffect.GetComponentInChildren<Image>(true); // 비활성화된 자식 포함 주석 처리

            if (cardImageComponent != null)
            {
                Color currentColor = cardImageComponent.color;
                currentColor.a = alpha;
                cardImageComponent.color = currentColor;
            }
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

    private void PostProcessGridActions()
    {
        DisableAllActiveImages();
        ReplaceOraImages();
        Debug.Log("[GridChecker] 모든 카드 처리가 완료되었습니다.");

        // activeCards 리스트를 여기서 비웁니다. (OnTurnEnd는 외부에서 턴 종료 시 호출될 수 있으므로 여기서 직접 처리)
        // activeCards.Clear(); // ProcessCardsSequentially 시작 시점에 activeCards를 복사해서 사용하고, 원본은 여기서 비우거나, CardManager에서 턴 종료 시 호출
        // OnTurnEnd() 메서드는 턴이 완전히 종료될 때 CardManager 등 외부에서 호출되도록 유지하는 것이 좋을 수 있습니다.
        // 여기서는 단순히 카드 처리 사이클이 끝났음을 의미하므로, activeCards는 CardManager가 관리하도록 둘 수 있습니다.
        // 또는, GridChecker가 activeCards를 독자적으로 관리한다면 여기서 Clear하는 것이 맞습니다.
        // 현재 Card.cs에서 GridChecker.inst.AddActiveCard(this)를 통해 카드가 추가되므로,
        // GridChecker에서 처리 완료 후 비워주는 것이 적절해 보입니다.
        activeCards.Clear();
        Debug.Log("[GridChecker] activeCards 리스트를 비웠습니다. (PostProcessGridActions)");

        // 카드 처리가 완료되고 모든 애니메이션이 끝나면 상호작용 다시 활성화
        if (CardManager.Inst != null)
        {
            CardManager.Inst.SetInteractionsEnabled(true);
        }
    }

    private void DiscardAndDestroyCard(Card card, bool discard)
    {
        if (discard)
        {
            if (Deck.Inst != null)
            {
                Deck.Inst.AddToDiscard(card.carditem);
                Debug.Log($"GridChecker: Card {card.carditem.CardName} moved to discard pile");
            }
        }
        else
        {
            Debug.Log($"GridChecker: Card {card.carditem.CardName} discarded");
        }

        if (card != null && card.gameObject != null)
        {
            Destroy(card.gameObject);
        }
    }

    // 누락된 헬퍼 메소드들 추가
    private int CountMatchedBlocksInShape(ShapeData cardShape)
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

    private int CountOraBlocksInMatch(Grid currentGrid, ShapeData shape, int startRow, int startCol)
    {
        int oraCount = 0;
        for (int row = 0; row < shape.rows; row++)
        {
            for (int col = 0; col < shape.columns; col++)
            {
                if (shape.board[row].colum[col] != ElementType.None)
                {
                    GameObject blockObj = currentGrid.GetBlockAt(startRow + row, startCol + col);
                    if (blockObj != null)
                    {
                        Block blockScript = blockObj.GetComponent<Block>();
                        if (blockScript != null && blockScript.IsOraActive())
                        {
                            oraCount++;
                        }
                    }
                }
            }
        }
        return oraCount;
    }

    private void ApplyCardEffect(Card card, int matchedBlockCount, int oraBlockCount)
    {
        int cardID = card.carditem.ID;
        int baseDamage = card.carditem.PowerLeft;
        int critDamage = card.carditem.PowerRight;

        int additionalDamageFromEffect = CalculateCardSpecificAdditionalDamage(cardID, matchedBlockCount);
        int finalDamage = CalculateFinalDamage(baseDamage, critDamage, matchedBlockCount, oraBlockCount);
        
        var ability = CardAbilityManager.GetAbility(cardID);
        if (ability != null)
        {
            ability.ExecuteAbility(player, finalDamage + additionalDamageFromEffect, oraBlockCount);
        }
        else
        {
            Debug.LogWarning($"[GridChecker] ID: {cardID} 카드의 Ability를 찾을 수 없습니다. 기본 공격을 수행합니다.");
            // 예: player.AttackTarget(finalDamage + additionalDamageFromEffect); 
        }
    }

    private int CalculateCardSpecificAdditionalDamage(int cardID, int matchedBlockCount)
    {
        if (cardID == 9) // 매직 넘버: 특수 효과 카드 ID
        {
            return matchedBlockCount;
        }
        return 0;
    }

    private int CalculateFinalDamage(int baseDamage, int critDamage, int matchedBlockCount, int oraBlockCount)
    {
        if (oraBlockCount > 0)
        {
            Debug.Log($"GridChecker : Critical Hit! Blocks: {matchedBlockCount}, Ora blocks: {oraBlockCount}, Damage: {critDamage * matchedBlockCount}");
            return critDamage * matchedBlockCount;
        }
        else
        {
            Debug.Log($"GridChecker : Normal Hit! Blocks: {matchedBlockCount}, Damage: {baseDamage * matchedBlockCount}");
            return baseDamage * matchedBlockCount;
        }
    }
}

