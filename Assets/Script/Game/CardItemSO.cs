using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI; // UI 관련 스크립트에 활용
using Sirenix.OdinInspector;

/*
[System.Serializable] //unityν â   ֵ 

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
    [TabGroup("기본")]
    [LabelText("이름")]
    public string CardName;
    
    [TabGroup("기본")]
    [LabelText("ID")]
    public int ID;
    
    [TabGroup("기본")]
    [TextArea(2, 3)]
    [LabelText("설명")]
    public string CardDescription;
    
    [TabGroup("기본")]
    [LabelText("이미지")]
    public Texture2D cardImage;
    
    [TabGroup("능력치")]
    [LabelText("기본 공격력")]
    public int PowerLeft;
    
    [TabGroup("능력치")]
    [LabelText("크리티컬 공격력")]
    public int PowerRight;
    
    [TabGroup("능력치")]
    [LabelText("원소")]
    public ElementType CreatedElementType;
    
    [TabGroup("특성")]
    [LabelText("마법")]
    public bool UseMagic = false;
    
    [TabGroup("특성")]
    [LabelText("확률")]
    [ShowIf("UseMagic")]
    public float Percent;
    
    [TabGroup("특성")]
    [LabelText("속성")]
    public bool UseProp = false;
    
    [TabGroup("특성")]
    [LabelText("버프")]
    public bool UseBuff = false;
    
    [TabGroup("특성")]
    [LabelText("드로우")]
    public bool UseDraw = false;
    
    [TabGroup("설정")]
    [LabelText("이미지사용")]
    public bool UseImage = false;
    
    [TabGroup("설정")]
    [LabelText("언락")]
    public bool IsUnlocked = false;
    
    [TabGroup("설정")]
    [LabelText("모양")]
    public ShapeData cardShape;

    /* void Awake()
     {

         if(UseProp == true)
         {

         }
     }*/

}


[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class CardItemSO : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    [LabelText("카드 목록")]
    public CardItem[] items;

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtility.SetDirty(this);
    }
#endif
}
