using Internal.Scripts.Controllers.Buildings;
using Internal.Scripts.Pathfinding; // Подключаем наш сервис
using Internal.Scripts.Bootstrap;
using ithappy.Animals_FREE; // Подключаем для доступа к GameSettingsManager
using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  /// <summary>
  /// Класс врага-лисы, управляющий его движением, здоровьем и атакой замка
  /// </summary>
  public class EnemyFox : MonoBehaviour, IEnemy
  {
    #region Поля настроек

    [SerializeField] 
    private EnemySettings _settings;
    
    // Добавим ссылку на компонент, управляющий движением/анимацией
    [SerializeField] 
    private CreatureMover _creatureMover;

    #endregion

    #region Поля состояния

    private float _health;
    private CastleController _targetCastle;
    private Vector3 _targetPosition; // Текущая цель движения (позиция следующего тайла или замка)
    private System.Collections.Generic.List<(int x, int z)> _path; // Список координат тайлов пути
    private int _currentPathIndex; // Индекс текущего тайла в пути, к которому движемся
    private const float _reachThreshold = 1.5f; // Расстояние, при котором считаем, что тайл достигнут

    #endregion

    #region Инициализация

    /// <summary>
    /// Вызывается при старте компонента, инициализирует параметры врага
    /// </summary>
    private void Start()
    {
      // Проверим, назначен ли CreatureMover в Inspector
      if (!_creatureMover)
      {
        _creatureMover = GetComponent<CreatureMover>();
        if (!_creatureMover)
        {
          Debug.LogError("EnemyFox: Компонент CreatureMover не найден на этом GameObject!", this);
          enabled = false; // Отключаем скрипт, если не можем найти управляющий компонент
          return;
        }
      }

      if (_settings)
      {
        _health = _settings.MaxHealth;
      }
      else
      {
        Debug.LogError("EnemyFox: EnemySettings не назначен!", this);
        _health = 100f;
      }
    }

    #endregion

    #region Публичные методы (IEnemy)

    /// <summary>
    /// Возвращает настройки врага
    /// </summary>
    /// <returns>Настройки врага</returns>
    public EnemySettings GetSettings() => _settings;

    /// <summary>
    /// Атакует замок
    /// </summary>
    public void Attack()
    {
      // Остановим движение перед атакой
      _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      // Уничтожим врага после атаки (или реализуйте другую логику)
      _targetCastle.TakeDamage(_settings.DamageAmount);
      Die();
    }

    /// <summary>
    /// Применяет урон к врагу
    /// </summary>
    /// <param name="damage">Количество урона</param>
    public void TakeDamage(float damage)
    {
      _health -= damage;
      if (_health <= 0)
      {
        Die();
      }
    }

    /// <summary>
    /// Уничтожает врага
    /// </summary>
    public void Die()
    {
      // Возможно, стоит отключить CreatureMover при смерти
      if(_creatureMover)
        _creatureMover.enabled = false;
      Destroy(gameObject);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Устанавливает замок в качестве цели для врага
    /// </summary>
    /// <param name="castle">Замок, который станет целью</param>
    public void SetTargetCastle(CastleController castle)
    {
      // Добавляем проверку на уже назначенную цель (Вариант 2)
      if (_targetCastle && _targetCastle != castle)
      {
        Debug.LogWarning($"EnemyFox ({gameObject.name}): Цель уже назначена другому замку. Пропускаем повторное назначение.", this);
        return;
      }
      if (!castle)
      {
        _targetCastle = null;
        // Остановим движение
        if(_creatureMover)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
        _path = null;
        return;
      }
      // Если цель уже назначена и совпадает с новой, можно просто выйти
      if (_targetCastle && _targetCastle == castle)
      {
        return;
      }

      _targetCastle = castle;
      if (_targetCastle)
      {
        CalculateAndStartPath(); // Рассчитываем путь и начинаем движение
      }
      else
      {
        Debug.LogWarning("EnemyFox: SetTargetCastle вызван с null замком.", this);
        // Остановим движение, если цель исчезла
        if(_creatureMover)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      }
    }

    #endregion

    #region Внутренние методы

    /// <summary>
    /// Рассчитывает и начинает движение по пути к замку
    /// </summary>
    private void CalculateAndStartPath()
    {
      if (!_targetCastle)
      {
        Debug.LogError("EnemyFox: Невозможно рассчитать путь без цели замка.", this);
        return;
      }

      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      if (settings == null)
      {
        Debug.LogError("EnemyFox: GameSettingsManager или GroundGeneratorSettings равен null.", this);
        return;
      }

      // Получаем координаты текущей позиции врага (округленные до сетки)
      var currentX = Mathf.RoundToInt(transform.position.x / settings.DefaultTileSize);
      var currentZ = Mathf.RoundToInt(transform.position.z / settings.DefaultTileSize);

      // Получаем координаты позиции замка (округленные до сетки)
      var targetX = Mathf.RoundToInt(_targetCastle.transform.position.x / settings.DefaultTileSize);
      var targetZ = Mathf.RoundToInt(_targetCastle.transform.position.z / settings.DefaultTileSize);

      // Ищем путь
      _path = PathfindingService.FindPath(currentX, currentZ, targetX, targetZ);

      if (_path is { Count: > 0 })
      {
        _currentPathIndex = 0; // Начинаем с первого тайла в пути
        // Устанавливаем первую цель - позиция первого тайла в пути
        SetNextWaypoint();
      }
      else
      {
        Debug.LogWarning($"EnemyFox ({gameObject.name}): Не удалось найти путь к замку в сетке ({targetX}, {targetZ}). Останавливаемся.", this);
        // Враг не может добраться - можно остановить его или сделать что-то еще
        _path = null; // Явно указываем, что пути нет
        // Остановим движение
        if(_creatureMover)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Устанавливает следующую точку маршрута
    /// </summary>
    private void SetNextWaypoint()
    {
      if (!_creatureMover)
      {
        Debug.LogError("EnemyFox: CreatureMover равен null при попытке установить путевую точку.", this);
        return;
      }

      if (_path == null || _currentPathIndex >= _path.Count)
      {
        Debug.LogError("EnemyFox: Попытка установить путевую точку, но путь недействителен или индекс вне диапазона.", this);
        // Остановим движение
        _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
        return;
      }

      var (x, z) = _path[_currentPathIndex];
      var tileSize = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings.DefaultTileSize;
      // Цель - центр тайла
      _targetPosition = new Vector3(x * tileSize, transform.position.y, z * tileSize); // Удерживаем Y от врага
    }

    /// <summary>
    /// Обновляет состояние врага каждый кадр
    /// </summary>
    private void Update()
    {
      if (_path == null || _currentPathIndex >= _path.Count)
        return;
      // Проверяем, достиг ли враг текущей цели
      if (Vector3.Distance(transform.position, _targetPosition) <= _reachThreshold)
      {
        _currentPathIndex++; // Переходим к следующему тайлу в пути

        if (_currentPathIndex < _path.Count)
        {
          SetNextWaypoint(); // Устанавливаем цель для следующего тайла
        }
        else
        {
          // Достигли последнего тайла в пути
          // Проверяем, это замок?
          if (_targetCastle)
          {
            Attack(); // Вызываем атаку
          }
          else
          {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.LogWarning($"EnemyFox ({gameObject.name}): Достигнут конец пути, но цель замка исчезла.", this);
          }
          _path = null; // Сброс пути
        }
      }
      else
      {
        // В Update постоянно отправляем врагу команду двигаться "вперед" и смотреть на следующую точку маршрута
        // Это заставит CreatureMover плавно поворачиваться к _targetPosition и двигаться вперед
        if (_creatureMover)
        {
          // inputAxis (0, 1) означает "двигаться вперед локально"
          // _targetPosition - точка, к которой нужно повернуть
          _creatureMover.SetInput(new Vector2(0f, 1f), _targetPosition, true, false);
        }
      }
    }

    #endregion
  }
}