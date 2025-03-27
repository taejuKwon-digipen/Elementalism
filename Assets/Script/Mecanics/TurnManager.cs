using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GridChecker gridChecker;

    private bool isProcessingTurn = false;

    void Awake() => Inst = this;

    public void OnTurnEndButtonPressed()
    {
        if (isProcessingTurn) return;
        StartCoroutine(ProcessTurnEnd());
    }

    private IEnumerator ProcessTurnEnd()
    {
        isProcessingTurn = true;
        Debug.Log("[TurnManager] 턴 엔드 처리 시작");

        // 1. Shape들이 3개로 채워지기
        if (grid != null)
        {
            grid.OnTurnEndButton();
            yield return new WaitForSeconds(0.5f);
        }

        // 2. 덱에서 웨이팅 카드를 4장이 될 때까지 뽑기
        if (cardManager != null)
        {
            yield return StartCoroutine(cardManager.RefillWaitingCards());
        }

        // 3. 적들이 앞으로 한 칸씩 이동
        if (enemyManager != null)
        {
            enemyManager.StartEnemyTurn();
            yield return new WaitForSeconds(0.5f);
        }

        isProcessingTurn = false;
        Debug.Log("[TurnManager] 턴 엔드 처리 완료");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
