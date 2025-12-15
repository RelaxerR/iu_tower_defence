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
    [SerializeField]
    private List<GameObject> EnemyList = new List<GameObject>();

    [SerializeField, Min(0.1f)]
    private float spawnInterval = 2f; // интервал в секундах

    [SerializeField]
    private Transform spawnPoint; // точка спавна (если null — используем позицию спавнера)

    private Coroutine spawnCoroutine;
    
    [CanBeNull]
    private CastleController targetCastle;

    private void Start()
    {
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

    private void StopSpawning()
    {
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
        SpawnRandomEnemy();
        yield return new WaitForSeconds(spawnInterval);
      }
    }

// Внутри метода SpawnRandomEnemy() в EnemySpawner.cs
    private void SpawnRandomEnemy()
    {
      if (EnemyList.Count == 0) return;

      var randomIndex = UnityEngine.Random.Range(0, EnemyList.Count);
      var enemyPrefab = EnemyList[randomIndex];

      var position = spawnPoint ? spawnPoint.position : transform.position;
      GameObject newEnemyObj = Instantiate(enemyPrefab, position, Quaternion.identity, transform.parent); // Изменили на enemyPrefab, а не enemyPrefab.gameObject

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
    }

    private void OnDisable()
    {
      if (spawnCoroutine == null) return;
      
      StopCoroutine(spawnCoroutine);
      spawnCoroutine = null;
    }
  }
}