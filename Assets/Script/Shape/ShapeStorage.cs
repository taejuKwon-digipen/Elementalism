// 이 스크립트는 게임에서 사용할 Shape(퍼즐 조각)들을 저장하고 관리합니다.
// ShapeStorage 클래스는 ShapeData와 Shape 인스턴스들의 리스트를 가지고 있으며,
// 게임 시작 시 각 Shape에 랜덤한 ShapeData를 할당하여 퍼즐 조각을 생성합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData; // 생성 가능한 ShapeData의 리스트
    public List<Shape> shapeList;     // 게임 내에 배치될 Shape 인스턴스들의 리스트

    private List<ShapeData> initialShapeDataList = new List<ShapeData>();

    private void OnEnable() 
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }
    private void OnDisable() 
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }
    void Start()
    {
        // 초기 Shape 데이터 저장
        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            var selectedShapeData = shapeData[shapeIndex];
            initialShapeDataList.Add(selectedShapeData);
            shape.CreateShape(selectedShapeData);
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach(var shape in shapeList) 
        {
            if(shape.IsSelected() && shape.IsAnyOfShapeSquareActive())
                return shape;
        }
        
        // 에러 대신 경고 메시지로 변경
        Debug.LogWarning("No shape is currently selected");
        return null;
    }

    private void RequestNewShapes()
    {
        // 이전 Shape 데이터 저장
        List<ShapeData> previousShapes = new List<ShapeData>();
        foreach (var shape in shapeList)
        {
            if (shape.CurrentShapeData != null)
            {
                previousShapes.Add(shape.CurrentShapeData);
            }
        }

        // 새로운 Shape 생성 시 이전과 다른 모양 선택
        foreach (var shape in shapeList)
        {
            var availableShapes = shapeData.Where(s => !previousShapes.Contains(s)).ToList();
            if (availableShapes.Count > 0)
            {
                var shapeIndex = UnityEngine.Random.Range(0, availableShapes.Count);
                shape.RequestNewShape(availableShapes[shapeIndex]);
            }
            else
            {
                // 모든 모양이 사용된 경우 랜덤 선택
                var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
                shape.RequestNewShape(shapeData[shapeIndex]);
            }
        }
    }

    public void ResetToInitialShapes()
    {
        // 저장된 초기 Shape 데이터로 복원
        for (int i = 0; i < shapeList.Count; i++)
        {
            if (i < initialShapeDataList.Count)
            {
                shapeList[i].CreateShape(initialShapeDataList[i]);
            }
        }
    }

}
