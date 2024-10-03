// �� ��ũ��Ʈ�� ���� ������ ���� ������ �����ϴ� ShapeData Ŭ�����Դϴ�.
// ���� ������ �����ϴ� ��(Row)�� ��(Column)�� �����͸� �����ϸ�,
// ���ο� ���带 �����ϰų� �ʱ�ȭ�ϴ� ����� �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // Unity �����Ϳ��� ScriptableObject�� ������ �� �ְ� ���ִ� ��Ʈ����Ʈ
[System.Serializable]
public class ShapeData : ScriptableObject
{
    // ���� ������ �� ���� ��Ÿ���� Ŭ����
    [System.Serializable]
    public class Row
    {
        public ElementType[] colum; // �� ���� Ÿ���� �����ϴ� �迭
        private int _size = 0; // ���� �� ���� �����ϴ� ����

        public Row() { } // �⺻ ������

        public Row(int size)
        {
            CreateRow(size); // �־��� ũ��� ���� ����
        }

        // ���� �����ϰ� �ʱ�ȭ�ϴ� �޼���
        public void CreateRow(int size)
        {
            _size = size;
            colum = new ElementType[size]; // �� �迭�� �ʱ�ȭ
            ClearRow(); // ���� ��� ���� ��Ȱ��ȭ
        }

        // ���� ��� ���� ��Ȱ��ȭ�ϴ� �޼���
        public void ClearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                colum[i] = ElementType.None; // None���� ����
            }
        }
    }

    public int columns = 0; // ���� ������ �� ��
    public int rows = 0;    // ���� ������ �� ��
    public Row[] board;     // ���� ������ ��ü ���� �����͸� �����ϴ� �迭

    // ������ ��� ���� �ʱ�ȭ�Ͽ� ���� ������ ��Ȱ��ȭ�ϴ� �޼���
    public void Clear()
    {
        for (var i = 0; i < rows; i++)
        {
            board[i].ClearRow(); // �� ���� ���� �ʱ�ȭ
        }
    }

    // ���� ������ ��� �� ���� ���� ���ο� ���带 �����ϴ� �޼���
    public void CreateNewBoard()
    {
        board = new Row[rows]; // �� �迭�� �ʱ�ȭ

        for (var i = 0; i < rows; i++)
        {
            board[i] = new Row(columns); // �� �࿡ �� ���� �����Ͽ� ����
        }
    }
}
