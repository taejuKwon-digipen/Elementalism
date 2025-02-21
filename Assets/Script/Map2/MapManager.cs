using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
    public Transform mapContainer; // �� ��尡 �� �θ� ������Ʈ
    [SerializeField] public GameObject linePrefab; // ���� ������
    public List<GameObject> lines = new();

    [SerializeField] public GameObject firstNodePO;

    private List<List<Node>> map = new(); //���� ��帮��Ʈ
    private Node currentNode; //���� �÷��̾ ��ġ�� ���

    void Start()
    {
        GenerateMap();
        ConnectNodes();
        DrawMap();
    }

    //��� ����
    void GenerateMap()
    {
        //First Node �߰�
        Vector2 FirstNodePosition = firstNodePO.transform.position;
        List<Node> nodeInFirst = new(); //���� ���� ��� ����Ʈ
        GameObject firstNode = Instantiate(nodePrefab, mapContainer);
        Node Firstnode = firstNode.GetComponent<Node>();
        Firstnode.SetPosition(FirstNodePosition);
        nodeInFirst.Add(Firstnode);
        map.Add(nodeInFirst);

        for (int i = 1; i < row -1; i++ )
        {
            int nodeCount = Random.Range(minNodePerCol, maxNodePerCol);
            List<Node> nodeInCol = new(); //���� ���� ��� ����Ʈ
            
            for(int j = 0; j < nodeCount; j++ )
            {
                GameObject nodeobj = Instantiate(nodePrefab, mapContainer); //��� ������ ����
                Node node = nodeobj.GetComponent<Node>(); //��� ������Ʈ ��������
                node.SetPosition(new Vector2(FirstNodePosition.x + i * 300, FirstNodePosition.y + j * 300 - nodeCount * 100)); //�����ġ ��ġ
                nodeInCol.Add(node);// ����Ʈ�� �߰�
            }
            map.Add(nodeInCol);//��ü �� ����Ʈ�� �߰�
        }

        //Boss Node �߰�
        List<Node> nodeInEnd = new(); //���� ���� ��� ����Ʈ
        GameObject BossNode = Instantiate(nodePrefab, mapContainer);
        Node bossNode = BossNode.GetComponent<Node>();
        bossNode.SetPosition(new Vector2(FirstNodePosition.x + (row-1) * 300, FirstNodePosition.y) );
        nodeInEnd.Add(bossNode);
        map.Add(nodeInEnd);

        currentNode = map[0][0];
    }

    //��� ����
    void ConnectNodes()
    {
        // ����� ��� 2���� �����ϴ� ������� ����
        for (int i = 0; i < row - 1; i++)
        {
            List<Node> currentCol = map[i]; // ���� ���� ��� ����Ʈ
            List<Node> nextCol = map[i + 1]; // ���� ���� ��� ����Ʈ

            foreach (Node node in currentCol)
            {
                if (i == row - 1) // ������ ���̸� ��� ��带 ���� ��忡 ����
                {
                    foreach (Node bossNode in nextCol)
                    {
                        node.connectedNodes.Add(bossNode);
                    }
                }
                else // �Ϲ����� ���, ����� 2�� ��常 ����
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
        GameObject lineObj = Instantiate(linePrefab, transform); // ���� ������ ����
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, nodeA.transform.position);
        line.SetPosition(1, nodeB.transform.position);

        lines.Add(lineObj); // ����Ʈ�� �߰��Ͽ� ���߿� Ȱ��ȭ/��Ȱ��ȭ ����
    }

    public void MovePlayer(Node selectedNode)
    {
       /* if (currentNode == null || currentNode.connectedNodes.Contains(selectedNode))
        { // �̵� ������ ������� Ȯ��
            currentNode = selectedNode; // �÷��̾��� ���� ��ġ ������Ʈ
            // ������ ��忡 ���� �̺�Ʈ ���� (����, ���� ��)
        }*/
    }
}
