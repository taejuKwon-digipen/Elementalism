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
    public float Percent; //���� Ȯ�� 
    //public Sprite sprite;
    //�ʿ�� �߰�

    private void Awake()
    {
    }

    void AddCard()
    {
        
    }
}


