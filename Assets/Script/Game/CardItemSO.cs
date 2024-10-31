using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable] //unity 인스펙터 창에서 수정할 수 있도록 설정

public class Prop
{
    public bool Fire;
    public bool Water;
    public bool Ground;
    public bool Wind;

}
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
