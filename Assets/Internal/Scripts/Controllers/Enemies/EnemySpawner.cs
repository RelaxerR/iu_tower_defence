using System;
using System.Collections;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using JetBrains.Annotations;
using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  /// <summary>
  /// Класс спавнера врагов, управляющий генерацией врагов с изменяющимся интервалом
  /// </summary>
  public class EnemySpawner : MonoBehaviour
  {
    #region Поля и свойства

    /// <summary>
    /// Список заспавненных врагов
    /// </summary>
    public List<GameObject> SpawnedList;
    
    [SerializeField]
    private List<GameObject> EnemyList = new();

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

    [CanBeNull]
    private CastleController targetCastle;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует спавнер
    /// </summary>
    private void Start()
    {
      ResetSpawner(); // инициализируем начальные значения
      
      if (EnemyList == null || EnemyList.Count == 0)
      {
        Debug.LogError("EnemySpawner: EnemyList пуст!", this);
        return;
      }
      
      targetCastle = FindAnyObjectByType<CastleController>();
      if (!targetCastle)
      {
        Debug.LogError("EnemySpawner: Замок не найден в сцене!", this);
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

    /// <summary>
    /// Вызывается при деактивации компонента, останавливает спавн
    /// </summary>
    private void OnDisable()
    {
      if (spawnCoroutine == null) return;
      
      StopCoroutine(spawnCoroutine);
      spawnCoroutine = null;
    }

    #endregion

    #region Методы управления спавном

    /// <summary>
    /// Сбрасывает параметры спавнера к начальным значениям
    /// </summary>
    private void ResetSpawner()
    {
      currentSpawnInterval = initialSpawnInterval;
    }

    /// <summary>
    /// Останавливает процесс спавна врагов
    /// </summary>
    private void StopSpawning()
    {
      if (spawnCoroutine != null)
      {
        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
      }
      gameObject.SetActive(false);
    }

    /// <summary>
    /// Запускает процесс спавна врагов
    /// </summary>
    private void StartSpawning()
    {
      if (spawnCoroutine != null)
        StopCoroutine(spawnCoroutine);

      spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    #endregion

    #region Методы спавна

    /// <summary>
    /// Корутина, управляющая циклом спавна врагов
    /// </summary>
    /// <returns>IEnumerator для корутины</returns>
    private IEnumerator SpawnLoop()
    {
      while (true)
      {
        yield return new WaitForSeconds(currentSpawnInterval);
        
        SpawnRandomEnemy();
        
        // Уменьшаем интервал после спавна, но не ниже минимума
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - intervalDecreasePerSpawn);
      }
      // ReSharper disable once IteratorNeverReturns
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Спавнит случайного врага из списка
    /// </summary>
    private void SpawnRandomEnemy()
    {
      if (EnemyList.Count == 0) return;

      var randomIndex = UnityEngine.Random.Range(0, EnemyList.Count);
      var enemyPrefab = EnemyList[randomIndex];

      var position = spawnPoint ? spawnPoint.position : transform.position;
      var newEnemyObj = Instantiate(enemyPrefab, position, Quaternion.identity, transform.parent);

      // Получаем компонент IEnemy от созданного объекта
      var spawnedEnemy = newEnemyObj.GetComponent<IEnemy>();
      if (spawnedEnemy is EnemyFox enemyFoxScript)
      {
        // Передаём цель врагу
        enemyFoxScript.SetTargetCastle(targetCastle);
      }
      else
      {
        Debug.LogWarning("Заспавненный враг не реализует IEnemy или не является скриптом EnemyFox.", newEnemyObj);
      }
      
      SpawnedList.Add(newEnemyObj);
    }

    #endregion
  }
}