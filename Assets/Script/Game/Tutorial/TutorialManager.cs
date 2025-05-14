// UTF-8
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References Names - 인스펙터 대신 이름으로 찾습니다.")]
    [SerializeField] private string tutorialPanelName = "TutorialPanel"; // 실제 씬에 있는 패널 오브젝트 이름
    [SerializeField] private string tutorialTextName = "TutorialText";   // 실제 씬에 있는 텍스트 오브젝트 이름
    [SerializeField] private string nextButtonName = "NextButton";     // 실제 씬에 있는 다음 버튼 오브젝트 이름
    [SerializeField] private string closeButtonName = "CloseButton";   // 실제 씬에 있는 닫기 버튼 오브젝트 이름

    // UI 참조는 동적으로 할당
    private GameObject tutorialPanel;
    private TextMeshProUGUI tutorialText;
    private Button nextButton;
    private Button closeButton;

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
        hiddenSteps = new HashSet<TutorialStep>();
        // tutorialPanelRect 초기화는 FindUIReferences 이후로 이동
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        Debug.Log("[TutorialManager] Start 호출됨");
        // FindUIReferences(); // 첫 씬 로드 시에도 UI 찾아보기 -> OnSceneLoaded에서 처리하므로 중복 호출 방지

        if (GoogleSheetLoader.Instance != null)
        {
            GoogleSheetLoader.Instance.OnSheetLoaded -= OnSheetLoadedAndStartTutorial;
            GoogleSheetLoader.Instance.OnSheetLoaded += OnSheetLoadedAndStartTutorial;

            if (GoogleSheetLoader.Instance.IsLoaded)
            {
                Debug.Log("[TutorialManager] GoogleSheetLoader 이미 로드됨, 즉시 튜토리얼 시작 로직 호출");
                OnSheetLoadedAndStartTutorial();
            }
            else
            {
                 Debug.Log("[TutorialManager] GoogleSheetLoader 로드 대기 중...");
            }
        }
        else
        {
            Debug.LogWarning("[TutorialManager] GoogleSheetLoader.Instance가 null입니다. 튜토리얼 텍스트 로드에 문제가 있을 수 있습니다.");
            // GoogleSheetLoader가 없는 경우에도 CheckTutorialMode는 진행하도록 함 (GameManager의 상태에 따라)
            StartCoroutine(CheckTutorialMode());
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[TutorialManager] OnSceneLoaded: {scene.name} 씬 로드됨");
        FindUIReferences();
        // 씬이 로드될 때 GameManager의 상태에 따라 튜토리얼을 다시 시작하거나 현재 스텝을 표시할지 결정
        if (GameManager.Instance != null && GameManager.Instance.IsTutorialMode)
        {
            ShowCurrentStep(); // GameManager가 튜토리얼 모드이면 현재 스텝 UI를 다시 표시 시도
        }
        else
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false); // 튜토리얼 모드가 아니면 패널 비활성화
        }
    }

    private void FindUIReferences()
    {
        Debug.Log("[TutorialManager] FindUIReferences 시작");
        GameObject panelObj = GameObject.Find(tutorialPanelName);
        if (panelObj != null)
        {
            tutorialPanel = panelObj;
            tutorialPanelRect = tutorialPanel.GetComponent<RectTransform>();
            Debug.Log($"[TutorialManager] '{tutorialPanelName}' (TutorialPanel) 찾음: {tutorialPanel != null}");

            Transform textTr = tutorialPanel.transform.Find(tutorialTextName); 
            if (textTr != null) 
            {
                tutorialText = textTr.GetComponent<TextMeshProUGUI>();
                Debug.Log($"[TutorialManager] '{tutorialTextName}' (TutorialText) Transform 찾음: {textTr != null}, TextMeshProUGUI 컴포넌트 유효성: {tutorialText != null}");
            }
            else
            {
                Debug.LogWarning($"[TutorialManager] '{tutorialTextName}' (TutorialText) Transform을 '{tutorialPanelName}' 자식에서 찾을 수 없습니다.");
                tutorialText = null; // 명시적으로 null 처리
            }
            
            Transform nextBtnTr = tutorialPanel.transform.Find(nextButtonName); 
            if (nextBtnTr != null)
            {
                nextButton = nextBtnTr.GetComponent<Button>();
                Debug.Log($"[TutorialManager] '{nextButtonName}' (NextButton) Transform 찾음: {nextBtnTr != null}, Button 컴포넌트 유효성: {nextButton != null}");
            }
            else
            {
                Debug.LogWarning($"[TutorialManager] '{nextButtonName}' (NextButton) Transform을 '{tutorialPanelName}' 자식에서 찾을 수 없습니다.");
                nextButton = null; // 명시적으로 null 처리
            }
            
            Transform closeBtnTr = tutorialPanel.transform.Find(closeButtonName); 
            if (closeBtnTr != null)
            {
                closeButton = closeBtnTr.GetComponent<Button>();
                Debug.Log($"[TutorialManager] '{closeButtonName}' (CloseButton) Transform 찾음: {closeBtnTr != null}, Button 컴포넌트 유효성: {closeButton != null}");
                if (closeButton != null) closeButton.onClick.AddListener(OnCloseButtonClick); // 리스너는 여기서 추가
            }
            else
            {
                Debug.LogWarning($"[TutorialManager] '{closeButtonName}' (CloseButton) Transform을 '{tutorialPanelName}' 자식에서 찾을 수 없습니다.");
                closeButton = null; // 명시적으로 null 처리
            }
        }
        else
        {
            Debug.LogWarning($"[TutorialManager] '{tutorialPanelName}' (TutorialPanel)을 찾을 수 없습니다. 이 씬에는 튜토리얼 UI가 없거나 이름이 다를 수 있습니다.");
            tutorialPanel = null; // 참조 초기화
            tutorialPanelRect = null;
            tutorialText = null;
            nextButton = null;
            closeButton = null;
        }
        Debug.Log("[TutorialManager] FindUIReferences 종료");
    }

    private void OnSheetLoadedAndStartTutorial()
    {
        Debug.Log("[TutorialManager] OnSheetLoadedAndStartTutorial 콜백 호출됨");
        StartCoroutine(CheckTutorialMode());
        // ShowCurrentStep(); // CheckTutorialMode 내부에서 StartTutorial을 호출하고, 거기서 ShowCurrentStep을 하므로 중복 호출 방지
    }

    private IEnumerator CheckTutorialMode()
    {
        Debug.Log("[TutorialManager] CheckTutorialMode 시작");
        while (GameManager.Instance == null) // GameManager 로드 대기
        {
            Debug.Log("[TutorialManager] GameManager.Instance 대기 중...");
            yield return new WaitForSeconds(0.1f);
        }
        // GoogleSheetLoader도 여기서 로드 완료되었는지 확인하는 것이 안전할 수 있음
        while (GoogleSheetLoader.Instance == null || !GoogleSheetLoader.Instance.IsLoaded)
        {
            Debug.Log("[TutorialManager] GoogleSheetLoader 로드 대기 중 (CheckTutorialMode)...");
            yield return new WaitForSeconds(0.1f); 
        }

        yield return new WaitForSeconds(0.2f); // GameManager의 Start 이후 IsTutorialMode가 설정될 시간을 줌
        Debug.Log($"[TutorialManager] GameManager.Instance.IsTutorialMode 체크: {GameManager.Instance.IsTutorialMode}");
        if (GameManager.Instance.IsTutorialMode)
        {
            Debug.Log("[TutorialManager] 튜토리얼 시작 조건 충족 (GameManager.IsTutorialMode is true). StartTutorial() 호출 시도.");
            StartTutorial();
        }
        else
        {
            Debug.Log("[TutorialManager] 튜토리얼 모드가 아님 (GameManager.IsTutorialMode is false). 패널 비활성화 시도.");
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            currentStep = TutorialStep.None; // 튜토리얼 모드가 아니면 현재 스텝도 초기화
        }
    }

    private void StartTutorial()
    {
        Debug.Log("[TutorialManager] StartTutorial 메서드 진입.");
        // UI 참조가 유효한지 먼저 확인
        if (tutorialPanel == null || tutorialText == null)
        {
            Debug.LogWarning($"[TutorialManager] StartTutorial 호출되었으나, UI 참조가 없어 튜토리얼을 시작할 수 없습니다. tutorialPanel is null: {tutorialPanel == null}, tutorialText is null: {tutorialText == null}. FindUIReferences가 먼저 성공해야 합니다.");
            currentStep = TutorialStep.None;
            return;
        }
        currentStep = TutorialStep.CardSelection;
        ShowCurrentStep();
        Debug.Log("[TutorialManager] 첫 번째 튜토리얼 단계 시작: " + currentStep);
    }

    public void NextStep()
    {
        if (currentStep == TutorialStep.None) return; // 이미 완료되었거나 시작 전이면 무시

        TutorialStep next = GetNextStep(currentStep);
        Debug.Log($"[TutorialManager] NextStep 호출됨. 현재: {currentStep}, 다음 예정: {next}");
        currentStep = next;

        if (currentStep == TutorialStep.None)
        {
            CompleteTutorial();
            return;
        }
        ShowCurrentStep();
    }

    private TutorialStep GetNextStep(TutorialStep step)
    {
        var values = (TutorialStep[])Enum.GetValues(typeof(TutorialStep));
        int idx = Array.IndexOf(values, step);

        // 다음 단계로 이동하되, 숨겨진 단계는 건너뛰기
        do
        {
            idx++;
            if (idx >= values.Length) return TutorialStep.None; // 마지막 단계 이후
        } while (hiddenSteps.Contains(values[idx]));
        
        return values[idx];
    }

    private void ShowCurrentStep()
    {
        if (tutorialPanel == null || tutorialText == null) 
        {
            if (GameManager.Instance != null && GameManager.Instance.IsTutorialMode)
            {
                Debug.LogWarning("[TutorialManager] ShowCurrentStep: UI 참조(tutorialPanel 또는 tutorialText)가 null입니다. 현재 씬에 UI가 없거나 FindUIReferences 실패.");
            }
            return;
        }

        if (tutorialSteps.TryGetValue(currentStep, out var data))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsTutorialMode && currentStep != TutorialStep.None) {
                 tutorialPanel.SetActive(false); // 튜토리얼 모드가 아니면 강제로 끔
                 return;
            }

            tutorialPanel.SetActive(true);
            string textToShow;
            if (GoogleSheetLoader.Instance != null && GoogleSheetLoader.Instance.IsLoaded)
            {
                textToShow = GoogleSheetLoader.Instance.GetText(data.key);
            }
            else
            {
                textToShow = $"{data.key} (내용 로드 실패)"; 
            }
            Debug.Log($"[튜토리얼] 표시 중. Key: {data.key}, Value: {textToShow}, Step: {currentStep}");
            tutorialText.text = textToShow;
            SetPanelPosition(data.position);
        }
        else
        {
            Debug.Log($"[TutorialManager] 현재 단계 {currentStep}에 대한 데이터 없음, 패널 비활성화.");
            if (tutorialPanel != null) tutorialPanel.SetActive(false); // null 체크 추가
        }
    }

    private void SetPanelPosition(Vector2 position)
    {
        if (tutorialPanelRect != null)
        {
            // Debug.Log($"[TutorialManager] 패널 위치 변경 시도: {position}"); // 너무 빈번한 로그는 주석 처리
            tutorialPanelRect.anchoredPosition = position;
        }
        else if (GameManager.Instance != null && GameManager.Instance.IsTutorialMode)
        {
            Debug.LogWarning("[TutorialManager] SetPanelPosition: tutorialPanelRect가 null입니다.");
        }
    }

    public void OnCloseButtonClick()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        hiddenSteps.Add(currentStep);
        Debug.Log($"[TutorialManager] 튜토리얼 단계 {currentStep} 숨김 처리됨.");
    }
    public void OnCardSelected()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsTutorialMode && currentStep == TutorialStep.CardSelection)
        {
            Debug.Log("[TutorialManager] 카드 선택 완료 (튜토리얼 진행)");
            NextStep();
        }
    }

    public void OnShopOpened()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsTutorialMode && currentStep == TutorialStep.Shop && isWaitingForShopOpen)
        {
            Debug.Log("[TutorialManager] 상점 열림 (튜토리얼 진행)");
            isWaitingForShopOpen = false;
            NextStep();
        }
    }

    private void CompleteTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 완료 처리");
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (GameManager.Instance != null) 
        {   
            GameManager.Instance.CompleteTutorial(); // GameManager에 완료 알림
        }
        currentStep = TutorialStep.None;
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