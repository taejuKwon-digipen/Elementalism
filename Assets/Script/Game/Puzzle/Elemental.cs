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
    public Image hooverImage;
    public Image activeImage;
    public Image nomalImage;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite airSprite;
    public Sprite earthSprite;

    public bool Selected { get; set; }
    public int ElementalIndex { get; set; }
    public bool SqareOccupied { get; set; }

    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;
    }

    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SqareOccupied = true;
    }

    private void Start()
    {
        Selected = false;
        SqareOccupied = false;
    }

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

    //충돌이 되고 있을때
    private void OnTriggerStay2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);
    }

    //충돌이 끝날때
    private void OnTriggerExit2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(false);
    }

    //충돌이 들어갈때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);
    }


}