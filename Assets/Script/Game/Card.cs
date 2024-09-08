using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity �ν����� â���� ������ �� �ֵ��� ����


public class Card : MonoBehaviour 
{
    public int ID;
    public int Attack;
    public int Health;
    //public float Percent; //���� Ȯ�� 
    public string CardDescription;
    //public Sprite sprite;
    //�ʿ�� �߰�

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
