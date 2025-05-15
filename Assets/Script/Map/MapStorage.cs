using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
using UnityEngine;

[System.Serializable]
public class ShapeSpecificBattleConfig
{
    [Tooltip("인스펙터에서 구분을 위한 이름")]
    public string configName = "New Battle Config";

    [Tooltip("이 전투 설정에 사용될 단일 ShapeData")]
    public ShapeData shapeForThisConfig;

    [Tooltip("각 스폰 포인트에 배치될 적 프리팹 리스트. 인덱스 0이 첫 번째 스폰 포인트입니다. 비워두려면 None(Game Object)으로 설정하세요.")]
    public List<GameObject> enemyPrefabsPerSpawnPoint; // List size should ideally match spawn points in EnemyManager
}

public class MapStorage : MonoBehaviour
{
    public static MapStorage Instance { get; private set; }

    [Header("전투 구성 목록")]
    [Tooltip("인스펙터에서 각 전투(스테이지, 웨이브 등)에 대한 Shape와 적 배치를 설정합니다.")]
    public List<ShapeSpecificBattleConfig> battleConfigurations;

    private ShapeData currentShapeForNextBattle;
    private List<GameObject> currentEnemiesForNextBattle;
    private int lastSelectedConfigIndex = -1; // 중복 방지 및 다음 랜덤 선택을 위한 인덱스

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentEnemiesForNextBattle = new List<GameObject>();
            // battleConfigurations는 인스펙터에서 채워집니다.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 이름으로 특정 전투 구성을 준비합니다.
    /// </summary>
    public void PrepareBattleConfiguration(string configName)
    {
        if (battleConfigurations == null || battleConfigurations.Count == 0)
        {
            Debug.LogError("[MapStorage] battleConfigurations 리스트가 비어있거나 null입니다! 전투 준비 불가.");
            currentShapeForNextBattle = null;
            currentEnemiesForNextBattle.Clear();
            lastSelectedConfigIndex = -1;
            return;
        }

        ShapeSpecificBattleConfig selectedConfig = battleConfigurations.FirstOrDefault(c => c.configName == configName);

        if (selectedConfig != null)
    {
            currentShapeForNextBattle = selectedConfig.shapeForThisConfig;
            currentEnemiesForNextBattle = new List<GameObject>(selectedConfig.enemyPrefabsPerSpawnPoint ?? new List<GameObject>());
            lastSelectedConfigIndex = battleConfigurations.IndexOf(selectedConfig);
            Debug.Log($"[MapStorage] 전투 구성 '{configName}' 준비 완료. Shape: {(currentShapeForNextBattle ? currentShapeForNextBattle.name : "없음")}, 적 {currentEnemiesForNextBattle.Count}마리.");
        }
        else
        {
            Debug.LogWarning($"[MapStorage] 이름 '{configName}'에 해당하는 전투 구성을 찾을 수 없습니다. 무작위 구성을 준비합니다.");
            PrepareRandomBattleConfiguration();
        }
    }

    /// <summary>
    /// 인덱스로 특정 전투 구성을 준비합니다.
    /// </summary>
    public void PrepareBattleConfiguration(int configIndex)
    {
        if (battleConfigurations == null || battleConfigurations.Count == 0)
        {
            Debug.LogError("[MapStorage] battleConfigurations 리스트가 비어있거나 null입니다! 전투 준비 불가.");
            currentShapeForNextBattle = null;
            currentEnemiesForNextBattle.Clear();
            lastSelectedConfigIndex = -1;
            return;
        }

        if (configIndex >= 0 && configIndex < battleConfigurations.Count)
        {
            ShapeSpecificBattleConfig selectedConfig = battleConfigurations[configIndex];
            currentShapeForNextBattle = selectedConfig.shapeForThisConfig;
            currentEnemiesForNextBattle = new List<GameObject>(selectedConfig.enemyPrefabsPerSpawnPoint ?? new List<GameObject>());
            lastSelectedConfigIndex = configIndex;
            Debug.Log($"[MapStorage] 전투 구성 (인덱스 {configIndex}) '{selectedConfig.configName}' 준비 완료. Shape: {(currentShapeForNextBattle ? currentShapeForNextBattle.name : "없음")}, 적 {currentEnemiesForNextBattle.Count}마리.");
        }
        else
        {
            Debug.LogWarning($"[MapStorage] 잘못된 인덱스 {configIndex}입니다. 무작위 구성을 준비합니다.");
            PrepareRandomBattleConfiguration();
        }
    }

    /// <summary>
    /// 사용 가능한 구성 중에서 무작위로 하나를 선택하여 준비합니다. 이전에 선택되지 않은 것을 우선합니다.
    /// </summary>
    public void PrepareRandomBattleConfiguration()
    {
        if (battleConfigurations == null || battleConfigurations.Count == 0)
        {
            Debug.LogError("[MapStorage] 전투 구성이 없어 무작위 선택이 불가능합니다.");
            currentShapeForNextBattle = null;
            currentEnemiesForNextBattle.Clear();
            lastSelectedConfigIndex = -1;
            return;
        }

        if (battleConfigurations.Count == 1) // 구성이 하나뿐이면 항상 그것을 선택
        {
            PrepareBattleConfiguration(0);
            return;
        }

        List<int> availableIndices = Enumerable.Range(0, battleConfigurations.Count).ToList();
        if (lastSelectedConfigIndex != -1) // 이전에 선택된 것이 있다면 제외 시도
        {
            availableIndices.Remove(lastSelectedConfigIndex);
        }
        
        if(availableIndices.Count == 0) // 모든 구성이 한번씩 선택되었거나, lastSelectedConfigIndex만 있었던 경우
        {
             availableIndices = Enumerable.Range(0, battleConfigurations.Count).ToList(); // 다시 전체 목록에서 선택
        }

        int randomIndex = Random.Range(0, availableIndices.Count);
        PrepareBattleConfiguration(availableIndices[randomIndex]);
        // lastSelectedConfigIndex는 PrepareBattleConfiguration(int) 내부에서 업데이트됨
        Debug.Log($"[MapStorage] 무작위 전투 구성 (인덱스 {lastSelectedConfigIndex}) '{battleConfigurations[lastSelectedConfigIndex].configName}' 준비 완료.");
        }


    /// <summary>
    /// 다음 전투에 사용될 Shape 리스트를 가져옵니다. (현재 구조에서는 주로 단일 ShapeData를 포함)
    /// </summary>
    public List<ShapeData> GetShapesForBattle()
    {
        if (currentShapeForNextBattle == null && (battleConfigurations != null && battleConfigurations.Count > 0))
        {
            Debug.LogWarning("[MapStorage] GetShapesForBattle: 현재 Shape이 준비되지 않아 무작위 구성을 준비합니다.");
            PrepareRandomBattleConfiguration();
        }

        if (currentShapeForNextBattle != null)
        {
            return new List<ShapeData> { currentShapeForNextBattle };
        }
        
        Debug.LogError("[MapStorage] GetShapesForBattle: 준비할 수 있는 ShapeData가 없습니다!");
        return new List<ShapeData>(); // 비어 있는 리스트 반환
    }

    /// <summary>
    /// 다음 전투에 사용될 적 프리팹 리스트를 가져옵니다.
    /// </summary>
    public List<GameObject> GetEnemiesForBattle()
    {
        // currentShapeForNextBattle이 null이면 currentEnemiesForNextBattle도 일관성 있게 설정되어야 함
        // GetShapesForBattle에서 PrepareRandomBattleConfiguration이 호출될 수 있으므로, 여기서 다시 호출할 필요는 없을 수 있음
        // 하지만 안전을 위해, shape은 있는데 enemy만 없는 경우를 대비하거나, 혹은 둘 다 없는 경우를 위해 체크
        if (currentShapeForNextBattle == null && (currentEnemiesForNextBattle == null || currentEnemiesForNextBattle.Count == 0) && 
            (battleConfigurations != null && battleConfigurations.Count > 0))
        {
            Debug.LogWarning("[MapStorage] GetEnemiesForBattle: 현재 전투 구성이 준비되지 않아 무작위 구성을 준비합니다.");
            PrepareRandomBattleConfiguration(); // Shape과 Enemy를 함께 설정
        }
        
        if (currentEnemiesForNextBattle != null)
        {
            return new List<GameObject>(currentEnemiesForNextBattle); // 방어적 복사본 반환
        }

        Debug.LogError("[MapStorage] GetEnemiesForBattle: 준비할 수 있는 적 리스트가 없습니다!");
        return new List<GameObject>(); // 비어 있는 리스트 반환
    }

    /// <summary>
    /// 현재 선택된 전투 구성을 초기화합니다.
    /// </summary>
    public void ClearBattleConfiguration()
    {
        currentShapeForNextBattle = null;
        currentEnemiesForNextBattle.Clear();
        lastSelectedConfigIndex = -1; // 마지막 선택 인덱스도 초기화
        Debug.Log("[MapStorage] 현재 전투 구성 정보가 초기화되었습니다.");
    }

    /// <summary>
    /// 외부에서 ShapeData 리스트와 Enemy 프리팹 리스트를 직접 받아 현재 전투 구성을 설정합니다.
    /// battleConfigurations에 저장되지는 않습니다.
    /// </summary>
    public void SetCurrentBattleManually(List<ShapeData> shapes, List<GameObject> enemies)
    {
        if (shapes != null && shapes.Count > 0)
        {
            currentShapeForNextBattle = shapes[0]; // 현재 구조는 단일 Shape을 가정
            if (shapes.Count > 1)
            {
                Debug.LogWarning("[MapStorage] SetCurrentBattleManually: 여러 개의 Shape이 제공되었지만, 현재 구조는 첫 번째 Shape만 사용합니다.");
            }
        }
        else
        {
            currentShapeForNextBattle = null;
        }
        currentEnemiesForNextBattle = new List<GameObject>(enemies ?? new List<GameObject>());
        lastSelectedConfigIndex = -1; // 특정 설정에서 온 것이 아니므로 인덱스 초기화
        Debug.Log($"[MapStorage] 전투 구성 수동 설정 완료. Shape: {(currentShapeForNextBattle ? currentShapeForNextBattle.name : "없음")}, 적 {currentEnemiesForNextBattle.Count}마리.");
    }
}