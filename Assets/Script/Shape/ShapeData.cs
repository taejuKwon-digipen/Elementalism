// 이 스크립트는 퍼즐 조각의 모양과 구조를 정의하는 ShapeData 클래스입니다.
// 퍼즐 조각을 구성하는 행(Row)과 열(Column)의 데이터를 관리하며,
// 새로운 보드를 생성하거나 초기화하는 기능을 제공합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // Unity 에디터에서 ScriptableObject를 생성할 수 있게 해주는 어트리뷰트
[System.Serializable]
public class ShapeData : ScriptableObject
{
    // 퍼즐 조각의 한 행을 나타내는 클래스
    [System.Serializable]
    public class Row
    {
        public ElementType[] colum; // 각 열의 타입을 저장하는 배열
        private int _size = 0; // 행의 열 수를 저장하는 변수

        public Row() { } // 기본 생성자

        public Row(int size)
        {
            CreateRow(size); // 주어진 크기로 행을 생성
        }

        // 행을 생성하고 초기화하는 메서드
        public void CreateRow(int size)
        {
            _size = size;
            colum = new ElementType[size]; // 열 배열을 초기화
            ClearRow(); // 행의 모든 열을 비활성화
        }

        // 행의 모든 열을 비활성화하는 메서드
        public void ClearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                colum[i] = ElementType.None; // None으로 설정
            }
        }
    }

    public int columns = 0; // 퍼즐 조각의 열 수
    public int rows = 0;    // 퍼즐 조각의 행 수
    public Row[] board;     // 퍼즐 조각의 전체 보드 데이터를 저장하는 배열

    // 보드의 모든 행을 초기화하여 퍼즐 조각을 비활성화하는 메서드
    public void Clear()
    {
        for (var i = 0; i < rows; i++)
        {
            board[i].ClearRow(); // 각 행의 열을 초기화
        }
    }

    // 현재 설정된 행과 열 수에 따라 새로운 보드를 생성하는 메서드
    public void CreateNewBoard()
    {
        board = new Row[rows]; // 행 배열을 초기화

        for (var i = 0; i < rows; i++)
        {
            board[i] = new Row(columns); // 각 행에 열 수를 설정하여 생성
        }
    }
}
