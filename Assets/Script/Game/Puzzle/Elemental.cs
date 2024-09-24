using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 원소 타입 Enum
public enum ElementType
{
    Fire,
    Water,
    Air,
    Earth
}

public class Elemental : MonoBehaviour
{
    public Image nomalImage;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite airSprite;
    public Sprite earthSprite;

    // 원소 타입에 따라 이미지 설정
    public void SetRandomElementImage(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                nomalImage.sprite = fireSprite;
                break;
            case ElementType.Water:
                nomalImage.sprite = waterSprite;
                break;
            case ElementType.Air:
                nomalImage.sprite = airSprite;
                break;
            case ElementType.Earth:
                nomalImage.sprite = earthSprite;
                break;
        }
    }
}