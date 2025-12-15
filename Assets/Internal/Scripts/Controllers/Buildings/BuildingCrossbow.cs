using UnityEngine;
using Internal.Scripts.Controllers.Projectiles;

namespace Internal.Scripts.Controllers.Buildings
{
  /// <summary>
  /// Класс строения-арбалета, который стреляет снарядами по врагам
  /// </summary>
  public class BuildingCrossbow : Building
  {
    #region Поля настроек

    [Header("Конфигурация стрельбы")]
    [SerializeField] 
    private float fireRate = 1f;
    
    [SerializeField] 
    private GameObject projectilePrefab;
    
    [SerializeField] 
    private Transform shootPoint;

    #endregion

    #region Поля состояния

    private float nextFireTime;

    #endregion

    #region Методы Unity

    private void Awake()
    {
      Debug.LogError("[BuildingCrossbow] Префаб снаряда не назначен!", this);
    }
    /// <summary>
    /// Обновляет состояние строения, включая логику стрельбы
    /// </summary>
    protected override void Update()
    {
      base.Update(); // Вызываем родительскую логику (поиск цели, поворот)

      if (nearestTarget && Time.time >= nextFireTime && CanShootByRotation)
      {
        TryShoot();
      }
    }

    #endregion

    #region Методы стрельбы

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Пытается произвести выстрел по ближайшей цели
    /// </summary>
    private void TryShoot()
    {
      if (!nearestTarget)
      {
        Debug.LogWarning("[BuildingCrossbow] Невозможно выстрелить: нет цели.", this);
        return;
      }

      var distanceToTarget = Vector3.Distance(transform.position, nearestTarget.position);

      if (!(distanceToTarget <= detectionRadius))
        return;
      Shoot(nearestTarget);
      nextFireTime = Time.time + (1f / fireRate);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Производит выстрел по указанной цели
    /// </summary>
    /// <param name="target">Цель для стрельбы</param>
    private void Shoot(Transform target)
    {
      if (!projectilePrefab)
      {
        return;
      }

      var spawnPosition = shootPoint ? shootPoint.position : transform.position;
      var newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

      var projectileScript = newProjectile.GetComponent<Projectile>();
      if (projectileScript)
      {
        projectileScript.SetTarget(target);
      }
      else
      {
        Debug.LogWarning("[BuildingCrossbow] Префаб снаряда не содержит компонент Projectile!", this);
      }
    }

    #endregion
  }
}