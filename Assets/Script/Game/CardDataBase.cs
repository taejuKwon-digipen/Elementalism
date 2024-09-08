using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]

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
    public List<Card> CardList = new List<Card>();
   
    void Awake()
    {
        CardList.Add(new Card(0, 1, 1, "Å×½ºÆ®"));
    }

    void AddCard()
    {
        
    }
}



