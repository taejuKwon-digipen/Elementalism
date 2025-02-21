using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    private List<List<Node>> map = new(); //층별 노드리스트
    private Node currentNode; //현재 플레이어가 위치한 노드

    void Start()
    {
        GenerateMap();
        ConnectNodes();
        DrawMap();
    }

    //노드 생성
    void GenerateMap()
    {
        //First Node 추가
        Vector2 FirstNodePosition = firstNodePO.transform.position;
        List<Node> nodeInFirst = new(); //현재 층의 노드 리스트
        GameObject firstNode = Instantiate(nodePrefab, NodemapContainer);
        Node Firstnode = firstNode.GetComponent<Node>();
        Firstnode.SetPosition(FirstNodePosition);
        Firstnode.SetNodeType(NodeType.Start); //노드 타입 변경
        nodeInFirst.Add(Firstnode);
        map.Add(nodeInFirst);

        for (int i = 1; i < row -1; i++ )
        {
            int nodeCount = Random.Range(minNodePerCol, maxNodePerCol);
            List<Node> nodeInCol = new(); //현재 층의 노드 리스트
            
            for(int j = 0; j < nodeCount; j++ )
            {
                int roll = Random.Range(200, 220);
                GameObject nodeobj = Instantiate(nodePrefab, NodemapContainer); //노드 프리펩 생성
                Node node = nodeobj.GetComponent<Node>(); //노드 컴포넌트 가져오기
                node.SetPosition(new Vector2(FirstNodePosition.x + i * roll, FirstNodePosition.y + j * roll - nodeCount * 100)); //노드위치 배치
                nodeInCol.Add(node);// 리스트에 추가
            }
            map.Add(nodeInCol);//전체 맵 리스트에 추가
        }
        int roll2 = Random.Range(220,250);
        //Boss Node 추가
        List<Node> nodeInEnd = new(); //현재 층의 노드 리스트
        GameObject BossNode = Instantiate(nodePrefab, NodemapContainer);
        Node bossNode = BossNode.GetComponent<Node>();
        bossNode.SetNodeType(NodeType.Boss);
        bossNode.SetPosition(new Vector2(FirstNodePosition.x + (row-2) * roll2, FirstNodePosition.y) );
        nodeInEnd.Add(bossNode);
        map.Add(nodeInEnd);

        currentNode = map[0][0];
    }

    //노드 연결
    void ConnectNodes()
    {
        // 가까운 노드 2개만 선택하는 방식으로 변경
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

                }else if( i == 0)
                {
                    foreach(Node startNode in nextCol)
                    {
                        node.connectedNodes.Add(startNode);
                    }
                }
                else // 일반적인 경우, 가까운 2개 노드만 선택
                {
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
        GameObject lineObj = Instantiate(linePrefab, LinemapContainer); // 라인 프리팹 생성
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, nodeA.transform.position);
        line.SetPosition(1, nodeB.transform.position);

        lines.Add(lineObj); // 리스트에 추가하여 나중에 활성화/비활성화 관리
    }

    public void MovePlayer(Node selectedNode)
    {
       /* if (currentNode == null || currentNode.connectedNodes.Contains(selectedNode))
        { // 이동 가능한 노드인지 확인
            currentNode = selectedNode; // 플레이어의 현재 위치 업데이트
            // 선택한 노드에 대한 이벤트 실행 (전투, 상점 등)
        }*/
    }
}
