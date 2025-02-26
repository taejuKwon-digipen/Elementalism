using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class MapManager : MonoBehaviour
{
    [SerializeField] public int col;
    [SerializeField] public int row;
    //col별 노드 갯수 범위
    [SerializeField] public int minNodePerCol;
    [SerializeField] public int maxNodePerCol;

    //100이 넘지않게 설정하기
    [SerializeField] public int battleChance;
    [SerializeField] public int eventChance;
    [SerializeField] public int shopChance;

    [SerializeField] public GameObject nodePrefab; // 노드 UI 프리펩
    [SerializeField] public Transform NodemapContainer; // 맵 노드가 들어갈 부모 오브젝트
    [SerializeField] public Transform LinemapContainer; // 라인 들어갈 부모 오브젝트
    [SerializeField] public GameObject linePrefab; // 라인 프리펩
    public List<GameObject> lines = new();

    [SerializeField] public GameObject firstNodePO;

    public string SceneToLoad;

    private List<List<Node>> map = new(); //층별 노드리스트
    private Node currentNode; //현재 플레이어가 위치한 노드

    void Start()
    {
        GenerateMap();
        ConnectNodes();
        DrawMap();

        map[0][0].SetSelectable(true);
        MovePlayer(map[0][0]);
    }

    // 노드 간 최소 거리 설정
    float minDistance = 150f; // 예시값 (너무 작은 값을 설정하면 노드들이 겹칠 수 있음)
    bool IsPositionValid(Vector2 newPos)
    {
        foreach (var col in map)
        {
            foreach (var node in col)
            {
                if (Vector2.Distance(node.GetPosition(), newPos) < minDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void GenerateMap()
    {
        Vector2 FirstNodePosition = firstNodePO.transform.position;
        List<Node> nodeInFirst = new();
        GameObject firstNode = Instantiate(nodePrefab, NodemapContainer);
        Node Firstnode = firstNode.GetComponent<Node>();
        Firstnode.SetPosition(FirstNodePosition);
        Firstnode.SetNodeType(NodeType.Start);
        nodeInFirst.Add(Firstnode);
        map.Add(nodeInFirst);

        for (int i = 1; i < row - 1; i++)
        {
            int nodeCount = Random.Range(minNodePerCol, maxNodePerCol);
            List<Node> nodeInCol = new();

            for (int j = 0; j < nodeCount; j++)
            {
                Vector2 newPos;
                do
                {
                    int roll = Random.Range(250, 300);
                    newPos = new Vector2(FirstNodePosition.x + i * roll, FirstNodePosition.y + j * roll - nodeCount * 100);
                }
                while (!IsPositionValid(newPos)); // 유효한 위치일 때까지 반복

                GameObject nodeobj = Instantiate(nodePrefab, NodemapContainer);
                Node node = nodeobj.GetComponent<Node>();
                node.SetPosition(newPos);
                nodeInCol.Add(node);
            }
            map.Add(nodeInCol);
        }

        // Boss Node 추가
        List<Node> nodeInEnd = new();
        GameObject BossNode = Instantiate(nodePrefab, NodemapContainer);
        Node bossNode = BossNode.GetComponent<Node>();
        bossNode.SetNodeType(NodeType.Boss);
        Vector2 bossPos;
        do
        {
            int roll2 = Random.Range(250, 300);
            bossPos = new Vector2(FirstNodePosition.x + (row - 1) * roll2, FirstNodePosition.y);
        }
        while (!IsPositionValid(bossPos)); // 유효한 위치일 때까지 반복

        bossNode.SetPosition(bossPos);
        nodeInEnd.Add(bossNode);
        map.Add(nodeInEnd);

        currentNode = map[0][0];
    }


    void ConnectNodes()
    {
        for (int i = 0; i < row - 1; i++)
        {
            List<Node> currentCol = map[i]; // 현재 층의 노드 리스트
            List<Node> nextCol = map[i + 1]; // 다음 층의 노드 리스트

            foreach (Node node in currentCol)
            {
                if (i == row - 2) // 마지막 층이면 모든 노드를 보스 노드에 연결
                {
                    foreach (Node bossNode in nextCol)
                    {
                        node.connectedNodes.Add(bossNode);
                    }
                }
                else if (i == 0) // 첫 번째 층이면 모든 노드를 시작 노드에 연결
                {
                    foreach (Node startNode in nextCol)
                    {
                        node.connectedNodes.Add(startNode);
                    }
                }
                else // 일반적인 경우, 각 노드를 다음 층의 2개 노드와 연결
                {
                    // 각 노드가 두 개의 노드와만 연결되도록 처리
                    List<Node> sortedNextCol = new List<Node>(nextCol);
                    sortedNextCol.Sort((a, b) => Vector3.Distance(node.transform.position, a.transform.position)
                                                .CompareTo(Vector3.Distance(node.transform.position, b.transform.position)));

                    for (int j = 0; j < Mathf.Min(2, sortedNextCol.Count); j++)
                    {
                        node.connectedNodes.Add(sortedNextCol[j]);
                    }
                }
            }
        }

        // 모든 층에서 연결이 잘 되었는지 확인하고, 연결되지 않은 노드들끼리도 연결 처리
        EnsureAllNodesConnected();
    }

    void EnsureAllNodesConnected()
    {
        // 모든 노드를 체크하여 연결되지 않은 노드가 있다면, 이를 해결
        for (int i = 0; i < row - 1; i++)
        {
            List<Node> currentCol = map[i]; // 현재 층의 노드 리스트
            List<Node> nextCol = map[i + 1]; // 다음 층의 노드 리스트

            foreach (Node node in currentCol)
            {
                // 연결된 노드가 없으면, 다음 층의 노드 중 하나와 연결
                if (node.connectedNodes.Count == 0)
                {
                    Node fallbackNode = nextCol[Random.Range(0, nextCol.Count)];
                    node.connectedNodes.Add(fallbackNode);
                }
            }

            foreach (Node nextNode in nextCol)
            {
                // 연결된 노드가 없으면, 이전 층의 노드 중 하나와 연결
                bool isConnected = false;
                foreach (Node node in map[i])
                {
                    if (node.connectedNodes.Contains(nextNode))
                    {
                        isConnected = true;
                        break;
                    }
                }

                if (!isConnected)
                {
                    Node fallbackNode = map[i][Random.Range(0, map[i].Count)];
                    nextNode.connectedNodes.Add(fallbackNode);
                }
            }
        }
    }



    void DrawMap()
    {
        foreach (List<Node> levelNodes in map)
        {
            foreach (Node node in levelNodes)
            {
                foreach (Node connectedNode in node.connectedNodes)
                {
                    CreateLineBetweenNodes(node, connectedNode);
                }
            }
        }
    }

    void CreateLineBetweenNodes(Node nodeA, Node nodeB)
    {
        GameObject lineObj = Instantiate(linePrefab, LinemapContainer);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        line.useWorldSpace = false; // World Space 사용 안 함 (부모 기준으로 움직이게)

        // 부모(Content) 기준으로 상대적인 좌표를 적용해야 함
        Vector3 localPosA = LinemapContainer.InverseTransformPoint(nodeA.transform.position);
        Vector3 localPosB = LinemapContainer.InverseTransformPoint(nodeB.transform.position);

        line.positionCount = 2;
        line.SetPosition(0, localPosA);
        line.SetPosition(1, localPosB);

        lines.Add(lineObj);
        lineObj.transform.SetSiblingIndex(0);
    }


    public void MovePlayer(Node selectedNode)
    {
       if(currentNode == null || currentNode.connectedNodes.Contains(selectedNode))
        {
            currentNode = selectedNode; // 플레이어의 현재 위치 업데이트
            Debug.Log("현재 노드: " + currentNode.name);

            // 이전에 반짝이던 노드들은 정지
            foreach (Node node in map.SelectMany(nodes => nodes))
            {
                node.StopBlinking();
            }

            // 현재 노드의 연결된 노드들을 반짝이게 설정
            foreach (Node node in currentNode.connectedNodes)
            {
                node.UpdateVisual();
            }
        }else if (selectedNode == map[0][0])
        {
            selectedNode.UpdateVisual();
        }
    }
}
