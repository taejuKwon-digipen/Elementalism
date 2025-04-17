// UTF-8
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    private int currentStepIndex = -1;
    private RectTransform tutorialPanelRect;
    private bool[] hiddenSteps;
    private bool isWaitingForShopOpen = false;

    // 튜토리얼 단계별 메시지와 위치 정의
    private readonly (string message, Vector2 position)[] tutorialSteps = new[]
    {
        //card selection
        ("카드를 선택하고 위로 드래그를 하여 적을 공격하세요!", new Vector2(-650, -350)),
        
        //card instruction
        ("카드 왼쪽 숫자는 일반 공격이고, 오른쪽 숫자는 치명타 공격입니다.\n노란색 테두리가 있는 블록을 깨면 치명타 데미지를 받습니다.", new Vector2(-650, -350)),
        
        //card limit
        ("카드는 한 턴에 3장만 사용 가능합니다!\n카드를 리필하고 싶으면 턴 종료 버튼을 누르세요!\n*주의 : 턴 종료시 몬스터가 앞으로 조금씩 이동합니다.", new Vector2(-650, -350)),
        
        //block
        ("오른쪽에 블럭을 가운데 퍼즐에 넣어 모양을 만들수 있습니다!\n마우스 오른쪽을 클릭하거나 QE를 눌러 블럭을 회전할 수 있습니다.", new Vector2(0, -200)),
        
        //block check
        ("카드의 모양은 왼쪽부터 오른쪽으로, 위에서 아래 순서로 검사합니다!", new Vector2(0, -200)),
        
        //shop
        ("상점에서 새로운 카드를 구매하세요!\n카드를 구매하면 덱에 추가가 되어 다음 레벨에서 사용 가능합니다!", new Vector2(0, -200)),
        
        //shop purchase
        ("돈을 사용하여 체력을 회복 할 수도 있고, 맵을 눌러 다음 레벨로 갈 수도 있습니다.", new Vector2(0, -200))
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        tutorialPanelRect = tutorialPanel.GetComponent<RectTransform>();
        hiddenSteps = new bool[System.Enum.GetValues(typeof(TutorialStep)).Length];
    }

    private void Start()
    {
        Debug.Log("[TutorialManager] Start 호출됨");
        StartCoroutine(CheckTutorialMode());
    }

    private IEnumerator CheckTutorialMode()
    {
        Debug.Log("[TutorialManager] CheckTutorialMode 시작");
        
        // GameManager가 초기화될 때까지 대기
        while (GameManager.Instance == null)
        {
            Debug.Log("[TutorialManager] GameManager.Instance 대기 중...");
            yield return new WaitForSeconds(0.1f);
        }

        // GameManager의 Start 메서드가 완료될 때까지 추가 대기
        yield return new WaitForSeconds(0.2f);
        
        Debug.Log($"[TutorialManager] GameManager 체크 - IsTutorialMode: {GameManager.Instance.IsTutorialMode}");
        
        if (GameManager.Instance.IsTutorialMode)
        {
            Debug.Log("[TutorialManager] 튜토리얼 시작");
            StartTutorial();
        }
        else
        {
            Debug.Log("[TutorialManager] 튜토리얼 모드가 아님");
        }
    }

    private void StartTutorial()
    {
        currentStepIndex = -1;
        NextStep();
        Debug.Log("[TutorialManager] 첫 번째 튜토리얼 단계 시작");
    }

    // 다음 튜토리얼 단계로 진행하는 함수
    public void NextStep()
    {
        currentStepIndex++;
        
        if (currentStepIndex >= tutorialSteps.Length)
        {
            CompleteTutorial();
            return;
        }

        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        var (message, position) = tutorialSteps[currentStepIndex];
        
        tutorialPanel.SetActive(true);
        tutorialText.text = message;
        SetPanelPosition(position);
        
        Debug.Log($"[TutorialManager] 튜토리얼 단계 {currentStepIndex} 표시");
    }

    private void SetPanelPosition(Vector2 position)
    {
        if (tutorialPanelRect != null)
        {
            Debug.Log($"[TutorialManager] 패널 위치 변경: {position}");
            tutorialPanelRect.anchoredPosition = position;
        }
    }

    public void OnCloseButtonClick()
    {
        hiddenSteps[(int)currentStepIndex] = true;
        tutorialPanel.SetActive(false);
    }

    public void OnCardSelected()
    {
        if (currentStepIndex == 0)
        {
            Debug.Log("[TutorialManager] 카드 선택 완료");
            NextStep();
        }
    }

    public void OnShopOpened()
    {
        if (currentStepIndex == 5 && isWaitingForShopOpen)
        {
            Debug.Log("[TutorialManager] 상점 열림");
            isWaitingForShopOpen = false;
            NextStep();
        }
    }

    private void CompleteTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 완료");
        tutorialPanel.SetActive(false);
        GameManager.Instance.CompleteTutorial();
    }

    private enum TutorialStep
    {
        None,
        CardSelection,
        CardInstruction,
        CardLimit,
        Block,
        BlockCheck,
        Shop,
        ShopPurchase
    }
} 