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

    [Header("UI References")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private Transform cardContainer;
    public GameObject cardPrefab;
    private List<Card> activeCards = new List<Card>();
    //private int currentEventId = 1; // ���� �̺�Ʈ id

    void Start()
    {
        inventory = Resources.Load<Inventory>("Inventory");
        if (inventory == null)
        {
            Debug.LogError("[InventoryManager] Inventory�� ã�� �� �����ϴ�!");
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
            Debug.LogError("�̺�Ʈ �����Ͱ� �����ϴ�!");
            return;
        }

        int minEventId = eventDatas.Keys.Min();
        int maxEventId = eventDatas.Keys.Max();

        // �������� �̺�Ʈ ID ����
        int[] allIds = eventDatas.Keys.ToArray();
        int currentEventId = allIds[UnityEngine.Random.Range(0, allIds.Length)];

        Debug.Log($"[EventManager] �̺�Ʈ ID ����: {minEventId} ~ {maxEventId}, ���� ����: {currentEventId}");
        ShowEvent(currentEventId);
    }

    private void Update()
    {
        Debug.Log("�÷��̾� ü��: "  + GameManager.Instance.Player_HP);
        Debug.Log("�÷��̾� ���: " + GameManager.Instance.Player_Gold); 
        Debug.Log("�÷��̾� �ִ�ü��: " + GameManager.Instance.Player_MaxHP);
    }
    void ShowEvent(int eventId)
    {
        Debug.Log("�̺�Ʈ ǥ��: " + eventId);
        if (!GoogleSheetLoader.Instance.eventDatas.TryGetValue(eventId, out var data))
        {
            Debug.LogError("�̺�Ʈ ������ ����: " + eventId);
            return;
        }
        GameObject btnObj = Instantiate(content, canvas.transform);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = data.content;
        RectTransform rectTransform = content.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -15);

        // ��ư1
        CreateButton(data.choice1_text, data.choice1_effect, new Vector2(0, -310));
        // ��ư2
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
        // ȿ�� �Ľ� �� ����
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

                // 1. "����~����" �������� Ȯ��
                if (value.Contains("~"))
                {
                    var parts = value.Split('~');
                    int min = int.Parse(parts[0]);
                    int max = int.Parse(parts[1]);

                    // 10���� ���� �� ����
                    int count = (max - min) / 10 + 1;
                    int gold = min + rand.Next(0, count) * 10;
                    GameManager.Instance.Player_Gold += gold;
                    Debug.Log(min + " ~ " + max + " ������ ���� ���: " + gold);
                }
                // 2. �Ϲ� ���� ó��
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
                // ī�� �߰� �� �� �̵� �ݹ� ����
                AddCards(() => SceneManager.LoadScene("Map2"));
                needWaitForCard = true;
            }
            else if (e.StartsWith("S_Card-"))
            {
                // ī�� ���� �� �� �̵� �ݹ� ����
                DeleteCards(() => SceneManager.LoadScene("Map2"));
                needWaitForCard = true;
            }
            else if (e.StartsWith("Random_Card"))
            {
                float randomValue = UnityEngine.Random.value; // 0.0 ~ 1.0 ������ float
                if (randomValue < 0.5f)
                {
                    Debug.Log("50% Ȯ���� ����ī�� �߰�!");
                    AddrandomCard();
                }
                else
                {
                    Debug.Log("50% Ȯ���� ����ī�� ����!");
                    DeleterandomCard();
                }
            }
            // �߰� ȿ�� ���� ����
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
            Debug.LogWarning("[EventManager] ����� ī�尡 �����ϴ�!");
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
            Debug.LogWarning("[EventManager] ����� ī�尡 �����ϴ�!");
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
            Debug.LogWarning("[InventoryManager] ����� ī�尡 �����ϴ�!");
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
                Debug.Log(cardItem.CardName + " ī�� Ŭ����");
                inventory.AddCard(cardItem);
                InventoryPanel.SetActive(false);

                // ī�� �߰� �� �ݹ� ����
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
            Debug.LogWarning("[InventoryManager] ����� ī�尡 �����ϴ�!");
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
                Debug.Log(cardItem.CardName + " ī�� Ŭ����");
                inventory.RemoveCard(cardItem);
                InventoryPanel.SetActive(false);

                // ī�� ���� �� �ݹ� ����
                onCardDeleted?.Invoke();
            });
        }
    }
}