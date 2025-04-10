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
    [SerializeField] private Button openButton;
    [SerializeField] private Button exitButton;

    [Header("References")]
    [SerializeField] private Inventory inventory;

    private List<Card> activeCards = new List<Card>(); // 현재 활성화된 카드 목록

    // Start is called before the first frame update
    void Start()
    {
        InventoryPanel.SetActive(false);
        inventory = Resources.Load<Inventory>("Inventory");
        openButton.onClick.AddListener(OpenInventory);
        exitButton.onClick.AddListener(CloseInventory);

        if (inventory == null)
        {
            Debug.LogError("[InventoryManager] Inventory를 찾을 수 없습니다!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    if (InventoryPanel.activeSelf)
        //    {
        //        //CloseInventory();
        //    }
        //    else
        //    {
        //        OpenInventory();
        //    }
        //}
    }

    void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        GenerateInventoryCards();
        openButton.interactable = false; // 인벤토리 열릴 때 버튼 비활성화
    }

    void CloseInventory()
    {
        InventoryPanel.SetActive(false);
        ClearInventoryCards();
        openButton.interactable = true; // 인벤토리 닫힐 때 버튼 활성화
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
            card.enabled = false; // 카드 컴포넌트 비활성화하여 OnPointerDown 이벤트 방지   
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