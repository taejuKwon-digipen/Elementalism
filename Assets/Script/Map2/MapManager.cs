using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] public LineRenderer linePrefab; // ���� ������

    private List<List<Node>> map = new(); //���� ��帮��Ʈ
    private Node currentNode; //���� �÷��̾ ��ġ�� ���

    void Start()
    {
        GenerateMap();
        ConncetNodes();
        DrawMap();
    }

    //��� ����
    void GenerateMap()
    {
        for(int i = 0; i < col; i++ )
        {
            int nodeCount = Random.Range(minNodePerCol, maxNodePerCol);
            List<Node> nodeInCol = new(); //���� ���� ��� ����Ʈ
            
            for(int j = 0; j < nodeCount; j++ )
            {
                GameObject nodeobj = Instantiate(nodePrefab, mapContainer); //��� ������ ����
                Node node = nodeobj.GetComponent<Node>(); //��� ������Ʈ ��������
                node.SetPosition(new Vector2(col * 200, i * 150 - nodeCount * 75)); //�����ġ ��ġ
                nodeInCol.Add(node);// ����Ʈ�� �߰�
            }
            map.Add(nodeInCol);//��ü �� ����Ʈ�� �߰�
        }
    }

    //��� ����
    void ConncetNodes()
    {
        for(int i = 0; i < col -1; i++)
        {
            List<Node> currentCol = map[i]; //���� ���� ��� ����Ʈ
            List<Node> nextCol = map[i + 1];//���� ���� ��� ����Ʈ

            foreach(Node node in currentCol)
            {
                int connections = Random.Range(1, nextCol.Count + 1); //�ּ� �Ѱ� �̻� ���� ����
                // HashSet -> ������ ��� ������ �����ϴ� �÷��� : �ߺ��� ���X
                HashSet<int> connectedIndices = new(); //�ߺ� ���� ������ ���� HashSet

                while (connectedIndices.Count < connections)
                {
                    int randomIndex = Random.Range(0, nextCol.Count); //������ ��� ����

                    if(!connectedIndices.Contains(randomIndex)){ //���� ��� ����
                        connectedIndices.Add(randomIndex);
                        node.connectedNodes.Add(nextCol[randomIndex]); //��� ���� ����
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
                    LineRenderer line = Instantiate(linePrefab, mapContainer); // ��(LineRenderer) ����
                    line.positionCount = 2;
                    line.SetPosition(0, node.transform.position); // ������
                    line.SetPosition(1, connectedNode.transform.position); // ����
                }
            }
        }
    }

    public void MovePlayer(Node selectedNode)
    {
        if (currentNode == null || currentNode.connectedNodes.Contains(selectedNode))
        { // �̵� ������ ������� Ȯ��
            currentNode = selectedNode; // �÷��̾��� ���� ��ġ ������Ʈ
            // ������ ��忡 ���� �̺�Ʈ ���� (����, ���� ��)
        }
    }
}
