using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
      if (EnemyList == null || EnemyList.Count == 0)
      {
        Debug.LogError("EnemySpawner: EnemyList is empty!", this);
        return;
      }

      StartSpawning();
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
        yield return new WaitForSeconds(spawnInterval);
        SpawnRandomEnemy();
      }
    }

    private void SpawnRandomEnemy()
    {
      if (EnemyList.Count == 0) return;

      var randomIndex = UnityEngine.Random.Range(0, EnemyList.Count);
      var enemyPrefab = EnemyList[randomIndex];

      // Предполагается, что IEnemy — это интерфейс, но для спавна нужен GameObject.
      // Поэтому добавим проверку: в реальности IEnemy должен быть реализован MonoBehaviour'ом.
      // Лучше использовать List<GameObject> или List<EnemyController>, но если хочется интерфейс —
      // убедитесь, что реализующий класс наследует MonoBehaviour.

      var position = spawnPoint ? spawnPoint.position : transform.position;
      Instantiate(enemyPrefab.gameObject, position, Quaternion.identity, transform.parent);
    }

    private void OnDisable()
    {
      if (spawnCoroutine == null) return;
      
      StopCoroutine(spawnCoroutine);
      spawnCoroutine = null;
    }
  }
}