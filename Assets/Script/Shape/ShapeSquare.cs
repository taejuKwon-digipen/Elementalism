// 이 스크립트는 게임에서 사용되는 ShapeSquare 클래스의 동작을 정의합니다.
// 퍼즐 조각의 개별 블록(square)에 대한 상태를 관리하며,
// 블록이 점유되었을 때 표시할 이미지를 제어합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;
    public ElementType elementType;

    // 각 원소에 해당하는 스프라이트
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite airSprite;
    public Sprite earthSprite;

    void Start()
    {
        occupiedImage.gameObject.SetActive(false);
        SetElementImage();
    }

    public void SetElementType(ElementType type)
    {
        elementType = type;
        SetElementImage();
    }

    private void SetElementImage()
    {
        var image = GetComponent<Image>();
        switch (elementType)
        {
            case ElementType.Fire:
                image.sprite = fireSprite;
                break;
            case ElementType.Water:
                image.sprite = waterSprite;
                break;
            case ElementType.Air:
                image.sprite = airSprite;
                break;
            case ElementType.Earth:
                image.sprite = earthSprite;
                break;
            default:
                image.sprite = null;
                break;
        }
    }
}
