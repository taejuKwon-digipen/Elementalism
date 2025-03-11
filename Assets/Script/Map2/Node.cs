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
    //    mapManager = MapManager.Instance; // 싱글턴으로 MapManager 참조

    //    if (!IsTypeAssigned)
    //    {
    //        //Debug.Log("노드 타입 랜덤 생성 했어요!");
    //        AssignRandomType(); // 노드 타입을 확률적으로 설정
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
            Debug.Log($"[Start 호출] {gameObject.name}: AssignRandomType() 실행({nodeType}) [Node ID] ({NodeID})");
        }
        else
        {
            Debug.Log($"[Start 호출] {gameObject.name}: 이미 타입 설정됨 ({nodeType}) [Node ID] ({NodeID})");
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
        if (MapManager.Instance != null)
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
        if (this.transform != null)
        {
            this.transform.DOKill(); // 기존 트윈 효과 정지
            this.transform.DOScale(Vector3.one * 1.2f, 0.5f) // 크기 1.2배 증가
                .SetLoops(-1, LoopType.Yoyo) // 무한 반복 (커졌다 줄어들기)
                .SetEase(Ease.InOutSine); // 부드러운 애니메이션
        }
    }

    public void StopBlinking()
    {
        isSelectable = false;
        if (this.transform != null)
        {
            this.transform.DOKill(); // 애니메이션 정지
            this.transform.localScale = Vector3.one; // 원래 크기로 복귀
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
        int roll = Random.Range(0, 100); // 0~99 사이 난수
        int cumulative = 0;

        if (MapManager.Instance == null)
        {
            Debug.Log("될거같냐?");
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
        //Debug.Log($"[AssignRandomType] 노드 타입 지정됨 → {nodeType}");

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
