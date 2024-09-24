using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private BlockRoot[,] allTiles;
    public GameObject[,] allDots;

    void Start()
    {
        Debug.Log("hi");
        allTiles = new BlockRoot[width * 50, height * 50];
        allDots = new GameObject[width, height];
        SetUp(); 
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(50 * i + 400, 50 * j + 30);
                GameObject background = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                background.transform.parent = this.transform;
                background.name = "( " + i + " , " + j + ")";

                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + " , " + j + ")";
                allDots[i,j] = dot;
            }
        }
    }

    void Update()
    {
    }
}
