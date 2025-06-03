using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
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
    private Image nodeImage;
    private TempleManager templeManager;
    public int NodeID { get; private set; }

    public void SetNodeID(int id)
    {
        NodeID = id;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapManager = MapManager.Instance;
        templeManager = TempleManager.Instance;
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

        // Battle 또는 Boss 노드일 경우, 다음 전투를 위한 Shape을 MapStorage에 설정 요청
        if (nodeType == NodeType.Battle || nodeType == NodeType.Boss || nodeType == NodeType.Start)
        {
            if (MapStorage.Instance != null)
            {
                if (nodeType == NodeType.Start)
                {
                    // "Start Battle"이라는 이름으로 Shape을 설정하도록 MapStorage에 요청
                    // MapStorage.cs에 PrepareBattleConfiguration(string configName)와 같은 메서드가 필요합니다.
                    MapStorage.Instance.PrepareBattleConfiguration("Start Battle"); 
                    Debug.Log($"[Node] {nodeType} 시작: MapStorage에 'Start Battle' 설정을 요청했습니다.");
                }
                else
                {
                    MapStorage.Instance.PrepareRandomBattleConfiguration(); // SetShapesForNextBattle(null) 대신 PrepareRandomBattleConfiguration 호출
                    Debug.Log($"[Node] {nodeType} 전투 준비: MapStorage에 무작위 전투 구성을 요청했습니다.");
                }
            }
            else
            {
                Debug.LogError("[Node] MapStorage.Instance가 null입니다.");
            }
        }

        string SceneToLoad = GetSceneNameByNodeType(nodeType);
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StopAllCoroutines();
            Debug.Log($"�� {SceneToLoad} �ε� ��...");
            if (SceneToLoad == "Shop")
            {
                templeManager.OpenTemple();
            }
            else
            {
                SceneManager.LoadScene(SceneToLoad);
            }
        }

        // ������ ��带 �� �Ŵ����� �ݿ�
        if (MapManager.Instance != null)
        {
            Debug.Log("��������Ʈ������ �׷���");
            /*nodeImage = GetComponent<Image>();
            nodeImage.color*/
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
            case NodeType.Shop: return "Shop";
            case NodeType.Event: return "Event";
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
