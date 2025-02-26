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
    //col�� ��� ���� ����
    [SerializeField] public int minNodePerCol;
    [SerializeField] public int maxNodePerCol;

    //100�� �����ʰ� �����ϱ�
    [SerializeField] public int battleChance;
    [SerializeField] public int eventChance;
    [SerializeField] public int shopChance;

    [SerializeField] public GameObject nodePrefab; // ��� UI ������
    [SerializeField] public Transform NodemapContainer; // �� ��尡 �� �θ� ������Ʈ
    [SerializeField] public Transform LinemapContainer; // ���� �� �θ� ������Ʈ
    [SerializeField] public GameObject linePrefab; // ���� ������
    public List<GameObject> lines = new();

    [SerializeField] public GameObject firstNodePO;

    public string SceneToLoad;

    private List<List<Node>> map = new(); //���� ��帮��Ʈ
    private Node currentNode; //���� �÷��̾ ��ġ�� ���

    void Start()
    {
        GenerateMap();
        ConnectNodes();
        DrawMap();

        map[0][0].SetSelectable(true);
        MovePlayer(map[0][0]);
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
                while (!IsPositionValid(newPos)); // ��ȿ�� ��ġ�� ������ �ݺ�

                GameObject nodeobj = Instantiate(nodePrefab, NodemapContainer);
                Node node = nodeobj.GetComponent<Node>();
                node.SetPosition(newPos);
                nodeInCol.Add(node);
            }
            map.Add(nodeInCol);
        }

        // Boss Node �߰�
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
        while (!IsPositionValid(bossPos)); // ��ȿ�� ��ġ�� ������ �ݺ�

        bossNode.SetPosition(bossPos);
        nodeInEnd.Add(bossNode);
        map.Add(nodeInEnd);

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
    }

    void CreateLineBetweenNodes(Node nodeA, Node nodeB)
    {
        GameObject lineObj = Instantiate(linePrefab, LinemapContainer);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        line.useWorldSpace = false; // World Space ��� �� �� (�θ� �������� �����̰�)

        // �θ�(Content) �������� ������� ��ǥ�� �����ؾ� ��
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
            currentNode = selectedNode; // �÷��̾��� ���� ��ġ ������Ʈ
            Debug.Log("���� ���: " + currentNode.name);

            // ������ ��¦�̴� ������ ����
            foreach (Node node in map.SelectMany(nodes => nodes))
            {
                node.StopBlinking();
            }

            // ���� ����� ����� ������ ��¦�̰� ����
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
