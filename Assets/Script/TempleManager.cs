using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections.Generic;

public class TempleManager : MonoBehaviour
{
    [SerializeField] public GameObject TemplePanel;
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Transform cardContainer;

    [SerializeField] private Inventory inventory;
    public GameObject cardPrefab;

    public static TempleManager Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        TemplePanel.SetActive(false);
        inventory = Resources.Load<Inventory>("Inventory");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenTemple()
    {
        TemplePanel.SetActive(true);
        Text.text = "강화할 카드를 선택하세요!";
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
                cardItem.CurrentLevel++;
                //Debug.Log(cardItem.CardName + " 카드 클릭됨");
                Debug.Log($"[TempleManager] 카드 강화됨: {cardItem.CardName} (ID: {cardItem.ID}, Level: {cardItem.CurrentLevel})");
                //inventory.AddCard(cardItem);
                CloseTemple();
            });
        }
    }

    void CloseTemple()
    {
        TemplePanel.SetActive(false);
        SceneManager.LoadScene("Map2");
    }
}
