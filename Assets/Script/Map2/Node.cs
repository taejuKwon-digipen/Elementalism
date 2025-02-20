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
    public List<Node> connectedNodes = new();//연결된 노드 리스트
    private Vector2 position; //노드 위치
    private bool isSelectable = false;

    private void Start()
    {
        AssignRandomType(); // 노드 타입을 확률적으로 설정
    }

    void AssignRandomType()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();// 맵 매니저 찾기

        int roll = Random.Range(0, 100); // 0~99 사이의 난수 생성
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

    // 노드 선택 가능 여부 설정
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
    }

    // 플레이어가 노드를 선택했을 때 실행되는 함수
    public void OnNodeSelected()
    {
        if (!isSelectable)
        {
            Debug.Log("이 노드는 선택할 수 없습니다.");
            return;
        }

        Debug.Log("Selected Node: " + nodeType);
        MapManager Mapmanager = FindObjectOfType<MapManager>();
        if (Mapmanager != null)
        {
            Mapmanager.MovePlayer(this);
        }

        // 선택한 노드의 연결된 노드만 활성화
        foreach (Node node in FindObjectsOfType<Node>())
        {
            node.SetSelectable(false); // 모든 노드 비활성화
        }
        foreach (Node connectedNode in connectedNodes)
        {
            connectedNode.SetSelectable(true); // 연결된 노드만 활성화
        }
    }
}
