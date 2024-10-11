// �� ��ũ��Ʈ�� ���ӿ��� ���� ����� ���۰� �ð��� ǥ���� �����մϴ�.
// ������ Ÿ�Կ� ���� �̹��� ����, ����� ���� �� Ȱ��ȭ ����,
// �׸��� �浹 ���� ���� ���� �����Ͽ� ���� ������ ���ͷ����� �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���� Ÿ���� �����ϴ� ������ Enum
public enum ElementType
{
    None,
    Fire,   // ��
    Water,  // ��
    Air,    // ����
    Earth   // ��
}

public class Block : MonoBehaviour
{
    // �̹��� ������Ʈ��
    public Image hooverImage;    // ���콺 ���� �� ��Ÿ���� �̹���
    public Image activeImage;    // Ȱ��ȭ�� ������ �̹���
    public Image nomalImage;     // �⺻ ������ �̹���

    // �� ���ҿ� �ش��ϴ� ��������Ʈ��
    public Sprite fireSprite;    // �� ��������Ʈ
    public Sprite waterSprite;   // �� ��������Ʈ
    public Sprite airSprite;     // ���� ��������Ʈ
    public Sprite earthSprite;   // �� ��������Ʈ

    public ElementType elementType = ElementType.None; // ���� Ÿ���� �����ϴ� ���� (�ʱ�ȭ)
    public ElementType currentCollidedBlock = ElementType.None;
    private bool isColliding = false;

    // ������Ƽ��
    public bool Selected { get; set; }          // ���õǾ����� ����
    public int BlockIndex { get; set; }     // ������ �ε���
    public bool SqareOccupied { get; set; }     // �ش� ĭ�� �����Ǿ����� ����

    private void Start()
    {
        Selected = false;       // ���� ���� �ʱ�ȭ
        SqareOccupied = false;  // ���� ���� �ʱ�ȭ

        // �ʱ�ȭ �� ���� ���� Ÿ������ ����
        elementType = (ElementType)Random.Range(1, 5);
        SetRandomBlockImage(elementType);
    }

    // �� ĭ�� ����� �� �ִ��� Ȯ���ϴ� �޼���
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage�� Ȱ��ȭ�Ǿ� ������ true ��ȯ
    }

    // ĭ�� Ȱ��ȭ�ϴ� �޼���
    // ĭ�� Ȱ��ȭ�ϴ� �޼���
    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);    // hooverImage ��Ȱ��ȭ
        activeImage.gameObject.SetActive(true);     // activeImage Ȱ��ȭ

        // Ȱ��ȭ�� �̹����� ��������Ʈ�� ���� Ÿ�Կ� �°� ����
        switch (currentCollidedBlock)
        {
            case ElementType.Fire:
                activeImage.sprite = fireSprite;     // �� ��������Ʈ�� ����
                break;
            case ElementType.Water:
                activeImage.sprite = waterSprite;    // �� ��������Ʈ�� ����
                break;
            case ElementType.Air:
                activeImage.sprite = airSprite;      // ���� ��������Ʈ�� ����
                break;
            case ElementType.Earth:
                activeImage.sprite = earthSprite;    // �� ��������Ʈ�� ����
                break;
            default:
                activeImage.sprite = null;
                break;
        }

        Selected = true;                            // ���õ����� ����
        SqareOccupied = true;                       // ���������� ����
    }

    // ���� Ÿ�Կ� ���� �̹��� �����ϴ� �޼���
    public void SetRandomBlockImage(ElementType elementType)
    {
        this.elementType = elementType; // ���� Ÿ�� ����
        switch (elementType)
        {
            case ElementType.Fire:
                nomalImage.sprite = fireSprite;     // �� ��������Ʈ�� ����
                break;
            case ElementType.Water:
                nomalImage.sprite = waterSprite;    // �� ��������Ʈ�� ����
                break;
            case ElementType.Air:
                nomalImage.sprite = airSprite;      // ���� ��������Ʈ�� ����
                break;
            case ElementType.Earth:
                nomalImage.sprite = earthSprite;    // �� ��������Ʈ�� ����
                break;
            default:
                nomalImage.sprite = null;
                break;
        }
    }
    // �浹 ���� �� ȣ��Ǵ� �޼���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true); // hooverImage Ȱ��ȭ

        ShapeSquare collidedSquare = collision.GetComponent<ShapeSquare>();

        if (collidedSquare != null)
        {
            currentCollidedBlock = collidedSquare.elementType; // �浹�� ���� Ÿ�� ����
            isColliding = true; // �浹 ���� ����
            Debug.Log("�浹�� ���� Ÿ��: " + currentCollidedBlock);
        }
    }


    // �浹 ���� �� ȣ��Ǵ� �޼���
    private void OnTriggerStay2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);     // hooverImage Ȱ��ȭ
    }

    // �浹�� ����Ǿ��� �� ȣ��Ǵ� �޼���
    private void OnTriggerExit2D(Collider2D collision)
    {
        ShapeSquare collidedSquare = collision.GetComponent<ShapeSquare>();

        if (collidedSquare != null && currentCollidedBlock == collidedSquare.elementType)
        {
            hooverImage.gameObject.SetActive(false);
            currentCollidedBlock = ElementType.None; // ���ǿ� ���� �ʱ�ȭ
            isColliding = false; // �浹 ���� ����
            Debug.Log("�浹 ����: ���� Ÿ�� �ʱ�ȭ��");
        }
    }



}
