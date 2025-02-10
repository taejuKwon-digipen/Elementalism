using UnityEngine;
using System.Collections;

public class MapCharacter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Node currentNode;
    private bool isMoving = false;

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
        transform.position = (Vector3)node.position;
    }

    public bool CanMoveTo(Node targetNode)
    {
        if (currentNode == null || isMoving) return false;
        
        // 현재 노드에서 연결된 노드인지 확인
        return currentNode.connectedNodes.Contains(targetNode);
    }

    public void MoveToNode(Node targetNode)
    {
        if (CanMoveTo(targetNode))
        {
            StartCoroutine(MoveCoroutine(targetNode));
        }
    }

    private IEnumerator MoveCoroutine(Node targetNode)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = (Vector3)targetNode.position;
        float journeyLength = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;

        while (transform.position != endPos)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            yield return null;
        }

        currentNode = targetNode;
        isMoving = false;
        
        // 노드 도착 시 이벤트 실행
        HandleNodeEvent();
    }

    private void HandleNodeEvent()
    {
        switch (currentNode.type)
        {
            case NodeType.Battle:
                Debug.Log("배틀 씬으로 전환");
                // SceneManager.LoadScene("BattleScene");
                break;
            case NodeType.Shop:
                Debug.Log("상점 씬으로 전환");
                // SceneManager.LoadScene("ShopScene");
                break;
            case NodeType.Event:
                Debug.Log("이벤트 실행");
                // 이벤트 처리
                break;
            case NodeType.Rest:
                Debug.Log("휴식 이벤트");
                // 휴식 처리
                break;
            case NodeType.Boss:
                Debug.Log("보스 씬으로 전환");
                // SceneManager.LoadScene("BossScene");
                break;
        }
    }
} 