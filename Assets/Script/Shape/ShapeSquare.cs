// �� ��ũ��Ʈ�� ���ӿ��� ���Ǵ� ShapeSquare Ŭ������ ������ �����մϴ�.
// ���� ������ ���� ���(square)�� ���� ���¸� �����ϸ�,
// ����� �����Ǿ��� �� ǥ���� �̹����� �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;
    public ElementType elementType;

    // �� ���ҿ� �ش��ϴ� ��������Ʈ
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
