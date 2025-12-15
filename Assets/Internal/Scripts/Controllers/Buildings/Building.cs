using System;
using UnityEngine;
using System.Collections.Generic;
using Internal.Scripts.Controllers.Enemies;
using Internal.Scripts.Controllers.Player;

namespace Internal.Scripts.Controllers.Buildings
{
  /// <summary>
  /// Абстрактный класс для строений, обеспечивающий базовую функциональность обнаружения врагов и поворота
  /// </summary>
  public abstract class Building : MonoBehaviour
  {
    #region Поля настроек

    [Header("Конфигурация строения")]
    [SerializeField] 
    protected float detectionRadius = 5f;
    
    [SerializeField] 
    protected float rotationSpeed = 90f;
    
    [SerializeField] 
    protected LayerMask enemyLayerMask = -1;

    #endregion

    #region Поля состояния

    // Используем обычный Collider для 3D
    private readonly Collider[] detectedEnemiesBuffer = new Collider[10];
    protected Transform nearestTarget = null;

    private Transform lastKnownTarget = null;

    /// <summary>
    /// Список ресурсов, необходимых для покупки/строительства этого строения
    /// </summary>
    public List<ResourceCost> CostResources;

    /// <summary>
    /// Флаг, показывающий возможность выстрела по результатам проверки поворота
    /// </summary>
    protected bool CanShootByRotation;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается каждый кадр, обновляет состояние строения
    /// </summary>
    protected virtual void Update()
    {
      FindNearestEnemy();
      RotateTowardsTarget();
    }

    #endregion

    #region Методы обнаружения

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Находит ближайшего врага в радиусе обнаружения
    /// </summary>
    protected virtual void FindNearestEnemy()
    {
      // Используем 3D физику: Physics.OverlapSphereNonAlloc
      var count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectedEnemiesBuffer, enemyLayerMask);

      if (count == 0)
      {
        nearestTarget = null;
        return;
      }

      var closestDistanceSqr = Mathf.Infinity;
      Transform closestEnemy = null;

      for (var i = 0; i < count; i++)
      {
        if (!detectedEnemiesBuffer[i]) continue; // Защита от null

        var potentialTarget = detectedEnemiesBuffer[i].transform;

        if (potentialTarget.GetComponent<IEnemy>() == null)
        {
          continue;
        }

        var distanceSqr = (potentialTarget.position - transform.position).sqrMagnitude;

        if (!(distanceSqr < closestDistanceSqr))
          continue;
        closestDistanceSqr = distanceSqr;
        closestEnemy = potentialTarget;
      }

      if (closestEnemy && closestEnemy != lastKnownTarget)
      {
        lastKnownTarget = closestEnemy;
      }

      nearestTarget = closestEnemy;
    }

    #endregion

    #region Методы поворота

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Поворачивает строение в сторону ближайшей цели
    /// </summary>
    protected virtual void RotateTowardsTarget()
    {
      if (!nearestTarget)
      {
        CanShootByRotation = false;
        return;
      }

      var directionToTarget = nearestTarget.position - transform.position;
      directionToTarget.y = 0f; // Только горизонтальный поворот

      if (directionToTarget.sqrMagnitude == 0f)
      {
        Debug.LogWarning($"[Building] {gameObject.name}: Цель находится прямо на строении, невозможно повернуться.", this);
        CanShootByRotation = false;
        return;
      }

      var targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

      // Плавный поворот
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        targetRotation,
        rotationSpeed * Time.deltaTime
      );

      // Вычисляем угол между текущим и целевым поворотом
      var angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

      // Проверяем, находится ли поворот в нужном диапазоне (например, 1 градус)
      CanShootByRotation = angleDifference <= 1f; // Можно заменить 1f на переменную, например maxAimAngle
    }

    #endregion

    #region Отладочные методы

    /// <summary>
    /// Отладочный метод для отображения радиуса обнаружения в 3D
    /// </summary>
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion
  }
}