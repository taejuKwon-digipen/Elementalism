using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity 인스펙터 창에서 수정할 수 있도록 설정


public class Card : MonoBehaviour 
{
    public string CardName;
    public int ID;
    public int  Attack;
    //public float Percent; //등장 확률 
    public string CardDescription;
    public bool UseMagic = false;
    //public Sprite sprite;


    public Card()
    {

    }

    public Card(string cardname, int id , int attack, string carddescription, bool usemagic)
    {
        CardName = cardname;
        ID = id;
        Attack = attack;
        //Percent = percent; 
        CardDescription = carddescription;  
        UseMagic = usemagic;
    }
}
