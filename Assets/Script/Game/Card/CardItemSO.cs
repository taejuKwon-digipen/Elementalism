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
    [LabelText("현재 레벨")]
    [Range(1, 3)]
    public int CurrentLevel = 1;
    
    [TabGroup("능력치")]
    [LabelText("레벨별 기본 공격력")]
    public List<int> PowerLeftByLevel = new List<int> { 0, 0, 0 };
    
    [TabGroup("능력치")]
    [LabelText("레벨별 크리티컬 공격력")]
    public List<int> PowerRightByLevel = new List<int> { 0, 0, 0 };
    
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

    // CardItem 인스턴스를 복제하는 메서드
    public CardItem Clone()
    {
        CardItem newItem = new CardItem();
        newItem.CardName = this.CardName;
        newItem.ID = this.ID;
        newItem.CardDescription = this.CardDescription;
        newItem.cardImage = this.cardImage; // Texture2D는 참조 복사로 충분할 수 있음
        newItem.CurrentLevel = this.CurrentLevel;
        newItem.PowerLeftByLevel = new List<int>(this.PowerLeftByLevel);
        newItem.PowerRightByLevel = new List<int>(this.PowerRightByLevel);
        newItem.CreatedElementType = this.CreatedElementType;
        newItem.UseMagic = this.UseMagic;
        newItem.Percent = this.Percent;
        newItem.UseProp = this.UseProp;
        newItem.UseBuff = this.UseBuff;
        newItem.UseDraw = this.UseDraw;
        newItem.UseImage = this.UseImage;
        newItem.IsUnlocked = this.IsUnlocked; // IsUnlocked 상태도 복사
        
        // ShapeData는 ScriptableObject이므로, 복제본을 사용하는 것이 안전합니다.
        // ShapeData.Clone() 메서드가 구현되어 있다고 가정합니다.
        if (this.cardShape != null)
        {
            newItem.cardShape = this.cardShape.Clone(); 
        }
        else
        {
            newItem.cardShape = null;
        }

        return newItem;
    }
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

    public void UpdateFromSheet()
    {
        if (GoogleSheetLoader.Instance == null) return;

        foreach (var item in items)
        {
            if (GoogleSheetLoader.Instance.cardDatas.TryGetValue(item.ID, out var sheetCard))
            {
                switch (GoogleSheetLoader.Instance.CurrentLanguage)
                {
                    case GoogleSheetLoader.Language.Korean:
                        item.CardName = sheetCard.Name_KR;
                        item.CardDescription = sheetCard.Desc_KR;
                        break;
                    case GoogleSheetLoader.Language.English:
                        item.CardName = sheetCard.Name_EN;
                        item.CardDescription = sheetCard.Desc_EN;
                        break;
                    case GoogleSheetLoader.Language.Japanese:
                        item.CardName = sheetCard.Name_JP;
                        item.CardDescription = sheetCard.Desc_JP;
                        break;
                }
            }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
