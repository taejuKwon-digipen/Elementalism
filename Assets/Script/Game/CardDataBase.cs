using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//List<CardStatBase> CardList = new List<CardStatBase>(); //카드 넣어놓기

public class CardDtatBase : MonoBehaviour
{
    public string Name;
    public int Attack;
    public int Health;
    public Sprite sprite;
    public float percent; //등장 확률 
    //필요시 추가

    private void Awake()
    {
       
    }

    void AddCard()
    {
        //CardList.Add();
    }
}



