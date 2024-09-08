using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity 인스펙터 창에서 수정할 수 있도록 설정


public class Card : MonoBehaviour 
{
    public int ID;
    public int Attack;
    public int Health;
    //public float Percent; //등장 확률 
    public string CardDescription;
    //public Sprite sprite;
    //필요시 추가

    public Card()
    {

    }

    public Card(int id , int attack, int health, string carddescription)
    {
        ID = id;
        Attack = attack;
        Health = health;
        //Percent = percent; 
        CardDescription = carddescription;  
    }
}
