using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Battle,
    Shop,
    Event,
    Start,
    Boss
}

public class Node : MonoBehaviour
{
    public NodeType nodeType;
    public List<Node> connectedNodes = new();//����� ��� ����Ʈ
    private Vector2 position; //��� ��ġ
    private bool isSelectable = false;

    private void Start()
    {
        AssignRandomType(); // ��� Ÿ���� Ȯ�������� ����
    }

    void AssignRandomType()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();// �� �Ŵ��� ã��

        int roll = Random.Range(0, 100); // 0~99 ������ ���� ����
        int cumulative = 0;

        if (roll < (cumulative += mapManager.battleChance)) nodeType = NodeType.Battle;
        else if (roll < (cumulative += mapManager.eventChance)) nodeType = NodeType.Event;
        else if (roll < (cumulative += mapManager.shopChance)) nodeType = NodeType.Shop;
    }

    public void SetPosition(Vector2 newPosition)
    {
        position = newPosition;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    // ��� ���� ���� ���� ����
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
    }

    // �÷��̾ ��带 �������� �� ����Ǵ� �Լ�
    public void OnNodeSelected()
    {
        if (!isSelectable)
        {
            Debug.Log("�� ���� ������ �� �����ϴ�.");
            return;
        }

        Debug.Log("Selected Node: " + nodeType);
        MapManager Mapmanager = FindObjectOfType<MapManager>();
        if (Mapmanager != null)
        {
            Mapmanager.MovePlayer(this);
        }

        // ������ ����� ����� ��常 Ȱ��ȭ
        foreach (Node node in FindObjectsOfType<Node>())
        {
            node.SetSelectable(false); // ��� ��� ��Ȱ��ȭ
        }
        foreach (Node connectedNode in connectedNodes)
        {
            connectedNode.SetSelectable(true); // ����� ��常 Ȱ��ȭ
        }
    }
}
