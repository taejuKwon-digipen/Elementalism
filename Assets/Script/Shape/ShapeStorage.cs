// 이 스크립트는 게임에서 사용할 Shape(퍼즐 조각)들을 저장하고 관리합니다.
// ShapeStorage 클래스는 ShapeData와 Shape 인스턴스들의 리스트를 가지고 있으며,
// 게임 시작 시 각 Shape에 랜덤한 ShapeData를 할당하여 퍼즐 조각을 생성합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData; // 생성 가능한 ShapeData의 리스트
    public List<Shape> shapeList;     // 게임 내에 배치될 Shape 인스턴스들의 리스트

    void Start()
    {
        // 게임이 시작될 때 각 Shape에 랜덤한 ShapeData를 할당하여 생성합니다.
        foreach (var shape in shapeList)
        {
            // shapeData 리스트에서 랜덤한 인덱스를 선택합니다.
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            // 선택된 ShapeData를 사용하여 Shape를 생성합니다.
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }
}
