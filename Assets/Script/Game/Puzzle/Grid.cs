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
    public GameObject elemental;                  // ���� ��� ������
    public Vector2 startPosition = new Vector2(0, 0); // �׸��� ���� ��ġ
    public float squareScale = 0.5f;              // ����� ũ�� ������
    public float everySquareOffset = 0.0f;        // �� ����� ������

    private Vector2 _offset = new Vector2(0, 0);  // ��� ���� ��ġ ������ ���� ����
    private List<GameObject> _elementals = new List<GameObject>(); // ������ ���� ��ϵ��� ����Ʈ

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
        SpawnElementals();        // ���� ��ϵ��� �����մϴ�.
        SetElementalsPositions(); // ������ ��ϵ��� ��ġ�� �����մϴ�.
    }

    // ���� ��ϵ��� �����ϴ� �޼���
    private void SpawnElementals()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                // ���� ��� �������� �ν��Ͻ�ȭ�Ͽ� �����մϴ�.
                GameObject newElemental = Instantiate(elemental) as GameObject;
                newElemental.transform.SetParent(this.transform); // �׸��� ������Ʈ�� �θ�� ����
                newElemental.transform.localScale = new Vector3(squareScale, squareScale, squareScale); // ������ ����

                // �������� ���� Ÿ���� �����Ͽ� �̹��� ����
                ElementType randomElement = (ElementType)Random.Range(1, 5);
                newElemental.GetComponent<Elemental>().SetRandomElementImage(randomElement);

                // ������ ���� ����� ����Ʈ�� �߰��մϴ�.
                _elementals.Add(newElemental);
            }
        }
    }

    // ������ ���� ��ϵ��� ��ġ�� �����ϴ� �޼���
    private void SetElementalsPositions()
    {
        int column_number = 0;    // ���� �� ��ȣ
        int row_number = 0;       // ���� �� ��ȣ
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f); // ��� ���� �߰� ���� ���� ����
        bool row_moved = false;   // ���� �̵��ߴ��� ����

        var square_rect = _elementals[0].GetComponent<RectTransform>(); // ����� RectTransform ��������

        // ��� ���� ������ ���
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _elementals)
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

    // ���� �̺�Ʈ�� ���� ����� Ȱ��ȭ�ϴ� �޼���
    private void CheckIfShapeCanBePlaced()
    {
        foreach (var square in _elementals)
        {
            var elemental = square.GetComponent<Elemental>();

            // ��� ������ ����̸� Ȱ��ȭ�մϴ�.
            if (elemental.CanWeUseThisSquare() == true)
            {
                elemental.ActivateSquare();
            }
        }
    }
}
