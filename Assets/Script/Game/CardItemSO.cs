using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] //unity �ν����� â���� ������ �� �ֵ��� ����
public class CardItem
{
    public string CardName;
    public int ID;
    public int PowerLeft;
    public int PowerRight;
    public string CardDescription;
    public bool UseMagic = false;
    public float Percent;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class CardItemSO: ScriptableObject
{
    public CardItem[] items;
}
