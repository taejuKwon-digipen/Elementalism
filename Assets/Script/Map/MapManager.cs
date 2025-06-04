using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using static TreeEditor.TreeEditorHelper;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using static UnityEditor.PlayerSettings;

public class MapManager : MonoBehaviour
{
    [SerializeField] public int col;
    [SerializeField] public int row;
    //col�� ��� ���� ����
    [SerializeField] public int minNodePerCol;
    [SerializeField] public int maxNodePerCol;

    //100% 넘지않게 설정하기
    [SerializeField] public int battleChance;
    [SerializeField] public int eventChance;
    [SerializeField] public int shopChance;

    [Header("Node Prefabs")]
    [SerializeField] public GameObject battleNodePrefab; // 배틀 노드 프리팹
    [SerializeField] public GameObject shopNodePrefab;   // 상점 노드 프리팹
    [SerializeField] public GameObject eventNodePrefab;  // 이벤트 노드 프리팹
    [SerializeField] public GameObject startNodePrefab;  // 시작 노드 프리팹
    [SerializeField] public GameObject bossNodePrefab;   // 보스 노드 프리팹
    [SerializeField] public GameObject defaultNodePrefab; // 기본 노드 프리팹 (대체용)
    
    [SerializeField] public GameObject linePrefab; // 선분 프리팹
    public List<GameObject> lines = new();
    [SerializeField] public GameObject firstNodePO;

    public string SceneToLoad;
    public Transform Container; // �� ��尡 �� �θ� ������Ʈ
    private static List<List<Node>> map = new(); //���� ��帮��Ʈ
    private Node currentNode; //���� �÷��̾ ��ġ�� ���

    public static MapManager Instance;

    private static bool isInitialized = false; // ���� ���� ����
    private List<int> nodeIdList = new();

    private int CurrNodeID;
    private void Awake()
    {
        Debug.Log("MapManager Awake ȣ��");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("MapManager �ν��Ͻ� ������");
        }
        else
        {
            Debug.Log("MapManager �ߺ� ������ �� �� ��ü ����");
            Destroy(gameObject);
            return;
        }

        FindContainer();
    }

    private void FindContainer()
    {
        //Content �������� NodemapContainer�� ã��
        GameObject containerObj = GameObject.Find("Content");
        if (containerObj != null)
        {
            Container = containerObj.transform;
            Debug.Log("NodemapContainer �Ҵ� �Ϸ�");
        }
        else
        {
            Debug.LogError("NodemapContainer�� ã�� �� ����!");
        }

        //Content �������� LinemapContainer�� ã��
        GameObject lineContainerObj = GameObject.Find("Content");
        if (lineContainerObj != null)
        {
            Container = lineContainerObj.transform;
            Debug.Log("LinemapContainer �Ҵ� �Ϸ�");
        }
        else
        {
            Debug.LogError("LinemapContainer�� ã�� �� ����!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map2") // �� ���� �� ����
        {
            if (map.Count == 0) // ���� ������ �ٽ� ���� (���� ó��)
            {
                Debug.LogWarning("�� ������ ���� �� ���� ����");
                GenerateMap();
                ConnectNodes();
                DrawMap();
                currentNode = map[0][0];
                currentNode.SetSelectable(true);
                MovePlayer(currentNode);
            }
            else
            {
                Debug.Log("�� ������ ���ƿ�, ���� �� ���� ��...");
                FindContainer();
                RestoreMapState();
                DrawMap();
            }
        }
    }

    private void Update()
    {
        
    }

    private void RestoreMapState()
    {
        Debug.Log("RestoreMapState ����: ���� �� ���� ��...");
        if (map == null || map.Count == 0)
        {
            Debug.LogWarning("���� �� ������ ���� �� ���� ����");
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
                for (int i = 0; i < num.Count; i++)
                {
                    Vector2 nodePos = nodePositions.Keys.ElementAt(node_col); // 노드의 위치 정보가져오기
                    NodeType nodetype = nodePositions.Values.ElementAt(node_col);
                    GameObject nodePrefab = GetPrefabForNodeType(nodetype); // 타입에 맞는 프리팹 선택
                    GameObject newNodeObj = Instantiate(nodePrefab, Container);
                    Node newNode = newNodeObj.GetComponent<Node>();
                    newNode.SetPosition(nodePos);

                    newNode.SetNodeType(nodetype);
                    map[node_row][i] = newNode;
                    newNode.SetNodeID(node_col);
                    node_col++;
                    
                    // 노드를 선들보다 위에 렌더링되도록 맨 뒤로 이동
                    newNodeObj.transform.SetAsLastSibling();

                    if (newNode.NodeID == currentNode.NodeID)
                    {
                        currentNode = newNode;
                        nodeIdList.Add(newNode.NodeID);
                    }

                    if (nodeIdList.Contains(newNode.NodeID))
                    {
                        Image image = newNode.GetComponentInChildren<Image>();
                        image.color = Color.gray;
                    }
                }
                node_row++;
            }
           
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
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
            DrawNewLine();
            
            // 모든 선들을 하이어라키 맨 위로 보내서 노드들보다 뒤에 렌더링
            foreach (var line in lines)
            {
                if (line != null)
                {
                    line.transform.SetAsFirstSibling();
                }
            }
        }
    }

    private void DrawNewLine()
    {
        List<Node> ConnectedNodeList = currentNode.connectedNodes;

        for (int i = 0; i < ConnectedNodeList.Count; i++)
        {
            ConnectedNodeList[i].SetSelectable(true);
        }

        foreach (var line in lines)
        {
            if (line == null)
            {
                Debug.LogWarning(" ������ ����� �� �ٽ� ����");
                DrawMap();
                return;
            }

            line.SetActive(true);
        }
    }


    private void Start()
    {
        //Debug.Log($"MapManager ���� Ȯ��: {this.gameObject.name}");
    }

    // ��� �� �ּ� �Ÿ� ����
    float minDistance = 150f; // ���ð� (�ʹ� ���� ���� �����ϸ� ������ ��ĥ �� ����)
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

    private Dictionary<Vector2, NodeType> nodePositions = new(); // ��� ��ġ �����

   
    private void MakePerNode(/*Vector2 pos,*/ NodeType type, int nodeid)
    {
        Vector2 pos = firstNodePO.transform.position;
        if (type == NodeType.Boss)
        {
            do
            {
                int roll2 = Random.Range(250, 300);
                pos = new Vector2(firstNodePO.transform.position.x + (row - 1) * roll2, firstNodePO.transform.position.y);
            }
            while (!IsPositionValid(pos));
        }
        List<Node> nodelist = new();
        GameObject nodePrefab = GetPrefabForNodeType(type); // 타입에 맞는 프리팹 선택
        GameObject GameOJNode = Instantiate(nodePrefab, Container);
        Node gameOJNode = GameOJNode.GetComponent<Node>();
        gameOJNode.SetPosition(pos);
        gameOJNode.SetNodeType(type);
        gameOJNode.SetNodeID(nodeid);
        
        // 노드를 선들보다 위에 렌더링되도록 맨 뒤로 이동
        GameOJNode.transform.SetAsLastSibling();
        
        nodelist.Add(gameOJNode);
        map.Add(nodelist);
        nodePositions.Add(pos, gameOJNode.GetNodeType());
    }//Start와 Boss 노드 생성

    private void MakeNode(Vector2 position)
    {
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
                    newPos = new Vector2(position.x + i * roll, position.y + j * roll - nodeCount * 100);
                }
                while (!IsPositionValid(newPos));

                // 먼저 노드 타입을 결정
                NodeType nodeType = GetRandomNodeType();
                GameObject nodePrefab = GetPrefabForNodeType(nodeType); // 타입에 맞는 프리팹 선택
                
                GameObject nodeobj = Instantiate(nodePrefab, Container);
                Node node = nodeobj.GetComponent<Node>();
                node.SetPosition(newPos);
                node.SetNodeType(nodeType); // 미리 결정된 타입 설정
                node.SetNodeID(nodeID);
                
                // 노드를 선들보다 위에 렌더링되도록 맨 뒤로 이동
                nodeobj.transform.SetAsLastSibling();
                
                nodeInCol.Add(node);
                nodeID++;
                nodePositions.Add(newPos, node.GetNodeType());
            }
            map.Add(nodeInCol);
        }
    }

    void GenerateMap()
    {
        Vector2 FirstNodePosition = firstNodePO.transform.position;
        MakePerNode(NodeType.Start, 0);
        MakeNode(FirstNodePosition);
        MakePerNode(NodeType.Boss, nodePositions.Count);
        currentNode = map[0][0];
    }

    void ConnectNodes()
    {
        for (int i = 0; i < row - 1; i++)
        {
            List<Node> currentCol = map[i]; // ���� ���� ��� ����Ʈ
            List<Node> nextCol = map[i + 1]; // ���� ���� ��� ����Ʈ

            foreach (Node node in currentCol)
            {
                if (i == row - 2) // ������ ���̸� ��� ��带 ���� ��忡 ����
                {
                    foreach (Node bossNode in nextCol)
                    {
                        node.connectedNodes.Add(bossNode);
                    }
                }
                else if (i == 0) // ù ��° ���̸� ��� ��带 ���� ��忡 ����
                {
                    foreach (Node startNode in nextCol)
                    {
                        node.connectedNodes.Add(startNode);
                        
                    }
                }
                else // �Ϲ����� ���, �� ��带 ���� ���� 2�� ���� ����
                {
                    // �� ��尡 �� ���� ���͸� ����ǵ��� ó��
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

        // ��� ������ ������ �� �Ǿ����� Ȯ���ϰ�, ������� ���� ���鳢���� ���� ó��
        EnsureAllNodesConnected();
    }

    void EnsureAllNodesConnected()
    {
        // ��� ��带 üũ�Ͽ� ������� ���� ��尡 �ִٸ�, �̸� �ذ�
        for (int i = 0; i < row - 1; i++)
        {
            List<Node> currentCol = map[i]; // ���� ���� ��� ����Ʈ
            List<Node> nextCol = map[i + 1]; // ���� ���� ��� ����Ʈ

            foreach (Node node in currentCol)
            {
                // ����� ��尡 ������, ���� ���� ��� �� �ϳ��� ����
                if (node.connectedNodes.Count == 0)
                {
                    Node fallbackNode = nextCol[Random.Range(0, nextCol.Count)];
                    node.connectedNodes.Add(fallbackNode);
                }
            }

            foreach (Node nextNode in nextCol)
            {
                // ����� ��尡 ������, ���� ���� ��� �� �ϳ��� ����
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
        
        // 모든 선들을 하이어라키 맨 위로 보내서 노드들보다 뒤에 렌더링
        foreach (var line in lines)
        {
            if (line != null)
            {
                line.transform.SetAsFirstSibling();
            }
        }
    }

    void CreateLineBetweenNodes(Node nodeA, Node nodeB)
    {
        // 점선 효과를 위해 여러 개의 작은 선분으로 나누어 그리기
        Vector3 posA = nodeA.transform.position;
        Vector3 posB = nodeB.transform.position;
        float totalDistance = Vector3.Distance(posA, posB);
        
        // 점선 설정
        float dashLength = 20f; // 각 선분의 길이
        float gapLength = 10f;  // 선분 사이의 간격
        float dashThickness = 5f; // 선의 두께
        
        Vector3 direction = (posB - posA).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        float currentDistance = 0f;
        bool isDash = true; // 선분을 그릴지 말지 결정
        
        while (currentDistance < totalDistance)
        {
            if (isDash)
            {
                // 선분 그리기
                float segmentLength = Mathf.Min(dashLength, totalDistance - currentDistance);
                Vector3 startPos = posA + direction * currentDistance;
                Vector3 endPos = posA + direction * (currentDistance + segmentLength);
                Vector3 midPoint = (startPos + endPos) / 2f;
                
                GameObject lineObj = Instantiate(linePrefab, Container);
                
                // LineRenderer가 있다면 UI에서는 사용하지 않고 Image로 변경
                LineRenderer line = lineObj.GetComponent<LineRenderer>();
                if (line != null)
                {
                    // LineRenderer 방식 (점선 효과 제한적)
                    line.useWorldSpace = false;
                    Vector3 localPosA = Container.InverseTransformPoint(startPos);
                    Vector3 localPosB = Container.InverseTransformPoint(endPos);
                    line.positionCount = 2;
                    line.SetPosition(0, localPosA);
                    line.SetPosition(1, localPosB);
                    line.sortingLayerName = "Default";
                    line.sortingOrder = -100;
                    line.startWidth = dashThickness / 100f; // LineRenderer는 월드 단위
                    line.endWidth = dashThickness / 100f;
                }
                else
                {
                    // UI Image 방식 (선호되는 방법)
                    RectTransform lineRect = lineObj.GetComponent<RectTransform>();
                    if (lineRect != null)
                    {
                        lineRect.position = midPoint;
                        lineRect.sizeDelta = new Vector2(segmentLength, dashThickness);
                        lineRect.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
                
                lines.Add(lineObj);
                lineObj.transform.SetAsFirstSibling();
                
                currentDistance += segmentLength;
            }
            else
            {
                // 간격 건너뛰기
                currentDistance += gapLength;
            }
            
            isDash = !isDash; // 선분과 간격을 번갈아 가며
        }
    }


    public void MovePlayer(Node selectedNode)
    {
        if (currentNode.connectedNodes.Contains(selectedNode))
        {
            currentNode = selectedNode;
            CurrNodeID = currentNode.NodeID;
            currentNode.connectedNodes.ForEach(node => node.UpdateVisual());
        }
    }

    /// <summary>
    /// 노드 타입에 따라 적절한 프리팹을 반환
    /// </summary>
    /// <param name="nodeType">노드 타입</param>
    /// <returns>해당하는 프리팹 GameObject</returns>
    private GameObject GetPrefabForNodeType(NodeType nodeType)
    {
        switch (nodeType)
        {
            case NodeType.Battle:
                return battleNodePrefab != null ? battleNodePrefab : defaultNodePrefab;
            case NodeType.Shop:
                return shopNodePrefab != null ? shopNodePrefab : defaultNodePrefab;
            case NodeType.Event:
                return eventNodePrefab != null ? eventNodePrefab : defaultNodePrefab;
            case NodeType.Start:
                return startNodePrefab != null ? startNodePrefab : defaultNodePrefab;
            case NodeType.Boss:
                return bossNodePrefab != null ? bossNodePrefab : defaultNodePrefab;
            default:
                return defaultNodePrefab;
        }
    }

    /// <summary>
    /// 확률에 따라 랜덤한 노드 타입을 반환
    /// </summary>
    /// <returns>랜덤하게 선택된 노드 타입</returns>
    private NodeType GetRandomNodeType()
    {
        int roll = Random.Range(0, 100);
        int cumulative = 0;

        if (roll < (cumulative += battleChance))
        {
            return NodeType.Battle;
        }
        else if (roll < (cumulative += eventChance))
        {
            return NodeType.Event;
        }
        else if (roll < (cumulative += shopChance))
        {
            return NodeType.Shop;
        }
        
        // 기본값으로 배틀 반환
        return NodeType.Battle;
    }

}
