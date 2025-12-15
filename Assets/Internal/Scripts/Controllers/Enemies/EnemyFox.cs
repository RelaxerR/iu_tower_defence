using Internal.Scripts.Controllers.Buildings;
using Internal.Scripts.Pathfinding; // Подключаем наш сервис
using Internal.Scripts.Bootstrap;
using ithappy.Animals_FREE; // Подключаем для доступа к GameSettingsManager
using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  public class EnemyFox : MonoBehaviour, IEnemy
  {
    [SerializeField] private EnemySettings _settings;
    // Добавим ссылку на компонент, управляющий движением/анимацией
    [SerializeField] private CreatureMover _creatureMover;

    private float _health;
    private CastleController _targetCastle;
    private Vector3 _targetPosition; // Текущая цель движения (позиция следующего тайла или замка)
    private System.Collections.Generic.List<(int x, int z)> _path; // Список координат тайлов пути
    private int _currentPathIndex = 0; // Индекс текущего тайла в пути, к которому движемся
    private float _moveSpeed = 1.0f; // Скорость движения, можно брать из _settings
    private const float _reachThreshold = 1.5f; // Расстояние, при котором считаем, что тайл достигнут

    #region Initialization

    private void Start()
    {
      // Проверим, назначен ли CreatureMover в Inspector
      if (_creatureMover == null)
      {
        _creatureMover = GetComponent<CreatureMover>();
        if (_creatureMover == null)
        {
          Debug.LogError("EnemyFox: CreatureMover component not found on this GameObject!", this);
          enabled = false; // Отключаем скрипт, если не можем найти управляющий компонент
          return;
        }
      }

      if (_settings)
      {
        _health = _settings.MaxHealth;
        _moveSpeed = _settings.Speed; // Предполагается, что Speed есть в EnemySettings
        // Debug.Log($"EnemyFox ({gameObject.name}): Initialized with Health: {_health}, Speed: {_moveSpeed}");
      }
      else
      {
        Debug.LogError("EnemyFox: EnemySettings is not assigned!", this);
        _health = 100f;
      }
    }

    #endregion

    #region Public Methods (IEnemy)

    public EnemySettings GetSettings() => _settings;

    public void Attack()
    {
      // Debug.Log($"{gameObject.name} is attacking the castle!");
      // TODO: Реализуйте логику атаки
      // Остановим движение перед атакой
      _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      // Уничтожим врага после атаки (или реализуйте другую логику)
      _targetCastle.TakeDamage(_settings.DamageAmount);
      Die();
    }

    public void TakeDamage(float damage)
    {
      _health -= damage;
      Debug.Log($"{name}: Took {damage} damage. Remaining health: {_health}", this);
      if (_health <= 0)
      {
        Die();
      }
    }

    public void Die()
    {
      // Debug.Log($"EnemyFox ({gameObject.name}): Died.");
      // Возможно, стоит отключить CreatureMover при смерти
      if(_creatureMover != null)
        _creatureMover.enabled = false;
      Destroy(gameObject);
    }

    public void Move()
    {
      // Устаревший метод, не используется
    }

    public void SetTargetCastle(CastleController castle)
    {
      // Добавляем проверку на уже назначенную цель (Вариант 2)
      if (_targetCastle != null && _targetCastle != castle)
      {
        Debug.LogWarning($"EnemyFox ({gameObject.name}): Target is already assigned to a different castle. Skipping reassignment.", this);
        return;
      }
      if (castle == null)
      {
         // Debug.Log($"EnemyFox ({gameObject.name}): SetTargetCastle called with null castle. Clearing target and stopping.", this);
         _targetCastle = null;
         // Остановим движение
         if(_creatureMover != null)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
        _path = null;
        return;
      }
      // Если цель уже назначена и совпадает с новой, можно просто выйти
      if (_targetCastle != null && _targetCastle == castle)
      {
         // Debug.Log($"EnemyFox ({gameObject.name}): Target castle is already assigned. Skipping reassignment.", this);
         return;
      }

      // Debug.Log($"EnemyFox ({gameObject.name}): SetTargetCastle called with {(castle != null ? "valid" : "null")} castle.");
      _targetCastle = castle;
      if (_targetCastle != null)
      {
        CalculateAndStartPath(); // Рассчитываем путь и начинаем движение
      }
      else
      {
        Debug.LogWarning("EnemyFox: SetTargetCastle called with null castle.", this);
        // Остановим движение, если цель исчезла
        if(_creatureMover != null)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      }
    }

    #endregion

    private void CalculateAndStartPath()
    {
      // Debug.Log($"EnemyFox ({gameObject.name}): Calculating path...");
      if (_targetCastle == null)
      {
        Debug.LogError("EnemyFox: Cannot calculate path without a target castle.", this);
        return;
      }

      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      if (settings == null)
      {
         Debug.LogError("EnemyFox: GameSettingsManager or GroundGeneratorSettings is null.", this);
         return;
      }

      // Получаем координаты текущей позиции врага (округленные до сетки)
      int currentX = Mathf.RoundToInt(transform.position.x / settings.DefaultTileSize);
      int currentZ = Mathf.RoundToInt(transform.position.z / settings.DefaultTileSize);

      // Получаем координаты позиции замка (округленные до сетки)
      int targetX = Mathf.RoundToInt(_targetCastle.transform.position.x / settings.DefaultTileSize);
      int targetZ = Mathf.RoundToInt(_targetCastle.transform.position.z / settings.DefaultTileSize);

      // Debug.Log($"EnemyFox ({gameObject.name}): Calculating path from grid ({currentX}, {currentZ}) to grid ({targetX}, {targetZ})");

      // Ищем путь
      _path = PathfindingService.FindPath(currentX, currentZ, targetX, targetZ);

      if (_path != null && _path.Count > 0)
      {
        _currentPathIndex = 0; // Начинаем с первого тайла в пути
        // Debug.Log($"EnemyFox ({gameObject.name}): Path found with {_path.Count} nodes. Starting movement.");
        // Устанавливаем первую цель - позиция первого тайла в пути
        SetNextWaypoint();
      }
      else
      {
        Debug.LogWarning($"EnemyFox ({gameObject.name}): Failed to find a path to the castle at grid ({targetX}, {targetZ}). Stopping.", this);
        // Враг не может добраться - можно остановить его или сделать что-то еще
        _path = null; // Явно указываем, что пути нет
        // Остановим движение
        if(_creatureMover != null)
          _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
      }
    }

    private void SetNextWaypoint()
    {
      if (_creatureMover == null)
      {
        Debug.LogError("EnemyFox: CreatureMover is null when trying to set waypoint.", this);
        return;
      }

      if (_path == null || _currentPathIndex >= _path.Count)
      {
        Debug.LogError("EnemyFox: Attempting to set waypoint but path is invalid or index is out of bounds.", this);
        // Остановим движение
        _creatureMover.SetInput(Vector2.zero, transform.position, false, false);
        return;
      }

      var (x, z) = _path[_currentPathIndex];
      var tileSize = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings.DefaultTileSize;
      // Цель - центр тайла
      _targetPosition = new Vector3(x * tileSize, transform.position.y, z * tileSize); // Удерживаем Y от врага

      // Debug.Log($"EnemyFox ({gameObject.name}): Moving towards waypoint grid ({x}, {z}) at world pos {_targetPosition}, path index: {_currentPathIndex}");

      // --- Ключевое изменение ---
      // Вместо вычисления inputAxis как вектора к цели, используем постоянное "вперед"
      // CreatureMover будет поворачиваться к _targetPosition, и (0, 1) заставит его идти "вперед" относительно его ориентации.
      // Но, чтобы избежать резких поворотов в начале, установим цель, и дадим врагу время повернуться в Update.
      // SetNextWaypoint теперь просто устанавливает _targetPosition и сбрасывает флаг остановки.

      // Мы не вызываем SetInput здесь, а будем вызывать его в Update, чтобы постоянно корректировать поворот и поддерживать движение вперед.
    }

    private void Update()
    {
      if (_path != null && _currentPathIndex < _path.Count)
      {
        // Проверяем, достиг ли враг текущей цели
        if (Vector3.Distance(transform.position, _targetPosition) <= _reachThreshold)
        {
          // Debug.Log($"EnemyFox ({gameObject.name}): Reached waypoint grid ({_path[_currentPathIndex].x}, {_path[_currentPathIndex].z}) at index {_currentPathIndex}");
          _currentPathIndex++; // Переходим к следующему тайлу в пути

          if (_currentPathIndex < _path.Count)
          {
            SetNextWaypoint(); // Устанавливаем цель для следующего тайла
            // После установки новой цели, Update продолжит вызывать SetInput в следующем кадре
          }
          else
          {
            // Достигли последнего тайла в пути
            // Debug.Log($"EnemyFox ({gameObject.name}): Reached the end of the path.");
            // Проверяем, это замок?
            if (_targetCastle != null)
            {
              // Проверим расстояние до замка на всякий случай
              float distanceToCastle = Vector3.Distance(transform.position, _targetCastle.transform.position);
              if(distanceToCastle <= _reachThreshold + 0.5f) // Немного больше порога пути
              {
                // Debug.Log($"EnemyFox ({gameObject.name}): Close enough to castle at {_targetCastle.transform.position}. Attacking.");
                Attack(); // Вызываем атаку
                // CreatureMover остановится в методе Attack
              }
              else
              {
                // Мы достигли последнего тайла в пути, но замок еще далеко.
                // В реальности враг может "увидеть" замок и пойти прямо на него, или атаковать отсюда.
                // Для простоты будем считать, что он достиг цели и атакует.
                // Debug.Log($"EnemyFox ({gameObject.name}): Reached path end near castle at {_targetCastle.transform.position}. Attacking (last path node).");
                Attack();
              }
            }
            else
            {
               Debug.LogWarning("EnemyFox ({gameObject.name}): Reached path end, but target castle is gone.", this);
            }
            _path = null; // Сброс пути
          }
        }
        else
        {
          // --- Ключевое изменение ---
          // В Update постоянно отправляем врагу команду двигаться "вперед" и смотреть на следующую точку маршрута
          // Это заставит CreatureMover плавно поворачиваться к _targetPosition и двигаться вперед
          if (_creatureMover != null)
          {
            // inputAxis (0, 1) означает "двигаться вперед локально"
            // _targetPosition - точка, к которой нужно повернуть
            _creatureMover.SetInput(new Vector2(0f, 1f), _targetPosition, true, false);
            // // Debug.Log($"Setting inputAxis: (0, 1), target: {_targetPosition}");
          }
        }
      }
      // else if (_path == null || _currentPathIndex >= _path.Count)
      // {
      //   // CreatureMover уже остановлен в SetNextWaypoint или SetTargetCastle
      //   // или в Attack
      // }
    }
  }
}