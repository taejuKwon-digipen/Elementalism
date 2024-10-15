// �� ��ũ��Ʈ�� ���� ���� �׸���(����)�� �����ϰ� �����մϴ�.
// �׸��� �ȿ� ���� ��ϵ��� �����ϰ� ��ġ�ϸ�,
// ���� �̺�Ʈ�� ���� ���ҵ��� ���¸� ������Ʈ�մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // �׸����� ���� �� ���� �����մϴ�.
    public int columns = 0;                       // ���� ��
    public int rows = 0;                          // ���� ��
    public float squaresGap = 0.1f;               // ��� ������ ����
    public GameObject block;                  // ���� ��� ������
    public Vector2 startPosition = new Vector2(0, 0); // �׸��� ���� ��ġ
    public float squareScale = 0.5f;              // ����� ũ�� ������
    public float everySquareOffset = 0.0f;        // �� ����� ������

    private Vector2 _offset = new Vector2(0, 0);  // ��� ���� ��ġ ������ ���� ����
    private List<GameObject> _blocks = new List<GameObject>(); // ������ ���� ��ϵ��� ����Ʈ

    // ���� �̺�Ʈ�� �޼��带 ����մϴ�.
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }
    // ���� �̺�Ʈ���� �޼��带 �����մϴ�.
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    void Start()
    {
        CreateGrid(); // �׸��带 �����մϴ�.
    }

    // �׸��带 �����ϴ� �޼���
    private void CreateGrid()
    {
        SpawnBlocks();        // ���� ��ϵ��� �����մϴ�.
        SetBlocksPositions(); // ������ ��ϵ��� ��ġ�� �����մϴ�.
    }

    // ���� ��ϵ��� �����ϴ� �޼���
    private void SpawnBlocks()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                // ���� ��� �������� �ν��Ͻ�ȭ�Ͽ� �����մϴ�.
                GameObject newBlock = Instantiate(block) as GameObject;
                newBlock.transform.SetParent(this.transform); // �׸��� ������Ʈ�� �θ�� ����
                newBlock.transform.localScale = new Vector3(squareScale, squareScale, squareScale); // ������ ����

                // �������� ���� Ÿ���� �����Ͽ� �̹��� ����
                ElementType randomElement = (ElementType)Random.Range(1, 5);
                //newBlock.GetComponent<Block>().SetBlockImage(randomElement);
                newBlock.GetComponent<Block>().SetElementType(randomElement);

                // ������ ���� ����� ����Ʈ�� �߰��մϴ�.
                _blocks.Add(newBlock);
            }
        }
    }

    public ElementType GetElementTypeAt(int row, int column)
    {
        // �׸����� �ش� ��ġ�� �ִ� ����� ã��, �� ����� ElementType�� ��ȯ�մϴ�.
        int index = row * columns + column; // 1���� ����Ʈ �ε��� ���
        if (index >= 0 && index < _blocks.Count)
        {
            Block block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                return block.elementType;
            }
        }
        return ElementType.None; // ��ȿ���� ���� ��ġ�� ��� None ��ȯ
    }

    // ������ ���� ��ϵ��� ��ġ�� �����ϴ� �޼���
    private void SetBlocksPositions()
    {
        int column_number = 0;    // ���� �� ��ȣ
        int row_number = 0;       // ���� �� ��ȣ
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f); // ��� ���� �߰� ���� ���� ����
        bool row_moved = false;   // ���� �̵��ߴ��� ����

        var square_rect = _blocks[0].GetComponent<RectTransform>(); // ����� RectTransform ��������

        // ��� ���� ������ ���
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _blocks)
        {
            // �� ��ȣ�� �ִ� �� ���� ������ ���� ������Ű�� �� ��ȣ�� �ʱ�ȭ�մϴ�.
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            // ����� X, Y ��ġ �������� ����մϴ�.
            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            // Ư�� ������ �߰� ������ �߰��Ͽ� ��ϵ��� �׷�ȭ�մϴ�.
            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            // Ư�� �ึ�� �߰� ������ �߰��Ͽ� ��ϵ��� �׷�ȭ�մϴ�.
            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            // ����� ��ġ�� �����մϴ�.
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset);

            square.GetComponent<RectTransform>().localPosition = new Vector3(
                startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset, 0.0f);

            column_number++; // �� ��ȣ�� ������ŵ�ϴ�.
        }
    }

    public void UpdateGridState(int row, int column, ElementType elementType)
    {
        int index = row * columns + column; // �ε��� ���
        if (index >= 0 && index < _blocks.Count)
        {
            Block block = _blocks[index].GetComponent<Block>();
            if (block != null)
            {
                block.elementType = elementType; // ���ο� ElementType���� ������Ʈ
                block.SetBlockImage(elementType); // �̹��� ������Ʈ
            }
        }
    }

    // ���� �̺�Ʈ�� ���� ����� Ȱ��ȭ�ϴ� �޼���
    private void CheckIfShapeCanBePlaced()
    {
        foreach (var square in _blocks)
        {
            var block = square.GetComponent<Block>();

            // ��� ������ ����̸� Ȱ��ȭ�մϴ�.
            if (block.CanWeUseThisSquare() == true)
            {
                block.ActivateSquare();
            }
        }
    }

}
