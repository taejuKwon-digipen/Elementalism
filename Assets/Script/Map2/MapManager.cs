using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public Transform mapContainer; // 맵 노드가 들어갈 부모 오브젝트
    [SerializeField] public LineRenderer linePrefab; // 라인 프리펩

    private List<List<Node>> map = new(); //층별 노드리스트
    private Node currentNode; //현재 플레이어가 위치한 노드

    void Start()
    {
        GenerateMap();
        ConncetNodes();
        DrawMap();
    }

    //노드 생성
    void GenerateMap()
    {
        for(int i = 0; i < col; i++ )
        {
            int nodeCount = Random.Range(minNodePerCol, maxNodePerCol);
            List<Node> nodeInCol = new(); //현재 층의 노드 리스트
            
            for(int j = 0; j < nodeCount; j++ )
            {
                GameObject nodeobj = Instantiate(nodePrefab, mapContainer); //노드 프리펩 생성
                Node node = nodeobj.GetComponent<Node>(); //노드 컴포넌트 가져오기
                node.SetPosition(new Vector2(col * 200, i * 150 - nodeCount * 75)); //노드위치 배치
                nodeInCol.Add(node);// 리스트에 추가
            }
            map.Add(nodeInCol);//전체 맵 리스트에 추가
        }
    }

    //노드 연결
    void ConncetNodes()
    {
        for(int i = 0; i < col -1; i++)
        {
            List<Node> currentCol = map[i]; //현재 층의 노드 리스트
            List<Node> nextCol = map[i + 1];//다음 층의 노드 리스트

            foreach(Node node in currentCol)
            {
                int connections = Random.Range(1, nextCol.Count + 1); //최소 한개 이상 연결 생성
                // HashSet -> 고유한 요소 집합을 저장하는 컬렉션 : 중복값 허용X
                HashSet<int> connectedIndices = new(); //중복 연결 방지를 위한 HashSet

                while (connectedIndices.Count < connections)
                {
                    int randomIndex = Random.Range(0, nextCol.Count); //랜덤한 노드 선택

                    if(!connectedIndices.Contains(randomIndex)){ //랜덤 노드 선택
                        connectedIndices.Add(randomIndex);
                        node.connectedNodes.Add(nextCol[randomIndex]); //노드 연결 저장
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
                    LineRenderer line = Instantiate(linePrefab, mapContainer); // 선(LineRenderer) 생성
                    line.positionCount = 2;
                    line.SetPosition(0, node.transform.position); // 시작점
                    line.SetPosition(1, connectedNode.transform.position); // 끝점
                }
            }
        }
    }

    public void MovePlayer(Node selectedNode)
    {
        if (currentNode == null || currentNode.connectedNodes.Contains(selectedNode))
        { // 이동 가능한 노드인지 확인
            currentNode = selectedNode; // 플레이어의 현재 위치 업데이트
            // 선택한 노드에 대한 이벤트 실행 (전투, 상점 등)
        }
    }
}
