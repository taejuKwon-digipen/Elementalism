using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] //unity 인스펙터 창에서 수정할 수 있도록 설정


public class Card
{
    public string CardName;
    public int ID;
    public int PowerLeft;
    public int PowerRight;
    public string CardDescription;
    public bool UseMagic = false;
    public float Percent;
    //public Sprite sprite;
}

[CreateAssetMenu(fileName = "CardStat", menuName = "Scriptable Object/CardStat")]
public class CardStat: ScriptableObject
{
    public Card[] cards;
}
