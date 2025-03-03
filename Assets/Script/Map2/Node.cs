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
    public List<Node> connectedNodes = new(); // 연결된 노드 리스트
    private Vector2 position; // 노드 위치
    private bool isSelectable = false;

    public bool IsTypeAssigned = false;
    private SpriteRenderer spriteRenderer;
    private MapManager mapManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapManager = MapManager.Instance; // 싱글턴으로 MapManager 참조

        if (!IsTypeAssigned)
        {
            AssignRandomType(); // 노드 타입을 확률적으로 설정
        }

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!isSelectable)
        {
            Debug.Log("이 노드는 선택할 수 없습니다.");
            return;
        }

        Debug.Log($"[클릭] {nodeType} 노드 선택됨");

        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
        }

        string SceneToLoad = GetSceneNameByNodeType(nodeType);
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StopAllCoroutines();
            Debug.Log($"씬 {SceneToLoad} 로드 중...");
            SceneManager.LoadScene(SceneToLoad);
        }

        // 선택한 노드를 맵 매니저에 반영
        if (mapManager != null)
        {
            mapManager.MovePlayer(this);
        }
        else
        {
            Debug.LogError("mapManager가 null입니다! MapManager.Instance가 정상적으로 설정되었는지 확인하세요.");
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
            StartBlinking(); // 선택 가능할 때 반짝이는 효과
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
        spriteRenderer.DOKill(); // DOTween 애니메이션 정지
        //spriteRenderer.color = new Color(1, 1, 1, 1); // 원래 색상 복원
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

    private void AssignRandomType()
    {
        int roll = Random.Range(0, 100); // 0~99 사이 난수
        int cumulative = 0;

        if (mapManager == null) return;

        if (roll < (cumulative += mapManager.battleChance))
        {
            SetNodeType(NodeType.Battle);
        }
        else if (roll < (cumulative += mapManager.eventChance))
        {
            SetNodeType(NodeType.Event);
        }
        else if (roll < (cumulative += mapManager.shopChance))
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

        // 모든 라인 비활성화
        foreach (GameObject line in mapManager.lines)
        {
            line.SetActive(false);
        }

        // 연결된 노드들의 라인 활성화
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
