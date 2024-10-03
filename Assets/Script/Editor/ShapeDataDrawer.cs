// 이 스크립트는 Unity 에디터에서 `ShapeData` 객체를 위한 커스텀 인스펙터를 구현합니다.
// 개발자가 인스펙터를 통해 퍼즐 조각의 형태를 시각적으로 편집할 수 있도록 합니다.
// 행과 열의 수를 설정하고, 각 셀을 토글하여 퍼즐 모양을 디자인할 수 있습니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData), false)] // ShapeData에 대한 커스텀 에디터로 지정
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    // 현재 편집 중인 ShapeData 인스턴스를 가져옵니다.
    private ShapeData ShapeDataInstance => target as ShapeData;

    // 인스펙터 GUI를 그리는 메서드
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // 직렬화된 객체를 업데이트
        ClearBoardButton();        // "Clear Board" 버튼을 그립니다.
        EditorGUILayout.Space();   // 공간을 추가하여 레이아웃을 정리

        DrawColumnsInputFields();  // 행과 열 입력 필드를 그립니다.
        EditorGUILayout.Space();   // 공간을 추가

        // 보드가 존재하고, 열과 행의 수가 유효하면 보드 테이블을 그립니다.
        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();      // 보드의 각 셀을 토글 버튼으로 표시
        }

        serializedObject.ApplyModifiedProperties(); // 변경된 속성을 적용

        // GUI에 변경이 발생하면 객체를 Dirty 상태로 설정하여 저장되도록 합니다.
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    // "Clear Board" 버튼을 생성하는 메서드
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board")) // 버튼이 클릭되었는지 확인
        {
            ShapeDataInstance.Clear();       // 보드를 초기화
        }
    }

    // 행과 열 입력 필드를 그리는 메서드
    private void DrawColumnsInputFields()
    {
        var columnsTemp = ShapeDataInstance.columns; // 기존 열 수를 저장
        var rowsTemp = ShapeDataInstance.rows;       // 기존 행 수를 저장

        // 열과 행의 수를 입력받습니다.
        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        // 열이나 행 수가 변경되었고 유효한 값이면 새로운 보드를 생성
        if ((ShapeDataInstance.columns != columnsTemp || ShapeDataInstance.rows != rowsTemp) &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard(); // 새로운 보드를 생성
        }
    }

    // 보드 테이블을 그리는 메서드
    private void DrawBoardTable()
    {
        // 테이블의 스타일을 설정합니다.
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10); // 내부 여백 설정
        tableStyle.margin.left = 32;                         // 왼쪽 여백 설정

        // 헤더 컬럼의 스타일을 설정합니다.
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;                   // 고정 너비 설정
        headerColumnStyle.alignment = TextAnchor.MiddleCenter; // 가운데 정렬

        // 행의 스타일을 설정합니다.
        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;                           // 고정 높이 설정
        rowStyle.alignment = TextAnchor.MiddleCenter;        // 가운데 정렬

        // 데이터 필드의 스타일을 설정합니다.
        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;       // 비활성화 상태 배경색
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;    // 활성화 상태 배경색

        // 보드의 각 행을 반복합니다.
        for (var row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle); // 행 시작

            // 각 행의 열을 반복합니다.
            for (var column = 0; column < ShapeDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle); // 열 시작
                // 토글 버튼을 생성하여 셀의 활성화 여부를 표시하고 변경할 수 있게 합니다.
                var data = EditorGUILayout.Toggle(ShapeDataInstance.board[row].colum[column], dataFieldStyle);
                ShapeDataInstance.board[row].colum[column] = data; // 변경된 값을 보드 데이터에 적용
                EditorGUILayout.EndHorizontal(); // 열 종료
            }
            EditorGUILayout.EndHorizontal(); // 행 종료
        }
    }
}
