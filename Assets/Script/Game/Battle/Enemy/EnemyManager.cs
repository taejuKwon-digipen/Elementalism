using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpawnAvailable
{
    public GameObject SpawnPoint;
    public bool IsAvailable;
}

public class EnemyManager : MonoBehaviour
{
    public GameObject canvas;
    public List<GameObject> spawnableEnemies;
    public List<GameObject> spawnPoints;

    [SerializeField]
    private List<GameObject> onFieldEntities;
    private readonly int[] numbers = {3};//{ 1, 2, 2, 3, 3, 1 }; // Number of enemies to spawn at the next wave.
    private readonly List<SpawnAvailable> areSpawnPointsAvailable = new();

    public bool isEnemyTurn = false;

    public GameObject hubDmgTextfab;

    private void Awake() {
        foreach (var item in spawnPoints)
        {
            SpawnAvailable spawn = new()
            {
                SpawnPoint = item,
                IsAvailable = true
            };
            areSpawnPointsAvailable.Add(spawn);
        }
    }

    private void Start()
    {
        SpawnNewEnemies();
    }

    public bool IsAllEnemiesDefeated()
    {
        return onFieldEntities.Count == 0;
    }

    /**
      * <summary>Spawn a random amount of enemies when all enemies are defeated</summary>

      */
    //private void SpawnNewEnemies()
    //{
    //    if (onFieldEntities.Count != 0)
    //        return;
    //    ResetSpawnPoints();
    //    int numberToSpawn = numbers[UnityEngine.Random.Range(0, numbers.Length)];
    //    for (int i = 0; i < numberToSpawn; i += 1) {
    //        var entityToSpawn = spawnableEnemies[UnityEngine.Random.Range(0, spawnableEnemies.Count)];
    //        var spawn = GetNewSpawn();
    //        var newEntity = Instantiate(entityToSpawn, spawn, Quaternion.identity, canvas.transform);
    //        newEntity.GetComponentInChildren<ImageClickHandler>().Canva = canvas;
    //        newEntity.GetComponentInChildren<Enemy>().SetEnemyManager(this);
    //        onFieldEntities.Add(newEntity);
    //    }
    //    onFieldEntities.Sort((a, b) => Convert.ToInt32(a.GetComponent<Transform>().position.x.CompareTo(b.GetComponent<Transform>().position.x)));
    //    onFieldEntities[0].GetComponentInChildren<Enemy>().NotifyClickToLockManager();
    //}

    private void SpawnNewEnemies()
    {
        if (onFieldEntities.Count != 0)
            return;

        ResetSpawnPoints();

        List<GameObject> enemiesFromMapStorage = MapStorage.Instance?.GetEnemiesForBattle();

        if (enemiesFromMapStorage != null && enemiesFromMapStorage.Count > 0)
        {
            Debug.Log("[EnemyManager] MapStorage에서 적 정보를 가져와 스폰합니다.");
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (i < enemiesFromMapStorage.Count && enemiesFromMapStorage[i] != null)
                {
                    GameObject entityToSpawn = enemiesFromMapStorage[i];
                    Transform spawnPointTransform = spawnPoints[i].transform;

                    var newEntity = Instantiate(entityToSpawn, spawnPointTransform.position, Quaternion.identity, canvas.transform);
                    var enemyComponent = newEntity.GetComponentInChildren<Enemy>();
                    newEntity.GetComponentInChildren<ImageClickHandler>().Canva = canvas;
                    enemyComponent.SetEnemyManager(this);
                    onFieldEntities.Add(newEntity);

                    if (hubDmgTextfab != null)
                    {
                        enemyComponent.SetDmgTextPrefab(hubDmgTextfab);
                    }
                    Debug.Log($"[EnemyManager] {entityToSpawn.name}을(를) 스폰 포인트 {i} ({spawnPointTransform.name})에 스폰했습니다.");
                }
                else
                {
                    Debug.Log($"[EnemyManager] 스폰 포인트 {i} ({spawnPoints[i].transform.name})에 지정된 적이 없거나 null입니다. 스킵합니다.");
                }
            }
        }
        else
        {
            Debug.Log("[EnemyManager] MapStorage에 적 정보가 없습니다. 기존 랜덤 스폰 로직을 실행합니다.");
            int numberToSpawn = numbers[UnityEngine.Random.Range(0, numbers.Length)];
            for (int i = 0; i < numberToSpawn; i++)
            {
                var entityToSpawn = spawnableEnemies[UnityEngine.Random.Range(0, spawnableEnemies.Count)];
                var spawnPosition = GetNewSpawn();
                var newEntity = Instantiate(entityToSpawn, spawnPosition, Quaternion.identity, canvas.transform);

                var enemyComponent = newEntity.GetComponentInChildren<Enemy>();
                newEntity.GetComponentInChildren<ImageClickHandler>().Canva = canvas;
                enemyComponent.SetEnemyManager(this);
                onFieldEntities.Add(newEntity);

                if (hubDmgTextfab != null)
                {
                    enemyComponent.SetDmgTextPrefab(hubDmgTextfab);
                }
            }
        }

        if (onFieldEntities.Count > 0)
        {
            onFieldEntities.Sort((a, b) => Convert.ToInt32(a.GetComponent<Transform>().position.x.CompareTo(b.GetComponent<Transform>().position.x)));
            onFieldEntities[0].GetComponentInChildren<Enemy>().NotifyClickToLockManager();
        }
        else
        {
            Debug.LogWarning("[EnemyManager] 스폰된 적이 없습니다.");
            // 예: 모든 적이 스폰되지 않았을 경우 게임 매니저에게 알리거나 다음 단계로 진행
            // GameManager.Instance?.CheckBattleEnd(); // GameManager에 해당 메서드가 없으므로 주석 처리합니다.
        }
    }
    /**
      * <summary>Set together spawn point and their availability</summary>
      */
    private Vector3 GetNewSpawn()
    {
        SpawnAvailable result = new()
        {
            IsAvailable = false
        };
        while (result.IsAvailable == false) {
            int index = UnityEngine.Random.Range(0, areSpawnPointsAvailable.Count);
            result = areSpawnPointsAvailable[index];
        }
        result.IsAvailable = false;
        return result.SpawnPoint.transform.position;
    }

    /**
      * <summary>Reset spawn point availability</summary>
      */
    private void ResetSpawnPoints()
    {
        for (int i = 0; i < areSpawnPointsAvailable.Count; i += 1)
        {
            var item = areSpawnPointsAvailable[i];
            item.IsAvailable = true;
        }
        return;
    }

    /**
      * <summary>Get all enemy entity on the field</summary>
      */
    public List<GameObject> GetOnFieldEntities()
    {
        return onFieldEntities;
    }

    public void StartEnemyTurn()
    {
        isEnemyTurn = true;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i < onFieldEntities.Count; i += 1) {
            if (i >= onFieldEntities.Count) {
                continue;
            }
            Enemy enemy = onFieldEntities[i].GetComponentInChildren<Enemy>();
            if (enemy.HP <= 0)
                continue;
            yield return StartCoroutine(enemy.Turn());
        }
        isEnemyTurn = false;
    }

    public void DestroyEnemy(GameObject enemy)
    {
        onFieldEntities.Remove(enemy);
    }
}
