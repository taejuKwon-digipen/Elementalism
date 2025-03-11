using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using static TreeEditor.TreeEditorHelper;

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
    [SerializeField] public GameObject linePrefab; // 라인 프리펩
    public List<GameObject> lines = new();
    [SerializeField] public GameObject firstNodePO;

    public string SceneToLoad;
    public Transform Container; // 맵 노드가 들어갈 부모 오브젝트
    private static List<List<Node>> map = new(); //층별 노드리스트
    private Node currentNode; //현재 플레이어가 위치한 노드

    public static MapManager Instance;

    private static bool isInitialized = false; // 최초 실행 여부
    //private Vector2 CurrNodePosition;

    private int CurrNodeID;
    private void Awake()
    {
        Debug.Log("MapManager Awake 호출");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("MapManager 인스턴스 생성됨");
        }
        else
        {
            Debug.Log("MapManager 중복 생성됨 → 새 객체 삭제");
            Destroy(gameObject);
            return;
        }

        FindContainer();
    }

    private void FindContainer()
    {
        //Content 하위에서 NodemapContainer를 찾음
        GameObject containerObj = GameObject.Find("Content");
        if (containerObj != null)
        {
            Container = containerObj.transform;
            Debug.Log("NodemapContainer 할당 완료");
        }
        else
        {
            Debug.LogError("NodemapContainer를 찾을 수 없음!");
        }

        //Content 하위에서 LinemapContainer를 찾음
        GameObject lineContainerObj = GameObject.Find("Content");
        if (lineContainerObj != null)
        {
            Container = lineContainerObj.transform;
            Debug.Log("LinemapContainer 할당 완료");
        }
        else
        {
            Debug.LogError("LinemapContainer를 찾을 수 없음!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map2") // 맵 씬일 때 실행
        {
            if (map.Count == 0) // 맵이 없으면 다시 생성 (예외 처리)
            {
                Debug.LogWarning("맵 데이터 없음 → 새로 생성");
                GenerateMap();
                ConnectNodes();
                DrawMap();
                currentNode = map[0][0];
                currentNode.SetSelectable(true);
                MovePlayer(currentNode);
            }
            else
            {
                Debug.Log("맵 씬으로 돌아옴, 기존 맵 복원 중...");
                FindContainer();
                RestoreMapState();
                DrawMap();
            }
        }
    }

    private void RestoreMapState()
    {
        Debug.Log("RestoreMapState 실행: 기존 맵 복원 중...");
        Debug.Log($"Map Count: {map.Count}, nodePosition Count: {nodePositions.Count}");
        if (map == null || map.Count == 0)
        {
            Debug.LogWarning("기존 맵 데이터 없음 → 새로 생성");
            GenerateMap();
            ConnectNodes();
            DrawMap();
            return;
        }
        else
        {

            lines.Clear();
            int node_row = 0;
            int node_col = 0;
            foreach (var num in map)
            {
                Debug.Log($"Num Count : " + num.Count);
                for (int i = 0; i < num.Count; i++)
                {
                    Vector2 nodePos = nodePositions.Keys.ElementAt(node_col); // 저장된 위치 가져오기
                    GameObject newNodeObj = Instantiate(nodePrefab, Container);
                    Node newNode = newNodeObj.GetComponent<Node>();
                    newNode.SetPosition(nodePos);

                    NodeType nodetype = nodePositions.Values.ElementAt(node_col);
                    //Debug.Log("Node type : " + nodetype);
                    newNode.SetNodeType(nodetype);
                    map[node_row][i] = newNode;
                    newNode.SetNodeID(node_col);
                    node_col++;
                    
                    if (newNode.NodeID == currentNode.NodeID)
                    {
                        currentNode = newNode;
                    }
                }
                node_row++;
            }
            //Debug.Log($"node Count : " + node_col);
            //foreach (List<Node> col in map)
            //{
            //    foreach (Node node in col)
            //    {
            //        Debug.Log($"Map: {node.nodeType}, {node}");
            //    }
            //}
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    Debug.Log("map 저장 되어있나: " + map[i][j]);

                    map[i][j].gameObject.SetActive(true);
                    map[i][j].UpdateVisual();

                    if (map[i][j] == currentNode)
                    {
                        map[i][j].SetSelectable(false);
                    }
                    else if (currentNode.connectedNodes.Contains(map[i][j]))
                    {
                        map[i][j].SetSelectable(true);
                    }
                }
            }
            ConnectNodes();

            List<Node> ConnectedNodeList = currentNode.connectedNodes;

            for(int i = 0; i < ConnectedNodeList.Count; i++)
            {
                ConnectedNodeList[i].SetSelectable(true);
            }

            foreach (var line in lines)
            {
                if (line == null)
                {
                    Debug.LogWarning(" 라인이 사라짐 → 다시 생성");
                    DrawMap();
                    return;
                }

                line.SetActive(true);
            }
        }

    }


    private void Start()
    {
        Debug.Log($"MapManager 존재 확인: {this.gameObject.name}");
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

    private Dictionary<Vector2, NodeType> nodePositions = new(); // 노드 위치 저장용

    void GenerateMap()
    {
        Vector2 FirstNodePosition = firstNodePO.transform.position;
        List<Node> nodeInFirst = new();
        GameObject firstNode = Instantiate(nodePrefab, Container);
        Node Firstnode = firstNode.GetComponent<Node>();
        Firstnode.SetPosition(FirstNodePosition);
        Firstnode.SetNodeType(NodeType.Start);
        Firstnode.SetNodeID(0);
        nodeInFirst.Add(Firstnode);
        map.Add(nodeInFirst);

        Debug.Log("187line" + Firstnode);
        nodePositions.Add(FirstNodePosition, Firstnode.GetNodeType());
        Debug.Log($"[노드 저장] 위치: {FirstNodePosition}, 타입: {Firstnode.GetNodeType()}"); //  로그 추가
        // 노드의 위치를 Dictionary에 저장
        //nodePositions[FirstNodePosition] = Firstnode;

        int nodeID = 1;
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
                while (!IsPositionValid(newPos));

                GameObject nodeobj = Instantiate(nodePrefab, Container);
                Node node = nodeobj.GetComponent<Node>();
                node.SetPosition(newPos);
                node.AssignRandomType();
                node.SetNodeID(nodeID);
                nodeInCol.Add(node);
                nodeID++;
                nodePositions.Add(newPos, node.GetNodeType());
            }
            map.Add(nodeInCol);
        }

        // Boss Node 추가
        List<Node> nodeInEnd = new();
        GameObject BossNode = Instantiate(nodePrefab, Container);
        Node bossNode = BossNode.GetComponent<Node>();
        bossNode.SetNodeType(NodeType.Boss);
        Vector2 bossPos;
        do
        {
            int roll2 = Random.Range(250, 300);
            bossPos = new Vector2(FirstNodePosition.x + (row - 1) * roll2, FirstNodePosition.y);
        }
        while (!IsPositionValid(bossPos));

        bossNode.SetPosition(bossPos);
        bossNode.SetNodeID(nodeID);
        nodeInEnd.Add(bossNode);
        map.Add(nodeInEnd);

        // 보스 노드 위치도 저장
        nodePositions.Add(bossPos, bossNode.GetNodeType());
        Debug.Log($"[노드 저장] 위치: {bossPos}, 타입: {bossNode.GetNodeType()}"); //  로그 추가


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
        GameObject lineObj = Instantiate(linePrefab, Container);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        line.useWorldSpace = false; // World Space 사용 안 함 (부모 기준으로 움직이게)

        // 부모(Content) 기준으로 상대적인 좌표를 적용해야 함
        Vector3 localPosA = Container.InverseTransformPoint(nodeA.transform.position);
        Vector3 localPosB = Container.InverseTransformPoint(nodeB.transform.position);

        line.positionCount = 2;
        line.SetPosition(0, localPosA);
        line.SetPosition(1, localPosB);

        lines.Add(lineObj);
        lineObj.transform.SetSiblingIndex(0);
    }


    public void MovePlayer(Node selectedNode)
    {
       if(/*currentNode == null ||*/ currentNode.connectedNodes.Contains(selectedNode))
        {
            currentNode = selectedNode; // 플레이어의 현재 위치 업데이트
            //CurrNodePosition = currentNode.GetPosition();
            CurrNodeID = currentNode.NodeID;
            //CurrNodePosition.
            Debug.Log("현재 노드: " + currentNode.name);

            /*// 이전에 반짝이던 노드들은 정지
            foreach (Node node in map.SelectMany(nodes => nodes))
            {
                node.StopBlinking();
            }*/

            // 현재 노드의 연결된 노드들을 반짝이게 설정
            foreach (Node node in currentNode.connectedNodes)
            {
                node.UpdateVisual();
            }
        }else if (selectedNode == map[0][0])
        {
            //urrNodePosition = map[0][0].GetPosition();
            CurrNodeID = map[0][0].NodeID;
            selectedNode.UpdateVisual();
        }
    }
}
