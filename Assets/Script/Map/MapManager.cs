using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum NodeType
{
    Battle,
    Event,
    Shop,
    Rest,
    Boss
}

public class Node
{
    public Vector2 position;
    public NodeType type;
    public List<Node> connectedNodes;

}

public class MapManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public Transform map;


    public int columns = 5;
    public int nodePerColumn = 3;

    private readonly Dictionary<NodeType, int> types = new()
    {
        { NodeType.Battle, 50 },
        { NodeType.Event, 30 },
        { NodeType.Rest, 10 },
        { NodeType.Shop, 5 },
    };
    private List<Node> nodes = new();
    private readonly List<List<Node>> nodesByColumn = new();

    void Start()
    {
        GenerateMap();
        DrawConnection();
    }

    private void GenerateMap()
    {
        List<Node> previousColumn = new();
        Node startingNode = new Node{
            position = new Vector3(-2, 0, 0),
            type = GetRandomNodeType(),
            connectedNodes = new List<Node>()
        };
        GameObject newObject = Instantiate(nodePrefab, startingNode.position, Quaternion.identity, map);
        previousColumn.Add(startingNode);

        for (int col = 0; col < columns; col += 1) {
            List<Node> currentColumn = new();

            for (int row = 0; row < nodePerColumn; row += 1) {
                Vector2 position = new Vector2(col * 3f + Random.Range(-0.5f, 0.5f), row * 2f + Random.Range(-0.5f, 0.5f));
                Node newNode = new Node {
                    position = position,
                    type = GetRandomNodeType(),
                    connectedNodes = new List<Node>()
                };
                newObject = Instantiate(nodePrefab, newNode.position, Quaternion.identity, map);

                if (previousColumn.Count > 0) {
                    Node parent = previousColumn[UnityEngine.Random.Range(0, previousColumn.Count)];
                    parent.connectedNodes.Add(newNode);
                }
                newObject.GetComponent<NodeManager>().node = newNode;
                currentColumn.Add(newNode);
                nodes.Add(newNode);
            }
            previousColumn = currentColumn;
        }
    }

    private NodeType GetRandomNodeType()
    {
        int totalWeight = types.Values.Sum();
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        foreach (var pair in types) {
            cumulative += pair.Value;
            if (randomValue < cumulative) {
                return pair.Key;
            }
        }

        return NodeType.Battle; // Default one
    }

    private void DrawConnection()
    {
        /*foreach (var node in nodes) {
            foreach(var connectedNode in node.connectedNodes) {        
                GameObject line = Instantiate(linePrefab, map);
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

                lineRenderer.SetPosition(0, node.position);
                lineRenderer.SetPosition(1, connectedNode.position);
            }
        }*/
    }
}
