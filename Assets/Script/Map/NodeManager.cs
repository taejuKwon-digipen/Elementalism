using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nodeText;  // Inspector에서 할당할 텍스트 컴포넌트
    public Node node { get; set; }

    private static MapCharacter mapCharacter;
    private static bool isInitialized = false;

    private readonly Dictionary<NodeType, string> names = new() {
        {NodeType.Battle, "Battle"},
        {NodeType.Event, "Event"},
        {NodeType.Shop, "Shop"},
        {NodeType.Boss, "Boss"},
        {NodeType.Rest, "Rest"},
    };

    void Start()
    {
        if (!isInitialized)
        {
            mapCharacter = FindObjectOfType<MapCharacter>();
            isInitialized = true;
        }

        if (node == null)
        {
            Debug.LogWarning("Node is not assigned!");
            return;
        }

        if (nodeText == null)
        {
            nodeText = GetComponentInChildren<TMP_Text>();
            if (nodeText == null)
            {
                Debug.LogError("No TMP_Text component found!");
                return;
            }
        }

        UpdateNodeDisplay();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mapCharacter != null && mapCharacter.CanMoveTo(node))
        {
            mapCharacter.MoveToNode(node);
        }
    }

    private void UpdateNodeDisplay()
    {
        if (node != null && names.ContainsKey(node.type))
        {
            nodeText.text = names[node.type];
        }
    }
}
