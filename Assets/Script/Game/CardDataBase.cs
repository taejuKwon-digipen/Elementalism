using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//List<CardStatBase> CardList = new List<CardStatBase>(); //ī�� �־����

public class CardDtatBase : MonoBehaviour
{
    public string Name;
    public int Attack;
    public int Health;
    public Sprite sprite;
    public float percent; //���� Ȯ�� 
    //�ʿ�� �߰�

    private void Awake()
    {
       
    }

    void AddCard()
    {
        //CardList.Add();
    }
}



