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
    private readonly int[] numbers = { 1, 2, 2, 3, 3, 4 }; // Number of enemies to spawn at the next wave.
    private readonly List<SpawnAvailable> areSpawnPointsAvailable = new();

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
        CleanUpList();
    }

    /**
      * <summary>Spawn a random amount of enemies when all enemies are defeated</summary>
      */
    private void SpawnNewEnemies()
    {
        if (onFieldEntities.Count != 0)
            return;
        ResetSpawnPoints();
        int numberToSpawn = numbers[Random.Range(0, numbers.Length)];
        for (int i = 0; i < numberToSpawn; i += 1) {
            var entityToSpawn = spawnableEnemies[0];
            var spawn = GetNewSpawn();
            var newEntity = Instantiate(entityToSpawn, spawn, Quaternion.identity, canvas.transform);
            newEntity.GetComponentInChildren<ImageClickHandler>().Canva = canvas;
            onFieldEntities.Add(newEntity);
        }
    }

    /**
      * <summary>Clean up all the null GameObject when enemy are dead, Maybe too expensive but there's not a lot of enemy to manage so</summary>
      */
    private void CleanUpList()
    {
        onFieldEntities.RemoveAll(item => item == null);
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
            int index = Random.Range(0, areSpawnPointsAvailable.Count);
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
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        foreach (var obj in onFieldEntities)
        {
            Enemy entity = obj.GetComponentInChildren<Enemy>();
            if (entity.HP <= 0)
                continue;
            yield return StartCoroutine(entity.Turn());
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
