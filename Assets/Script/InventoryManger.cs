using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private Transform cardContainer;

    [Header("References")]
    [SerializeField] private Inventory inventory;

    private List<Card> activeCards = new List<Card>(); // 현재 활성화된 카드 목록

    // Start is called before the first frame update
    void Start()
    {
        InventoryPanel.SetActive(false);
        inventory = Resources.Load<Inventory>("Inventory");

        if (inventory == null)
        {
            Debug.LogError("[InventoryManager] Inventory를 찾을 수 없습니다!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (InventoryPanel.activeSelf)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        GenerateInventoryCards();
    }

    void CloseInventory()
    {
        InventoryPanel.SetActive(false);
        ClearInventoryCards();
    }

    void GenerateInventoryCards()
    {
        var unlockedCards = inventory.unlockedCards;

        if (unlockedCards.Count == 0)
        {
            Debug.LogWarning("[InventoryManager] 언락된 카드가 없습니다!");
            return;
        }

        foreach (var cardItem in unlockedCards)
        {
            var cardObject = Instantiate(CardManager.Inst.cardPrefab, cardContainer);
            var card = cardObject.GetComponent<Card>();

            card.Setup(cardItem, true);
            activeCards.Add(card); // 활성화된 카드 목록에 추가
        }
    }

    void ClearInventoryCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card.gameObject); // 카드 오브젝트 제거
        }
        activeCards.Clear(); // 리스트 초기화
    }
}