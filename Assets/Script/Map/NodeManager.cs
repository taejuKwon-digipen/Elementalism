using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public Node node;
    private readonly Dictionary<NodeType, string> names = new() {
        {NodeType.Battle, "Battle"},
        {NodeType.Event, "Event"},
        {NodeType.Shop, "Shop"},
        {NodeType.Boss, "Boss"},
        {NodeType.Rest, "Rest"},
    };

    void Start()
    {
        var text = GetComponentInChildren<TMP_Text>();
        text.text = names[node.type];
    }
}
