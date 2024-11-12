using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
[System.Serializable] //unity �ν����� â���� ������ �� �ֵ��� ����

public class Prop
{
    public bool Fire;
    public bool Water;
    public bool Ground;
    public bool Wind;

}*/

[System.Serializable]
public class CardItem
{
    public string CardName;
    public int ID;
    public int PowerLeft;
    public int PowerRight;
    public string CardDescription;
    public bool UseMagic = false;
    public float Percent;

    public bool UseProp = false;
    public bool UseBuff = false;
    public bool UseDraw = false;

    public int Damage;
    public int AddDamage;

    public ShapeData cardShape; // 카드 모양 (ShapeData 사용)
    public ElementType CreatedElementType;

    /* void Awake()
     {

         if(UseProp == true)
         {

         }
     }*/

}


[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class CardItemSO: ScriptableObject
{
    public CardItem[] items;
}
