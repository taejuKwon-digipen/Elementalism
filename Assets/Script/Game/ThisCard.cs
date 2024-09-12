using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard = new List<Card>();
    public int ThisID;

    public int ID;
    public int Attack;
    public int Health;
    //public float Percent; //µîÀå È®·ü 
    public string CardDescription;

    void Start()
    {
        //thisCard[0] = CardDataBase.CardList[ThisID];
    }

    void Update()
    {
        //ID = thisCard[0].ID;
        //Attack = thisCard[0].Attacl;
        
    }

}
