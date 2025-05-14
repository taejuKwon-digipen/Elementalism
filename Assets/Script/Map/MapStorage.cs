using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
using UnityEngine;

public class MapStorage : MonoBehaviour
{
    public static MapStorage Instance { get; private set; }

    public List<ShapeData> shapesForNextBattle;
    public List<ShapeData> allPossibleShapes; // Inspector에서 할당할 모든 ShapeData 목록
    // 여기에 다음 전투에 등장할 몬스터 관련 데이터도 추가할 수 있습니다.
    // public List<EnemyType> enemyTypesForNextBattle; // 예시

    private ShapeData lastUsedShape = null; // 마지막으로 사용된 ShapeData

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            shapesForNextBattle = new List<ShapeData>();
            // allPossibleShapes는 Inspector를 통해 할당되거나, 필요시 여기서 로드할 수 있습니다.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 이 메서드는 외부에서 특정 Shape 리스트를 설정하거나,
    // 파라미터로 null 또는 빈 리스트를 받으면 allPossibleShapes에서 랜덤하게 하나를 선택합니다.
    public void SetShapesForNextBattle(List<ShapeData> shapesToSet)
    {
        if (shapesForNextBattle == null) // Awake에서 초기화하지만, 안전을 위해 한번 더 체크
        {
            shapesForNextBattle = new List<ShapeData>();
        }
        shapesForNextBattle.Clear(); // 이전 데이터 클리어

        if (shapesToSet != null && shapesToSet.Count > 0)
        {
            shapesForNextBattle.AddRange(shapesToSet);
            if (shapesForNextBattle.Count > 0) // 실제 추가된 shape이 있을 때만 lastUsedShape 업데이트
            {
                lastUsedShape = shapesForNextBattle.LastOrDefault(); // 지정된 리스트의 마지막 것을 lastUsedShape으로 간주 (선택적)
            }
            Debug.Log($"[MapStorage] 다음 전투를 위한 Shape 설정 (지정됨): {shapesForNextBattle.Count}개");
        }
        else if (allPossibleShapes != null && allPossibleShapes.Count > 0)
        {
            List<ShapeData> availableShapes = allPossibleShapes.Where(shape => shape != lastUsedShape && shape != null).ToList();

            if (availableShapes.Count == 0) // 사용 가능한 다른 Shape이 없으면 (lastUsedShape밖에 없거나, 모두 null일 경우)
            {
                availableShapes = allPossibleShapes.Where(shape => shape != null).ToList(); // null 아닌 전체 목록에서 다시 선택
                if (availableShapes.Count == 0) // 그래도 없으면 (allPossibleShapes가 모두 null)
                {
                    Debug.LogWarning("[MapStorage] allPossibleShapes 목록에 유효한 Shape이 없습니다.");
                    return; // 더 이상 진행 불가
                }
            }

            int randomIndex = Random.Range(0, availableShapes.Count);
            ShapeData randomShape = availableShapes[randomIndex];
            
            shapesForNextBattle.Add(randomShape);
            lastUsedShape = randomShape; // 선택된 Shape을 마지막 사용으로 기록
            Debug.Log($"[MapStorage] 다음 전투를 위한 Shape 설정 (랜덤 선택됨): {randomShape.name}, 이전 사용: {(lastUsedShape?.name ?? "없음")}");
        }
        else
        {
            Debug.LogWarning("[MapStorage] 설정할 Shape이 없거나 allPossibleShapes 목록이 비어있습니다. shapesForNextBattle는 비어있게 됩니다.");
            // shapesForNextBattle는 이미 위에서 Clear() 되었으므로, 이 경우 비어있게 됩니다.
        }

        // 로그 상세화 (선택 사항)
        if (shapesForNextBattle.Count > 0) {
            foreach (var shape in shapesForNextBattle)
            {
                if (shape != null)
                    Debug.Log($" - 전투 준비 Shape: {(string.IsNullOrEmpty(shape.name) ? "Unnamed ShapeData" : shape.name)}");
                else
                    Debug.Log(" - 전투 준비 Shape: null (in shapesForNextBattle list)");
            }
        }
    }

    public List<ShapeData> GetShapesForBattle()
    {
        if (shapesForNextBattle != null && shapesForNextBattle.Count > 0)
        {
            Debug.Log($"[MapStorage] 전투를 위한 Shape 가져옴: {shapesForNextBattle.Count}개");
            return new List<ShapeData>(shapesForNextBattle); // 외부 변경 방지를 위해 복사본 반환
        }
        Debug.LogWarning("[MapStorage] 전투를 위한 Shape이 설정되지 않았거나 비어있습니다.");
        return null; // 또는 빈 리스트 반환: return new List<ShapeData>();
    }

    public void ClearShapesForBattle()
    {
        if (shapesForNextBattle != null)
        {
            shapesForNextBattle.Clear();
        }
        Debug.Log("[MapStorage] 전투 Shape 정보 초기화됨.");
    }
}