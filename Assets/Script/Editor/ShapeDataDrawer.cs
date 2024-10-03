// �� ��ũ��Ʈ�� Unity �����Ϳ��� `ShapeData` ��ü�� ���� Ŀ���� �ν����͸� �����մϴ�.
// �����ڰ� �ν����͸� ���� ���� ������ ���¸� �ð������� ������ �� �ֵ��� �մϴ�.
// ��� ���� ���� �����ϰ�, �� ���� ����Ͽ� ���� ����� �������� �� �ֽ��ϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData), false)] // ShapeData�� ���� Ŀ���� �����ͷ� ����
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    // ���� ���� ���� ShapeData �ν��Ͻ��� �����ɴϴ�.
    private ShapeData ShapeDataInstance => target as ShapeData;

    // �ν����� GUI�� �׸��� �޼���
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // ����ȭ�� ��ü�� ������Ʈ
        ClearBoardButton();        // "Clear Board" ��ư�� �׸��ϴ�.
        EditorGUILayout.Space();   // ������ �߰��Ͽ� ���̾ƿ��� ����

        DrawColumnsInputFields();  // ��� �� �Է� �ʵ带 �׸��ϴ�.
        EditorGUILayout.Space();   // ������ �߰�

        // ���尡 �����ϰ�, ���� ���� ���� ��ȿ�ϸ� ���� ���̺��� �׸��ϴ�.
        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();      // ������ �� ���� ��� ��ư���� ǥ��
        }

        serializedObject.ApplyModifiedProperties(); // ����� �Ӽ��� ����

        // GUI�� ������ �߻��ϸ� ��ü�� Dirty ���·� �����Ͽ� ����ǵ��� �մϴ�.
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    // "Clear Board" ��ư�� �����ϴ� �޼���
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board")) // ��ư�� Ŭ���Ǿ����� Ȯ��
        {
            ShapeDataInstance.Clear();       // ���带 �ʱ�ȭ
        }
    }

    // ��� �� �Է� �ʵ带 �׸��� �޼���
    private void DrawColumnsInputFields()
    {
        var columnsTemp = ShapeDataInstance.columns; // ���� �� ���� ����
        var rowsTemp = ShapeDataInstance.rows;       // ���� �� ���� ����

        // ���� ���� ���� �Է¹޽��ϴ�.
        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        // ���̳� �� ���� ����Ǿ��� ��ȿ�� ���̸� ���ο� ���带 ����
        if ((ShapeDataInstance.columns != columnsTemp || ShapeDataInstance.rows != rowsTemp) &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard(); // ���ο� ���带 ����
        }
    }

    // ���� ���̺��� �׸��� �޼���
    private void DrawBoardTable()
    {
        // ���̺��� ��Ÿ���� �����մϴ�.
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10); // ���� ���� ����
        tableStyle.margin.left = 32;                         // ���� ���� ����

        // ��� �÷��� ��Ÿ���� �����մϴ�.
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;                   // ���� �ʺ� ����
        headerColumnStyle.alignment = TextAnchor.MiddleCenter; // ��� ����

        // ���� ��Ÿ���� �����մϴ�.
        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;                           // ���� ���� ����
        rowStyle.alignment = TextAnchor.MiddleCenter;        // ��� ����

        // ������ �ʵ��� ��Ÿ���� �����մϴ�.
        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;       // ��Ȱ��ȭ ���� ����
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;    // Ȱ��ȭ ���� ����

        // ������ �� ���� �ݺ��մϴ�.
        for (var row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle); // �� ����

            // �� ���� ���� �ݺ��մϴ�.
            for (var column = 0; column < ShapeDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle); // �� ����
                // ��� ��ư�� �����Ͽ� ���� Ȱ��ȭ ���θ� ǥ���ϰ� ������ �� �ְ� �մϴ�.
                var data = EditorGUILayout.Toggle(ShapeDataInstance.board[row].colum[column], dataFieldStyle);
                ShapeDataInstance.board[row].colum[column] = data; // ����� ���� ���� �����Ϳ� ����
                EditorGUILayout.EndHorizontal(); // �� ����
            }
            EditorGUILayout.EndHorizontal(); // �� ����
        }
    }
}
