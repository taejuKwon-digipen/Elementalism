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
    public GameObject content; // Content 영역의 텍스트

    [SerializeField] private CardItemSO cardDatabase;
    [SerializeField] private Inventory inventory;

    [Header("UI References")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private Transform cardContainer;
    public GameObject cardPrefab;
    private List<Card> activeCards = new List<Card>();
    //private int currentEventId = 1; // 시작 이벤트 id

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

        Debug.Log($"[EventManager] 이벤트 ID 범위: {minEventId} ~ {maxEventId}, 랜덤 선택: {currentEventId}");
        ShowEvent(currentEventId);
    }

    private void Update()
    {
        Debug.Log("플레이어 체력: "  + GameManager.Instance.Player_HP);
        Debug.Log("플레이어 골드: " + GameManager.Instance.Player_Gold); 
        Debug.Log("플레이어 최대체력: " + GameManager.Instance.Player_MaxHP);
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
        btnObj.GetComponent<Button>().onClick.AddListener(() => OnChoice(effect));
    }

    void OnChoice(string effect)
    {
        // 효과 파싱 및 실행
        var effects = effect.Split(',');
        System.Random rand = new System.Random();
        bool needWaitForCard = false;

        foreach (var e in effects)
        {
            if (e.StartsWith("HP-")) GameManager.Instance.Player_HP -= int.Parse(e.Substring(3));
            else if (e.StartsWith("HP+")) GameManager.Instance.Player_HP += int.Parse(e.Substring(3));
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
                    Debug.Log(min + " ~ " + max + " 사이의 랜덤 골드: " + gold);
                }
                // 2. 일반 숫자 처리
                else
                {
                    GameManager.Instance.Player_Gold += int.Parse(value);
                }
            }
            else if (e.StartsWith("Gold-")) GameManager.Instance.Player_Gold -= int.Parse(e.Substring(5));
            else if (e.StartsWith("MaxHP+")) GameManager.Instance.Player_MaxHP += int.Parse(e.Substring(6));
            else if (e.StartsWith("MaxHP-")) GameManager.Instance.Player_MaxHP -= int.Parse(e.Substring(6));
            else if (e.StartsWith("R_Card+"))
            {
                AddrandomCard();
            }
            else if (e.StartsWith("R_Card-"))
            {
                AddrandomCard();
            }
            else if (e.StartsWith("S_Card-"))
            {
                // 카드 추가 후 씬 이동 콜백 전달
                AddCards(() => SceneManager.LoadScene("Map2"));
                needWaitForCard = true;
            }
            else if (e.StartsWith("S_Card-"))
            {
                // 카드 삭제 후 씬 이동 콜백 전달
                DeleteCards(() => SceneManager.LoadScene("Map2"));
                needWaitForCard = true;
            }
            else if (e.StartsWith("Random_Card"))
            {
                float randomValue = UnityEngine.Random.value; // 0.0 ~ 1.0 사이의 float
                if (randomValue < 0.5f)
                {
                    Debug.Log("50% 확률로 랜덤카드 추가!");
                    AddrandomCard();
                }
                else
                {
                    Debug.Log("50% 확률로 랜덤카드 삭제!");
                    DeleterandomCard();
                }
            }
            // 추가 효과 구현 가능
        }

        if (!needWaitForCard)
        {
            SceneManager.LoadScene("Map2");
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
        inventory.RemoveCard(cardItem);
    }
    void AddCards(Action onCardAdd)
    {
        InventoryPanel.SetActive(true);
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
            activeCards.Add(card);
            var button = cardObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Debug.Log(cardItem.CardName + " 카드 클릭됨");
                inventory.AddCard(cardItem);
                InventoryPanel.SetActive(false);

                // 카드 추가 후 콜백 실행
                onCardAdd?.Invoke();
            });
        }
    }
    void DeleteCards(Action onCardDeleted)
    {
        InventoryPanel.SetActive(true);
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
            activeCards.Add(card);
            var button = cardObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Debug.Log(cardItem.CardName + " 카드 클릭됨");
                inventory.RemoveCard(cardItem);
                InventoryPanel.SetActive(false);

                // 카드 삭제 후 콜백 실행
                onCardDeleted?.Invoke();
            });
        }
    }
}