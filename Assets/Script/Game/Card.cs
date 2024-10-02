using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity �ν����� â���� ������ �� �ֵ��� ����


public class Card : MonoBehaviour 
{
    public int ID;
    public int Attack;
    //public float Percent; //���� Ȯ�� 
    public string CardDescription;
    public bool UseMagic = false;
    //public Sprite sprite;


    public Card()
    {

    }

    public Card(int id , int attack, string carddescription, bool usemagic)
    {
        ID = id;
        Attack = attack;
        //Percent = percent; 
        CardDescription = carddescription;  
        UseMagic = usemagic;
    }
}
