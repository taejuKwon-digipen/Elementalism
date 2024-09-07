using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardDtatBase : MonoBehaviour
{
    public static CardDtatBase Instance
    {
        get
        {
            return _instance;
        }
    }
    private static CardDtatBase _instance;

    GoogleSheetLoader datasheet;

    public int ID;
    public int Attack;
    public int Health;
    public float Percent; //등장 확률 
    //public Sprite sprite;
    //필요시 추가

    private void Awake()
    {
    }

    void AddCard()
    {
        
    }
}



