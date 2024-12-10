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
    private readonly int[] numbers = {4};//{ 1, 2, 2, 3, 3, 1 }; // Number of enemies to spawn at the next wave.
    private readonly List<SpawnAvailable> areSpawnPointsAvailable = new();

    public bool isEnemyTurn = false;

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

    private void Update()
    {
        SpawnNewEnemies();
    }

    /**
      * <summary>Spawn a random amount of enemies when all enemies are defeated</summary>
      */
    private void SpawnNewEnemies()
    {
        if (onFieldEntities.Count != 0)
            return;
        ResetSpawnPoints();
        int numberToSpawn = numbers[UnityEngine.Random.Range(0, numbers.Length)];
        for (int i = 0; i < numberToSpawn; i += 1) {
            var entityToSpawn = spawnableEnemies[UnityEngine.Random.Range(0, spawnableEnemies.Count)];
            var spawn = GetNewSpawn();
            var newEntity = Instantiate(entityToSpawn, spawn, Quaternion.identity, canvas.transform);
            newEntity.GetComponentInChildren<ImageClickHandler>().Canva = canvas;
            newEntity.GetComponentInChildren<Enemy>().SetEnemyManager(this);
            onFieldEntities.Add(newEntity);
        }
        onFieldEntities.Sort((a, b) => Convert.ToInt32(a.GetComponent<Transform>().position.x.CompareTo(b.GetComponent<Transform>().position.x)));
        onFieldEntities[0].GetComponentInChildren<Enemy>().NotifyClickToLockManager();
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
