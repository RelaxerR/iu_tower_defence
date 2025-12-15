using System;
using System.Collections;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using JetBrains.Annotations;
using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  public class EnemySpawner : MonoBehaviour
  {
    public List<GameObject> SpawnedList;
    
    [SerializeField]
    private List<GameObject> EnemyList = new List<GameObject>();

    [SerializeField, Min(0.1f)]
    private float initialSpawnInterval = 2f; // начальный интервал в секундах

    [SerializeField, Min(0.0f)]
    private float intervalDecreasePerSpawn = 0.05f; // на сколько уменьшать интервал после каждого спавна
    
    [SerializeField, Min(0.1f)]
    private float minSpawnInterval = 0.3f; // минимальный интервал, которого не превысим

    [SerializeField]
    private Transform spawnPoint; // точка спавна (если null — используем позицию спавнера)

    private Coroutine spawnCoroutine;
    private float currentSpawnInterval; // текущий интервал спавна
    private int spawnCount = 0; // количество заспавненных врагов
    
    [CanBeNull]
    private CastleController targetCastle;

    private void Start()
    {
      ResetSpawner(); // инициализируем начальные значения
      
      if (EnemyList == null || EnemyList.Count == 0)
      {
        Debug.LogError("EnemySpawner: EnemyList is empty!", this);
        return;
      }
      
      targetCastle = FindAnyObjectByType<CastleController>();
      if (!targetCastle)
      {
        Debug.LogError("EnemySpawner: No CastleController found in the scene!", this);
        return;
      }
      StartSpawning();
      
      GameManager.GetInstance().OnGameStateChanged.AddListener(state =>
      {
        if (state == GameManager.GameState.Game)
        {
          StartSpawning();
        }
        else
        {
          StopSpawning();
        }
      });
    }

    private void ResetSpawner()
    {
      currentSpawnInterval = initialSpawnInterval;
      spawnCount = 0;
    }

    private void StopSpawning()
    {
      if (spawnCoroutine != null)
      {
        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
      }
      gameObject.SetActive(false);
    }

    private void StartSpawning()
    {
      if (spawnCoroutine != null)
        StopCoroutine(spawnCoroutine);

      spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
      while (true)
      {
        yield return new WaitForSeconds(currentSpawnInterval);
        
        SpawnRandomEnemy();
        
        // Уменьшаем интервал после спавна, но не ниже минимума
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - intervalDecreasePerSpawn);
        spawnCount++;
      }
    }

    // Внутри метода SpawnRandomEnemy() в EnemySpawner.cs
    private void SpawnRandomEnemy()
    {
      if (EnemyList.Count == 0) return;

      var randomIndex = UnityEngine.Random.Range(0, EnemyList.Count);
      var enemyPrefab = EnemyList[randomIndex];

      var position = spawnPoint ? spawnPoint.position : transform.position;
      GameObject newEnemyObj = Instantiate(enemyPrefab, position, Quaternion.identity, transform.parent);

      // Получаем компонент IEnemy от созданного объекта
      IEnemy spawnedEnemy = newEnemyObj.GetComponent<IEnemy>();
      if (spawnedEnemy != null && spawnedEnemy is EnemyFox enemyFoxScript)
      {
        // Передаём цель врагу
        enemyFoxScript.SetTargetCastle(targetCastle);
      }
      else
      {
        Debug.LogWarning("Spawned enemy does not implement IEnemy or is not EnemyFox script.", newEnemyObj);
      }
      
      SpawnedList.Add(newEnemyObj);
    }

    private void OnDisable()
    {
      if (spawnCoroutine == null) return;
      
      StopCoroutine(spawnCoroutine);
      spawnCoroutine = null;
    }
  }
}