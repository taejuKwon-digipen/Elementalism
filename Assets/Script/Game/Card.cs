using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity 인스펙터 창에서 수정할 수 있도록 설정


public class Card : MonoBehaviour 
{

    CardDtatBase carddatabase;
    List<CardDtatBase> CardList = new List<CardDtatBase>(); //카드 넣어놓기

    void Init()
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //여기서 카드 꺼내기
    }
}
