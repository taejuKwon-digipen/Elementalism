using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] TMP_Text TypeTXT;

    public NodeType nodeType;
    public List<Node> connectedNodes = new(); // ����� ��� ����Ʈ
    private Vector2 position; // ��� ��ġ
    private bool isSelectable = false;

    public bool IsTypeAssigned = false;
    private SpriteRenderer spriteRenderer;
    private MapManager mapManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapManager = FindObjectOfType<MapManager>(); // �� �Ŵ��� ã��

        if (!IsTypeAssigned)
        {
            AssignRandomType(); // ��� Ÿ���� Ȯ�������� ����
        }

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!isSelectable) return;

        Debug.Log($"[Ŭ��] {nodeType} ��� ���õ�");

        // DOTween �ִϸ��̼� ���� (�� ��ȯ ���� �ݵ�� ����)
        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
        }

        // �� �̵� ó��
        string SceneToLoad = GetSceneNameByNodeType(nodeType);
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StopAllCoroutines(); // DOTween �̿��� �ڷ�ƾ�� ����
            SceneManager.LoadScene(SceneToLoad);
        }

        // ������ ��带 �� �Ŵ����� �ݿ�
        if (mapManager != null)
        {
            mapManager.MovePlayer(this);
        }

        // ����� ��� Ȱ��ȭ
        foreach (var node in connectedNodes)
        {
            node.SetSelectable(true);
        }

        // ��� ���� �� ���� Ȱ��ȭ
        ShowConnectedLines();
    }


    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (isSelectable)
        {
            StartBlinking(); // ���� ������ �� ��¦�̴� ȿ��
        }
        else
        {
            StopBlinking();
        }
    }

    private void StartBlinking()
    {
        spriteRenderer.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        isSelectable = false;
        spriteRenderer.DOKill(); // DOTween �ִϸ��̼� ����
        spriteRenderer.color = new Color(1, 1, 1, 1); // ���� ���� ����
    }

    private string GetSceneNameByNodeType(NodeType type)
    {
        switch (type)
        {
            case NodeType.Battle: return "Main";
            case NodeType.Start: return "Main";
            // case NodeType.Shop: return "ShopScene";
            // case NodeType.Event: return "EventScene";
            // case NodeType.Boss: return "BossScene";
            default: return "";
        }
    }

    public Vector2 GetPosition()
    {
        return position = transform.position;
    }

    public void SetNodeType(NodeType type)
    {
        nodeType = type;
        IsTypeAssigned = true;
        TypeTXT.text = nodeType.ToString();
    }

    private void AssignRandomType()
    {
        int roll = Random.Range(0, 100); // 0~99 ���� ����
        int cumulative = 0;

        if (mapManager == null) return;

        if (roll < (cumulative += mapManager.battleChance))
        {
            nodeType = NodeType.Battle;
        }
        else if (roll < (cumulative += mapManager.eventChance))
        {
            nodeType = NodeType.Event;
        }
        else if (roll < (cumulative += mapManager.shopChance))
        {
            nodeType = NodeType.Shop;
        }

        TypeTXT.text = nodeType.ToString();
    }

    public void SetPosition(Vector2 newPosition)
    {
        position = newPosition;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    private void ShowConnectedLines()
    {
        if (mapManager == null) return;

        // ��� ���� ��Ȱ��ȭ
        foreach (GameObject line in mapManager.lines)
        {
            line.SetActive(false);
        }

        // ����� ������ ���� Ȱ��ȭ
        foreach (Node connectedNode in connectedNodes)
        {
            foreach (GameObject line in mapManager.lines)
            {
                LineRenderer lr = line.GetComponent<LineRenderer>();
                if ((lr.GetPosition(0) == transform.position && lr.GetPosition(1) == connectedNode.transform.position) ||
                    (lr.GetPosition(1) == transform.position && lr.GetPosition(0) == connectedNode.transform.position))
                {
                    line.SetActive(true);
                }
            }
            connectedNode.SetSelectable(true);
        }
    }
}
