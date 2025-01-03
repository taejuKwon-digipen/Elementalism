using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public class CardItem
{
    [HorizontalGroup("Header")]
    [VerticalGroup("Header/Left")]
    [LabelText("카드 이름")]
    public string CardName;
    
    [VerticalGroup("Header/Left")]
    [LabelText("카드 ID")]
    public int ID;

    [VerticalGroup("Header/Right")]
    [PreviewField(100)]
    public Texture2D cardImage;

    [FoldoutGroup("상세 정보")]
    [LabelText("설명")]
    [TextArea(3,5)]
    public string CardDescription;

    [FoldoutGroup("상세 정보")]
    [LabelText("왼쪽 파워")]
    public int PowerLeft;
    
    [FoldoutGroup("상세 정보")]
    [LabelText("오른쪽 파워")]
    public int PowerRight;

    [FoldoutGroup("상세 정보")]
    [LabelText("마법 사용")]
    public bool UseMagic = false;
    
    [FoldoutGroup("상세 정보")]
    [LabelText("확률")]
    [ShowIf("UseMagic")]
    [Range(0, 100)]
    public float Percent;

    [FoldoutGroup("상세 정보")]
    public bool UseImage = false;
    
    [FoldoutGroup("상세 정보")]
    public bool UseProp = false;
    
    [FoldoutGroup("상세 정보")]
    public bool UseBuff = false;
    
    [FoldoutGroup("상세 정보")]
    public bool UseDraw = false;

    [FoldoutGroup("상세 정보")]
    public ShapeData cardShape;
    
    [FoldoutGroup("상세 정보")]
    public ElementType CreatedElementType;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class CardItemSO: ScriptableObject
{
    [ListDrawerSettings(
        ShowPaging = true,
        NumberOfItemsPerPage = 10,
        CustomAddFunction = "AddNewCard",
        CustomRemoveElementFunction = "RemoveCard",
        OnTitleBarGUI = "DrawTitleBar",
        ListElementLabelName = "CardName"
    )]
    [Searchable]
    public CardItem[] items;

    private void DrawTitleBar()
    {
        if (items != null && items.Length > 0)
        {
            GUILayout.Label($"카드 수: {items.Length}");
        }
    }

    private CardItem AddNewCard()
    {
        return new CardItem { CardName = "새 카드" };
    }

    private void RemoveCard(CardItem card)
    {
        // 카드 제거 시 필요한 추가 로직이 있다면 여기에 구현
    }
}
