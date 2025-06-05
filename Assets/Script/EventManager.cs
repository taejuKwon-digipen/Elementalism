using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject buttonPrefab;
    public GameObject content; // Content ������ �ؽ�Ʈ

    [SerializeField] private CardItemSO cardDatabase;
    [SerializeField] private Inventory inventory;

    [Header("Inventory UI")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private TextMeshProUGUI InventoryText;
    [SerializeField] private Transform cardContainer;
    public GameObject cardPrefab;

    [Header("Batting UI")]
    [SerializeField] private GameObject BattingPanel;
    [SerializeField] private GameObject Plus_button;
    [SerializeField] private GameObject Minus_button;
    [SerializeField] private GameObject Gold_panel;
    [SerializeField] private GameObject Batting_button;

    [Header("Result UI")]
    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private GameObject World_button;
    [SerializeField] private GameObject ResultPrefab; //Result text
    //[SerializeField] private GameObject ResultObject; //Result Object

    //private int RandomGold = 0;
    //string valueForTemplate;
    //string result_text;
    void Start()
    {
        inventory = Resources.Load<Inventory>("Inventory");
        if (inventory == null)
        {
            Debug.LogError("[InventoryManager] Inventory를 찾을 수 없습니다!");
            return;
        }
        if (GoogleSheetLoader.Instance != null)
        {
            GoogleSheetLoader.Instance.OnSheetLoaded -= OnSheetLoadedHandler;
            GoogleSheetLoader.Instance.OnSheetLoaded += OnSheetLoadedHandler;

            if (GoogleSheetLoader.Instance.IsLoaded)
            {
                OnSheetLoadedHandler();
            }
        }
        BattingPanel.SetActive(false);
        //DeleteCards();
    }

    void OnSheetLoadedHandler()
    {
        var eventDatas = GoogleSheetLoader.Instance.eventDatas;
        if (eventDatas.Count == 0)
        {
            Debug.LogError("이벤트 데이터가 없습니다!");
            return;
        }

        int minEventId = eventDatas.Keys.Min();
        int maxEventId = eventDatas.Keys.Max();

        // 랜덤으로 이벤트 ID 선택
        int[] allIds = eventDatas.Keys.ToArray();
        int currentEventId = allIds[UnityEngine.Random.Range(0, allIds.Length)];
        currentEventId = 1; // 테스트용으로 2번 이벤트로 고정
        Debug.Log($"[EventManager] 이벤트 ID 범위: {minEventId} ~ {maxEventId}, 랜덤 선택: {currentEventId}");
        ShowEvent(currentEventId);
    }

    private void Update()
    {
        Debug.Log("플레이어 체력: " + GameManager.Instance.Player_HP);
        Debug.Log("플레이어 골드: " + GameManager.Instance.Player_Gold);
        //Debug.Log("플레이어 최대체력: " + GameManager.Instance.Player_MaxHP);
    }
    void ShowEvent(int eventId)
    {
        Debug.Log("이벤트 표시: " + eventId);
        if (!GoogleSheetLoader.Instance.eventDatas.TryGetValue(eventId, out var data))
        {
            Debug.LogError("이벤트 데이터 없음: " + eventId);
            return;
        }
        GameObject btnObj = Instantiate(content, canvas.transform);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = data.content;
        RectTransform rectTransform = content.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -15);

        // 버튼1
        CreateButton(data.choice1_text, data.choice1_effect, new Vector2(0, -310));
        // 버튼2
        CreateButton(data.choice2_text, data.choice2_effect, new Vector2(0, -450));
    }

    void CreateButton(string text, string effect, Vector2 pos)
    {
        GameObject btnObj = Instantiate(buttonPrefab, canvas.transform);
        //btnObj.tag = "Select_Button";
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btnObj.GetComponent<RectTransform>().anchoredPosition = pos;
        btnObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            //result_text = result;
            OnChoice(effect);
        });
    }

    List<string> resultTexts = new List<string>();
    void OnChoice(string effect)
    {
        // 효과 파싱 및 실행
        var effects = effect.Split(',');
        System.Random rand = new System.Random();
        bool needWaitForCard = false;
        bool needWaitForBatting = false;
        string oddEvenEffect = null;

        foreach (var e in effects)
        {
            int changeValue = 0;
            if (e.StartsWith("HP-"))
            {
                int value = int.Parse(e.Substring(3));
                GameManager.Instance.Player_HP -= value;
                changeValue = -value;
                resultTexts.Add($"{value}만큼 체력을 잃었습니다");
            }
            else if (e.StartsWith("HP+"))
            {
                int value = int.Parse(e.Substring(3));
                GameManager.Instance.Player_HP += value;
                changeValue = value;
                resultTexts.Add($"{value}만큼 체력을 회복했습니다");
            }
            else if (e.StartsWith("Gold+"))
            {
                string value = e.Substring(5).Trim();
                
                // 1. "숫자~숫자" 형태인지 확인
                if (value.Contains("~"))
                {
                    var parts = value.Split('~');
                    int min = int.Parse(parts[0]);
                    int max = int.Parse(parts[1]);

                    // 10단위 랜덤 값 생성
                    int count = (max - min) / 10 + 1;
                    int gold = min + rand.Next(0, count) * 10;
                    GameManager.Instance.Player_Gold += gold;
                    changeValue = gold;
                    resultTexts.Add($"{gold}골드를 획득하였습니다");
                    //valueForTemplate = gold.ToString();
                    Debug.Log(min + " ~ " + max + " 사이의 랜덤 골드: " + gold);
                }
                // 2. 일반 숫자 처리
                else
                {
                    resultTexts.Add($"{value}골드를 획득하였습니다");
                    GameManager.Instance.Player_Gold += int.Parse(value);
                    //valueForTemplate = int.Parse(value).ToString();
                }
            }
            else if (e.StartsWith("Gold-"))
            {
                int value = int.Parse(e.Substring(5));
                GameManager.Instance.Player_Gold -= value;
                changeValue = -value;
                resultTexts.Add($"{value}골드를 잃었습니다");
            }
            else if (e.StartsWith("MaxHP+"))
            {
                int value = int.Parse(e.Substring(6));
                GameManager.Instance.Player_MaxHP += value;
                changeValue = value;
                resultTexts.Add($"최대체력이{value}증가하였습니다");
                //valueForTemplate = int.Parse(e.Substring(6)).ToString();
            }
            else if (e.StartsWith("MaxHP-"))
            {
                int value = int.Parse(e.Substring(6));
                GameManager.Instance.Player_MaxHP -= value;
                changeValue = value;
                resultTexts.Add($"최대체력이{value}증가하였습니다");
                //valueForTemplate = int.Parse(e.Substring(6)).ToString();
            }
            else if (e.StartsWith("R_Card+"))
            {
                AddrandomCard();
            }
            else if (e.StartsWith("R_Card-"))
            {
                DeleterandomCard();
            }
            else if (e.StartsWith("S_Card-"))
            {
                // 카드 추가 후 씬 이동 콜백 전달
                AddCards(() => ShowResultAndWait(() => SceneManager.LoadScene("Map2")));
                needWaitForCard = true;
            }
            else if (e.StartsWith("S_Card-"))
            {
                // 카드 삭제 후 씬 이동 콜백 전달
                DeleteCards(() => ShowResultAndWait(() => SceneManager.LoadScene("Map2")));
                needWaitForCard = true;
            }
            else if (e.StartsWith("Random_Card"))
            {
                float randomValue = UnityEngine.Random.value; // 0.0 ~ 1.0 사이의 float
                if (randomValue < 0.5f)
                {
                    Debug.Log("50% 확률로 랜덤카드 추가!");
                    //result_text = "?카드가 추가되었습니다";
                    AddrandomCard();
                }
                else
                {
                    Debug.Log("50% 확률로 랜덤카드 삭제!");
                    //result_text = "?카드가 삭제되었습니다";
                    DeleterandomCard();
                }
            }
            else if (e.StartsWith("홀수") || e.StartsWith("짝수"))
            {
                oddEvenEffect = e;
                needWaitForBatting = true;
            }
            else if (e.StartsWith("Random_50_"))
            {
                string[] options = e.Substring("Random_50_".Length).Split('|');
                if (options.Length == 2)
                {
                    float randomValue = UnityEngine.Random.value;
                    string selectedEffect = randomValue < 0.5f ? options[0] : options[1];
                    Debug.Log(options[0]);
                    OnChoice(selectedEffect); // 재귀 호출로 효과 처리
                    return; // 중복 Scene 이동 방지
                }
            }
            // 추가 효과 구현 가능
        }


        if (needWaitForBatting)
        {
            Batting((betGold, randomNum) =>
            {
                Debug.Log($"배팅: {betGold}, 랜덤숫자: {randomNum}");

                bool isOdd = randomNum % 2 == 1;
                bool isSuccess = (oddEvenEffect == "홀수" && isOdd) || (oddEvenEffect == "짝수" && !isOdd);

                if (isSuccess)
                {
                    // 성공: 배팅금의 2배 지급 (예시)
                    GameManager.Instance.Player_Gold += betGold * 2;
                    resultTexts.Add($"{betGold * 2}골드를 획득하였습니다");
                    Debug.Log("성공! 골드 2배 획득");
                }
                else
                {
                    // 실패: 배팅금 차감
                    GameManager.Instance.Player_Gold -= betGold;
                    resultTexts.Add($"{betGold}골드를 잃었습니다");
                    Debug.Log("실패! 배팅금 차감");
                }

                ShowResultAndWait(() => SceneManager.LoadScene("Map2"));
            });
            return;
        }

        if (resultTexts.Count == 0)
        {
            SceneManager.LoadScene("Map2");
            return;
        }

        if (!needWaitForCard)
        {
            ShowResultAndWait(() => SceneManager.LoadScene("Map2"));
        }
    }

    void AddrandomCard()
    {
        var unlockedCards = cardDatabase.items.Where(card => card.IsUnlocked).ToList();
        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[EventManager] 언락된 카드가 없습니다!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, unlockedCards.Count);
        CardItem cardItem = unlockedCards[randomIndex];
        //valueForTemplate = cardItem.CardName;
        resultTexts.Add($"{cardItem.CardName}카드를 획득하였습니다");
        inventory.AddCard(cardItem);
    }
    void DeleterandomCard()
    {
        var unlockedCards = inventory.unlockedCards;
        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[EventManager] 언락된 카드가 없습니다!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, unlockedCards.Count);
        CardItem cardItem = unlockedCards[randomIndex];
        resultTexts.Add($"{cardItem.CardName}카드가 삭제되었습니다");
        //valueForTemplate = cardItem.CardName;
        inventory.RemoveCard(cardItem);
    }
    void AddCards(Action onCardAdd)
    {
        InventoryPanel.transform.SetAsLastSibling();
        InventoryPanel.SetActive(true);
        InventoryText.text = "추가할 카드를 선택하세요!";
        var unlockedCards = inventory.unlockedCards;

        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[InventoryManager] 언락된 카드가 없습니다!");
            return;
        }

        foreach (var cardItem in unlockedCards)
        {
            var cardObject = Instantiate(cardPrefab, cardContainer);
            var card = cardObject.GetComponent<Card>();

            card.Setup(cardItem, true);
            card.enabled = false;
            var button = cardObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Debug.Log(cardItem.CardName + " 카드 클릭됨");
                resultTexts.Add($"{cardItem.CardName}카드를 획득하였습니다");
                inventory.AddCard(cardItem);
                InventoryPanel.SetActive(false);

                // 카드 추가 후 콜백 실행
                onCardAdd?.Invoke();
            });
        }
    }
    void DeleteCards(Action onCardDeleted)
    {
        InventoryPanel.transform.SetAsLastSibling();
        InventoryPanel.SetActive(true);
        InventoryText.text = "삭제할 카드를 선택하세요!";
        var unlockedCards = inventory.unlockedCards;

        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[InventoryManager] 언락된 카드가 없습니다!");
            return;
        }

        foreach (var cardItem in unlockedCards)
        {
            var cardObject = Instantiate(cardPrefab, cardContainer);
            var card = cardObject.GetComponent<Card>();

            card.Setup(cardItem, true);
            card.enabled = false;
            var button = cardObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Debug.Log(cardItem.CardName + " 카드 클릭됨");
                //valueForTemplate = cardItem.CardName;
                resultTexts.Add($"{cardItem.CardName}카드가 삭제되었습니다");
                inventory.RemoveCard(cardItem);
                InventoryPanel.SetActive(false);

                // 카드 삭제 후 콜백 실행
                onCardDeleted?.Invoke();
            });
        }
    }

    void Batting(Action<int, int> onBatting)
    {
        BattingPanel.transform.SetAsLastSibling();
        BattingPanel.SetActive(true);
        int gold = 0;
        Gold_panel.GetComponentInChildren<TextMeshProUGUI>().text = gold.ToString();
        Plus_button.GetComponent<Button>().onClick.AddListener(() =>
        {
            gold += 10;
            if (gold >= GameManager.Instance.Player_Gold)
            {
                gold = GameManager.Instance.Player_Gold;
            }
            Gold_panel.GetComponentInChildren<TextMeshProUGUI>().text = gold.ToString();
        });
        Minus_button.GetComponent<Button>().onClick.AddListener(() =>
        {
            gold -= 10;
            if (gold <= 0)
            {
                gold = 0;
            }
            Gold_panel.GetComponentInChildren<TextMeshProUGUI>().text = gold.ToString();
        });

        Batting_button.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("배팅 금액: " + gold);
            BattingPanel.SetActive(false);

            // 배팅 후 랜덤 숫자 생성 (예: 1~10)
            int randomNum = UnityEngine.Random.Range(1, 7);

            // 콜백에 배팅 금액과 랜덤 숫자 전달
            onBatting?.Invoke(gold, randomNum);
        });
    }

    void ShowResultAndWait(Action onNext)
    {
        ResultPanel.transform.SetAsLastSibling();
        ResultPanel.SetActive(true);

        string finalResultText = string.Join("\n", resultTexts);
        //string finalText = FillTemplate(resultTemplate, finalResultText);
        //string finalText = FillTemplate(result_text, valueForTemplate);
        ResultPrefab.GetComponentInChildren<TextMeshProUGUI>().text = finalResultText;

        World_button.GetComponent<Button>().onClick.RemoveAllListeners();
        World_button.GetComponent<Button>().onClick.AddListener(() =>
        {
            ResultPanel.SetActive(false);
            onNext?.Invoke();
        });
    }

    //string FillTemplate(string template, string value)
    //{
    //    return template.Replace("?", value);
    //}
}