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
        serializedObject.Update(); // 직렬화된 객체를 업데이트
        ClearBoardButton();        // "Clear Board" 버튼을 그립니다.
        RandomizeBoardButton();    // "Randomize Board" 버튼을 그립니다.
        SetAllToRandomButton();    // "Set All To Random" 버튼을 그립니다.
        EditorGUILayout.Space();   // 버튼 사이에 공백을 추가

        DrawColumnsInputFields();  // 열과 행 입력 필드를 그립니다.
        EditorGUILayout.Space();   // 공백 추가

        // 보드가 생성되고, 열과 행이 유효할 때만 보드 테이블을 그립니다.
        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();      // 보드 테이블을 그립니다.
        }

        serializedObject.ApplyModifiedProperties(); // 변경된 속성을 적용

        // GUI가 변경되었다면 객체를 Dirty 상태로 표시하여 저장되도록 합니다.
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    // "Clear Board" 버튼을 그리는 메서드
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board")) // 버튼이 클릭되었는지 확인
        {
            ShapeDataInstance.Clear();       // 보드를 초기화
        }
    }

    // "Randomize Board" 버튼을 그리는 메서드
    private void RandomizeBoardButton()
    {
        if (GUILayout.Button("Randomize Board")) // 버튼이 클릭되었는지 확인
        {
            ShapeDataInstance.RandomizeBoard(); // 보드를 랜덤화
        }
    }

    // "Set All To Random" 버튼을 그리는 메서드
    private void SetAllToRandomButton()
    {
        if (GUILayout.Button("Set All To Random")) // 버튼이 클릭되었는지 확인
        {
            ShapeDataInstance.SetAllToRandom(); // 모든 원소를 Random으로 설정
        }
    }

    //   Է ʵ带 ׸ ޼
    private void DrawColumnsInputFields()
    {
        var columnsTemp = ShapeDataInstance.columns; //    
        var rowsTemp = ShapeDataInstance.rows;       //    

        //    Է¹޽ϴ.
        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        // ̳   Ǿ ȿ ̸ ο 带 
        if ((ShapeDataInstance.columns != columnsTemp || ShapeDataInstance.rows != rowsTemp) &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard(); // ο 带 
        }
    }

    //  ̺ ׸ ޼
    private void DrawBoardTable()
    {
        // ̺ Ÿ մϴ.
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10); //   
        tableStyle.margin.left = 32;                         //   

        //  ÷ Ÿ մϴ.
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;                   //  ʺ 
        headerColumnStyle.alignment = TextAnchor.MiddleCenter; //  

        //  Ÿ մϴ.
        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;                           //   
        rowStyle.alignment = TextAnchor.MiddleCenter;        //  

        //  ʵ Ÿ մϴ.
        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;       // Ȱȭ  
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;    // Ȱȭ  

        //    ݺմϴ.
        for (var row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle); //  

            //    ݺմϴ.
            for (var column = 0; column < ShapeDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle); //  
                                                           // EnumPopup Ͽ  Ÿ 
                var elementType = (ElementType)EditorGUILayout.EnumPopup(ShapeDataInstance.board[row].colum[column]);
                ShapeDataInstance.board[row].colum[column] = elementType;

                EditorGUILayout.EndHorizontal(); //  
            }
            EditorGUILayout.EndHorizontal(); //  
        }
    }
}
