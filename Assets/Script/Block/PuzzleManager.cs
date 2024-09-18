using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private BlockRoot[,] allTiles;

    void Start()
    {
        allTiles = new BlockRoot[width * 100, height * 100];
        SetUp(); 
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(100 * i + 450, 100 * j + 50);
                GameObject background = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                background.transform.parent = this.transform;
                background.name = "( " + i + " , " + j + ")";
            }
        }
    }

    void Update()
    {
    }
}
