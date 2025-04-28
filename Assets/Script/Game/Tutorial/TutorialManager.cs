// UTF-8
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    private TutorialStep currentStep = TutorialStep.None;
    private RectTransform tutorialPanelRect;
    private HashSet<TutorialStep> hiddenSteps;
    private bool isWaitingForShopOpen = false;

    public TutorialStep CurrentStep => currentStep;

    // 튜토리얼 단계별 메시지 Key와 위치만 저장
    private readonly Dictionary<TutorialStep, (string key, Vector2 position)> tutorialSteps = new()
    {
        { TutorialStep.CardSelection,   ("TUTORIAL_STEP1", new Vector2(-650, -350)) },
        { TutorialStep.CardInstruction, ("TUTORIAL_STEP2", new Vector2(-650, -350)) },
        { TutorialStep.CardLimit,       ("TUTORIAL_STEP3", new Vector2(-650, -350)) },
        { TutorialStep.Block,           ("TUTORIAL_STEP4", new Vector2(+650, -350)) },
        { TutorialStep.BlockCheck,      ("TUTORIAL_STEP5", new Vector2(-650, -350)) },
        { TutorialStep.Shop,            ("TUTORIAL_STEP6", new Vector2(650, 350)) },
        { TutorialStep.ShopPurchase,    ("TUTORIAL_STEP7", new Vector2(650, 350)) }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        tutorialPanelRect = tutorialPanel.GetComponent<RectTransform>();
        hiddenSteps = new HashSet<TutorialStep>();
    }

    private void Start()
    {
        Debug.Log("[TutorialManager] Start 호출됨");
        if (GoogleSheetLoader.Instance != null)
        {
            GoogleSheetLoader.Instance.OnSheetLoaded -= OnSheetLoadedAndStartTutorial;
            GoogleSheetLoader.Instance.OnSheetLoaded += OnSheetLoadedAndStartTutorial;

            if (GoogleSheetLoader.Instance.IsLoaded)
            {
                Debug.Log("[TutorialManager] GoogleSheetLoader 이미 로드됨, 즉시 튜토리얼 시작");
                OnSheetLoadedAndStartTutorial();
            }
        }
        else
        {
            StartCoroutine(CheckTutorialMode());
        }
    }

    private void OnSheetLoadedAndStartTutorial()
    {
        Debug.Log("[TutorialManager] OnSheetLoadedAndStartTutorial 콜백 호출됨");
        StartCoroutine(CheckTutorialMode());
        ShowCurrentStep();
    }

    private IEnumerator CheckTutorialMode()
    {
        Debug.Log("[TutorialManager] CheckTutorialMode 시작");
        while (GameManager.Instance == null)
        {
            Debug.Log("[TutorialManager] GameManager.Instance 대기 중...");
            yield return new WaitForSeconds(0.1f);
        }
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
        currentStep = TutorialStep.CardSelection;
        ShowCurrentStep();
        Debug.Log("[TutorialManager] 첫 번째 튜토리얼 단계 시작");
    }

    public void NextStep()
    {
        currentStep = GetNextStep(currentStep);
        if (currentStep == TutorialStep.None)
        {
            CompleteTutorial();
            return;
        }
        ShowCurrentStep();
    }

    private TutorialStep GetNextStep(TutorialStep step)
    {
        // enum 순서대로 다음 단계 반환, 마지막이면 None 반환
        var values = (TutorialStep[])Enum.GetValues(typeof(TutorialStep));
        int idx = Array.IndexOf(values, step);
        if (idx + 1 < values.Length)
        {
            return values[idx + 1];
        }
        return TutorialStep.None;
    }

    private void ShowCurrentStep()
    {
        if (tutorialSteps.TryGetValue(currentStep, out var data))
        {
            tutorialPanel.SetActive(true);
            string text = GoogleSheetLoader.Instance != null
                ? GoogleSheetLoader.Instance.GetText(data.key)
                : data.key;
            Debug.Log($"[튜토리얼] key: {data.key}, value: {text}");
            tutorialText.text = text;
            SetPanelPosition(data.position);
            Debug.Log($"[TutorialManager] 튜토리얼 단계 {currentStep} 표시");
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
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
        hiddenSteps.Add(currentStep);
        tutorialPanel.SetActive(false);
    }

    public void OnCardSelected()
    {
        if (currentStep == TutorialStep.CardSelection)
        {
            Debug.Log("[TutorialManager] 카드 선택 완료");
            NextStep();
        }
    }

    public void OnShopOpened()
    {
        if (currentStep == TutorialStep.Shop && isWaitingForShopOpen)
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

    public bool IsCurrentStepCardLimit() => currentStep == TutorialStep.CardLimit;

    public bool ShouldPreventAutoProgress()
    {
        return currentStep == TutorialStep.CardLimit ||
               currentStep == TutorialStep.Shop ||
               currentStep == TutorialStep.ShopPurchase;
    }

    public enum TutorialStep
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