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

    private List<ShapeData> currentShapeDataList = new List<ShapeData>(); // 현재 각 Shape에 할당된 (변환될 수 있는) ShapeData 저장

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
        currentShapeDataList.Clear(); // 시작 시 초기화
        // 초기 Shape 데이터 저장 및 생성
        for (int i = 0; i < shapeList.Count; i++)
        {
            var originalShapeData = shapeData[UnityEngine.Random.Range(0, shapeData.Count)];
            ShapeData runtimeShapeData = ApplyGameManagerRulesToShapeData(originalShapeData);
            currentShapeDataList.Add(runtimeShapeData); // 변환된 데이터를 현재 데이터로 저장
            shapeList[i].CreateShape(runtimeShapeData);
        }
    }

    // GameManager 규칙을 ShapeData에 적용하는 헬퍼 메서드
    private ShapeData ApplyGameManagerRulesToShapeData(ShapeData originalData)
    {
        if (originalData == null) return null;
        ShapeData clonedData = originalData.Clone(); // 원본 에셋 수정을 피하기 위해 복제

        if (GameManager.Instance != null && clonedData.board != null)
        {
            for (int r = 0; r < clonedData.rows; r++)
            {
                for (int c = 0; c < clonedData.columns; c++)
                {
                    clonedData.board[r].colum[c] = 
                        GameManager.Instance.GetModifiedElementType(clonedData.board[r].colum[c]);
                }
            }
        }
        return clonedData;
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
        List<ShapeData> previousOriginalShapes = new List<ShapeData>();
        // currentShapeDataList에는 이미 변환된 데이터가 들어있으므로, 원본을 추적하려면 다른 방법이 필요하거나,
        // 혹은 단순히 인덱스를 기반으로 중복을 피하는 로직을 사용해야 함.
        // 여기서는 간단히 중복을 허용하고 랜덤으로 선택 후 변환하는 방식으로 변경.

        for (int i = 0; i < shapeList.Count; i++)
        {
            var originalNewShapeData = shapeData[UnityEngine.Random.Range(0, shapeData.Count)];
            ShapeData runtimeNewShapeData = ApplyGameManagerRulesToShapeData(originalNewShapeData);
            currentShapeDataList[i] = runtimeNewShapeData; // 현재 사용 데이터 업데이트
            shapeList[i].RequestNewShape(runtimeNewShapeData);
        }
    }

    public void ResetToInitialShapes() // 이 메서드는 initialShapeDataList가 제거됨에 따라 재검토 필요
    {
        // 만약 "초기 모양"을 저장하고 싶다면, Start 시점의 변환된 runtimeShapeData를 별도 리스트에 저장해야함
        // 현재는 Start에서 생성된 것과 동일한 로직으로 "새로운 초기 모양"을 만듬.
        Debug.LogWarning("ResetToInitialShapes는 현재 새로운 랜덤 모양으로 초기화합니다. 수정이 필요할 수 있습니다.");
        Start(); // 간단히 Start 로직을 재호출하여 새로운 랜덤 모양들로 설정
    }

    public void ResetToCurrentShapes()
    {
        // 현재 currentShapeDataList에 저장된 데이터로 복원
        for (int i = 0; i < shapeList.Count; i++)
        {
            if (i < currentShapeDataList.Count && currentShapeDataList[i] != null)
            {
                shapeList[i].CreateShape(currentShapeDataList[i]);
            }
            else if (i < shapeData.Count) // Fallback: current 데이터가 없다면 원본에서 새로 생성
            {
                 var originalShapeData = shapeData[UnityEngine.Random.Range(0, shapeData.Count)];
                 ShapeData runtimeShapeData = ApplyGameManagerRulesToShapeData(originalShapeData);
                 if(i < currentShapeDataList.Count) currentShapeDataList[i] = runtimeShapeData;
                 else currentShapeDataList.Add(runtimeShapeData);
                 shapeList[i].CreateShape(runtimeShapeData);
            }
        }
    }
}
