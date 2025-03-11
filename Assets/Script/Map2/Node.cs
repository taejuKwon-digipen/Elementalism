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

    private bool IsTypeAssigned = false;
    private SpriteRenderer spriteRenderer;
    private MapManager mapManager;

    public int NodeID { get; private set; }

    public void SetNodeID(int id)
    {
        NodeID = id;
    }
    //private void Start()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    mapManager = MapManager.Instance; // �̱������� MapManager ����

    //    if (!IsTypeAssigned)
    //    {
    //        //Debug.Log("��� Ÿ�� ���� ���� �߾��!");
    //        AssignRandomType(); // ��� Ÿ���� Ȯ�������� ����
    //    }

    //    UpdateVisual();
    //}

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapManager = MapManager.Instance;

        if (!IsTypeAssigned)
        {
            //AssignRandomType();
            Debug.Log($"[Start ȣ��] {gameObject.name}: AssignRandomType() ����({nodeType}) [Node ID] ({NodeID})");
        }
        else
        {
            Debug.Log($"[Start ȣ��] {gameObject.name}: �̹� Ÿ�� ������ ({nodeType}) [Node ID] ({NodeID})");
        }

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!isSelectable)
        {
            Debug.Log("�� ���� ������ �� �����ϴ�.");
            return;
        }

        Debug.Log($"[Ŭ��] {nodeType} ��� ���õ�");

        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
        }

        string SceneToLoad = GetSceneNameByNodeType(nodeType);
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StopAllCoroutines();
            Debug.Log($"�� {SceneToLoad} �ε� ��...");
            SceneManager.LoadScene(SceneToLoad);
        }

        // ������ ��带 �� �Ŵ����� �ݿ�
        if (MapManager.Instance != null)
        {
            mapManager.MovePlayer(this);
        }
        else
        {
            Debug.LogError("mapManager�� null�Դϴ�! MapManager.Instance�� ���������� �����Ǿ����� Ȯ���ϼ���.");
        }

        foreach (var node in connectedNodes)
        {
            node.SetSelectable(true);
        }

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
        if (this.transform != null)
        {
            this.transform.DOKill(); // ���� Ʈ�� ȿ�� ����
            this.transform.DOScale(Vector3.one * 1.2f, 0.5f) // ũ�� 1.2�� ����
                .SetLoops(-1, LoopType.Yoyo) // ���� �ݺ� (Ŀ���� �پ���)
                .SetEase(Ease.InOutSine); // �ε巯�� �ִϸ��̼�
        }
    }

    public void StopBlinking()
    {
        isSelectable = false;
        if (this.transform != null)
        {
            this.transform.DOKill(); // �ִϸ��̼� ����
            this.transform.localScale = Vector3.one; // ���� ũ��� ����
        }
    }
    private string GetSceneNameByNodeType(NodeType type)
    {
        switch (type)
        {
            case NodeType.Battle: return "Main";
            case NodeType.Start: return "Main";
            case NodeType.Shop: return "Main";
            case NodeType.Event: return "Main";
            case NodeType.Boss: return "Main";
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

    public void AssignRandomType()
    {
        int roll = Random.Range(0, 100); // 0~99 ���� ����
        int cumulative = 0;

        if (MapManager.Instance == null)
        {
            Debug.Log("�ɰŰ���?");
            return;
        }
        if (roll < (cumulative += MapManager.Instance.battleChance))
        {
            SetNodeType(NodeType.Battle);
        }
        else if (roll < (cumulative += MapManager.Instance.eventChance))
        {
            SetNodeType(NodeType.Event);
        }
        else if (roll < (cumulative += MapManager.Instance.shopChance))
        {
            SetNodeType(NodeType.Shop);
        }

        TypeTXT.text = nodeType.ToString();
        //Debug.Log($"[AssignRandomType] ��� Ÿ�� ������ �� {nodeType}");

    }

    public NodeType GetNodeType()
    {
        return this.nodeType;
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
