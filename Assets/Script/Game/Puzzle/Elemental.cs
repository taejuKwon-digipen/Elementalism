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
    Fire,   // ��
    Water,  // ��
    Air,    // ����
    Earth   // ��
}

public class Elemental : MonoBehaviour
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

    // ������Ƽ��
    public bool Selected { get; set; }          // ���õǾ����� ����
    public int ElementalIndex { get; set; }     // ������ �ε���
    public bool SqareOccupied { get; set; }     // �ش� ĭ�� �����Ǿ����� ����

    // �� ĭ�� ����� �� �ִ��� Ȯ���ϴ� �޼���
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage�� Ȱ��ȭ�Ǿ� ������ true ��ȯ
    }

    // ĭ�� Ȱ��ȭ�ϴ� �޼���
    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);    // hooverImage ��Ȱ��ȭ
        activeImage.gameObject.SetActive(true);     // activeImage Ȱ��ȭ
        Selected = true;                            // ���õ����� ����
        SqareOccupied = true;                       // ���������� ����
    }

    private void Start()
    {
        Selected = false;       // ���� ���� �ʱ�ȭ
        SqareOccupied = false;  // ���� ���� �ʱ�ȭ
    }

    // ���� Ÿ�Կ� ���� �̹��� �����ϴ� �޼���
    public void SetRandomElementImage(ElementType elementType)
    {
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
        hooverImage.gameObject.SetActive(false);    // hooverImage ��Ȱ��ȭ
    }

    // �浹�� ���۵� �� ȣ��Ǵ� �޼���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);     // hooverImage Ȱ��ȭ
    }
}
