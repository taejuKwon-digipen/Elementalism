// �� ��ũ��Ʈ�� ���ӿ��� ���� ����� ���۰� �ð��� ǥ���� �����մϴ�.
// ������ Ÿ�Կ� ���� �̹��� ����, ����� ���� �� Ȱ��ȭ ����,
// �׸��� �浹 ���� ���� ���� �����Ͽ� ���� ������ ���ͷ����� �����մϴ�.

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
    public bool SquareOccupied { get; set; }     // �ش� ĭ�� �����Ǿ����� ����

    private void Start()
    {
        Selected = false;       // ���� ���� �ʱ�ȭ
        SquareOccupied = false;  // ���� ���� �ʱ�ȭ

        // �ʱ�ȭ �� ���� ���� Ÿ������ ����
        elementType = (ElementType)Random.Range(1, 5);
        SetBlockImage(elementType);
    }

    private void Update()
    {
        SetBlockImage(elementType);
    }

    // �� ĭ�� ����� �� �ִ��� Ȯ���ϴ� �޼���
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage�� Ȱ��ȭ�Ǿ� ������ true ��ȯ
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    // ĭ�� Ȱ��ȭ�ϴ� �޼���
    // ĭ�� Ȱ��ȭ�ϴ� �޼���
    public void ActivateSquare()
    {
        if (currentCollidedBlock != ElementType.None)
        {
            hooverImage.gameObject.SetActive(false);    // hooverImage ��Ȱ��ȭ
            activeImage.gameObject.SetActive(true);     // activeImage Ȱ��ȭ


            elementType = currentCollidedBlock;

            Selected = true;                            // ���õ����� ����
            SquareOccupied = true;                       // ���������� ����
        }
    }

    public void SetElementType(ElementType newType)
    {
        elementType = newType;
    }

    // ���� Ÿ�Կ� ���� �̹��� �����ϴ� �޼���
    public void SetBlockImage(ElementType elementType)
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

        Selected = true;
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
        Selected = true;
        hooverImage.gameObject.SetActive(true);     // hooverImage Ȱ��ȭ
    }

    // �浹�� ����Ǿ��� �� ȣ��Ǵ� �޼���
    private void OnTriggerExit2D(Collider2D collision)
    {
        Selected = false;
        hooverImage.gameObject.SetActive(false);
        isColliding = false; // �浹 ���� ����
    }
}
