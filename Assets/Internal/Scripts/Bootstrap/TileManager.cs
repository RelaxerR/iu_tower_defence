using System;
using System.Collections.Generic;
using System.Linq;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;
using static Internal.Scripts.Models.Tile.TileDirection;
using static Internal.Scripts.Models.Tile.TileType;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Менеджер управления тайлами на карте: инициализация замка, дорог и поворотов.
  /// </summary>
  public class TileManager : MonoBehaviour
  {
    #region Поля

    /// <summary>
    /// Словарь всех тайлов на карте, индексированных по координатам (x, z).
    /// </summary>
    public readonly Dictionary<(int x, int z), Tile> Tiles = new();

    private static GroundGeneratorSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }

    private static TileManager _instance;

    #endregion

    #region Жизненный цикл Unity

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
      _instance = this;
    }

    #endregion

    #region Публичные методы

    /// <summary>
    /// Инициализирует тайлы на карте: размещает замок и генерирует дороги от него.
    /// </summary>
    /// <param name="level">Уровень сложности, влияющий на количество дорог и ресурсов</param>
    public void InitTiles(int level)
    {
      var castleTile = GetCastleTile();
      Tiles.Add(castleTile.pos, castleTile.tile);
      
      CreateRoadFromCastle(castleTile.pos, North);
      if (level > settings.RoadLevelRequirement2) CreateRoadFromCastle(castleTile.pos, South);
      if (level > settings.RoadLevelRequirement3) CreateRoadFromCastle(castleTile.pos, West);
      if (level > settings.RoadLevelRequirement4) CreateRoadFromCastle(castleTile.pos, East);
      
      InitResources(level);
      FillGroundTiles();
    }

    /// <summary>
    /// Возвращает экземпляр TileManager.
    /// </summary>
    /// <returns>Единственный экземпляр TileManager в сцене</returns>
    public static TileManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<TileManager>();
      return _instance;
    }

    #endregion

    #region Приватные методы - Создание тайлов

    /// <summary>
    /// Заполняет пустые места на карте тайлами земли
    /// </summary>
    private void FillGroundTiles()
    {
      for (var x = -settings.MapSizeX; x <= settings.MapSizeX; x++)
      {
        for (var z = -settings.MapSizeZ; z <= settings.MapSizeZ; z++)
        {
          if (Tiles.ContainsKey((x, z)))
            continue;

          var tile = new Tile(Ground, x, z, North);
          Tiles.Add((x, z), tile);
        }
      }
    }

    /// <summary>
    /// Инициализирует тайлы ресурсов на карте
    /// </summary>
    /// <param name="level">Уровень игры, влияющий на количество ресурсов</param>
    private void InitResources(int level)
    {
      InitResourceTiles(ResourceTree, Mathf.CeilToInt(level * settings.TreeLevelModifier) + Random.Range(settings.FreeRandomTreeMin, settings.FreeRandomTreeMax));
      InitResourceTiles(ResourceStone, Mathf.CeilToInt(level * settings.StoneLevelModifier) + Random.Range(settings.FreeRandomStoneMin, settings.FreeRandomStoneMax));
      InitResourceTiles(ResourceDiamond, Mathf.CeilToInt(level * settings.DiamondLevelModifier) + Random.Range(settings.FreeRandomDiamondMin, settings.FreeRandomDiamondMax));
    }

    /// <summary>
    /// Инициализирует тайлы конкретного типа ресурса
    /// </summary>
    /// <param name="type">Тип ресурса</param>
    /// <param name="amount">Количество тайлов ресурса</param>
    private void InitResourceTiles(Tile.TileType type, int amount)
    {
      for (var i = 0; i < amount; i++)
      {
        var (posX, posZ) = GetFreeRandomPosition();
        
        var tile = new Tile(type, posX, posZ, North);
        if (Tiles.TryAdd((posX, posZ), tile))
        {
          // Тайл успешно добавлен
        }
        else
        {
          Debug.LogError($"Не удалось добавить тайл ресурса в ({posX}, {posZ}), уже занято тайлом {Tiles[(posX, posZ)].Type} - позиция была занята");
        }
      }
    }

    /// <summary>
    /// Получает случайную позицию в пределах карты
    /// </summary>
    /// <returns>Кортеж с координатами (x, z)</returns>
    private static (int x, int z) GetRandomPosition()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.MapSizeX, settings.MapSizeX));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.MapSizeZ, settings.MapSizeZ));
      return (posX, posZ);
    }

    /// <summary>
    /// Получает свободную случайную позицию на карте
    /// </summary>
    /// <returns>Кортеж с координатами (x, z) свободной позиции</returns>
    private (int x, int z) GetFreeRandomPosition()
    {
      var attempt = 0;
      (int x, int z) pos;
      
      do
      {
        pos = GetRandomPosition();
        attempt++;

        if (attempt <= settings.CreationAttemptLimit)
          continue;
        
        Debug.LogWarning("Превышено максимальное количество попыток найти свободную позицию для ресурса");
        return pos;

      } while (Tiles.ContainsKey(pos));
      
      return pos;
    }

    /// <summary>
    /// Получает тайл замка с случайной позицией
    /// </summary>
    /// <returns>Кортеж с позицией и тайлом замка</returns>
    private static ((int x, int z) pos, Tile tile) GetCastleTile()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDeltaX, settings.CastleMaxPositionDeltaX));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDeltaZ, settings.CastleMaxPositionDeltaZ));
      return ((posX, posZ), new Tile(Castle, posX, posZ, North));
    }

    /// <summary>
    /// Создаёт дорогу от замка в заданном направлении
    /// </summary>
    /// <param name="castlePos">Позиция замка</param>
    /// <param name="initialDirection">Начальное направление дороги</param>
    private void CreateRoadFromCastle((int x, int z) castlePos, Tile.TileDirection initialDirection)
    {
      var startPos = GetNextPosition(castlePos.x, castlePos.z, initialDirection);
      var startRotation = GetRotationForDirection(initialDirection);

      CreateRoadWithTurns(startPos.x, startPos.z, initialDirection, startRotation);
    }

    /// <summary>
    /// Создаёт сегмент дороги с поворотами.
    /// </summary>
    /// <param name="startX">Начальная координата X</param>
    /// <param name="startZ">Начальная координата Z</param>
    /// <param name="initialDirection">Начальное направление дороги</param>
    /// <param name="initialRotation">Начальный угол поворота</param>
    private void CreateRoadWithTurns(int startX, int startZ, Tile.TileDirection initialDirection, int initialRotation)
    {
      var currentX = startX;
      var currentZ = startZ;
      var prevCurrentX = startX;
      var prevCurrentZ = startZ;
      bool isReachedBounds = false;
      var currentDirection = initialDirection;
      var excludedDirection = GetOpposite(initialDirection);
      var currentRotation = initialRotation;
      Tile.TileDirection? previousTurnDirection = null;

      var turnsCount = Random.Range(settings.RoadMinTurns, settings.RoadMaxTurns + 1);

      for (var turn = 0; turn < turnsCount; turn++)
      {
        if (IsOutOfMapBounds(currentX, currentZ))
        {
          isReachedBounds = true;
          break;
        }

        var segmentLength = Random.Range(settings.RoadMinLength, settings.RoadMaxLength + 1);

        if (turn == turnsCount - 1)
        {
          segmentLength = GetMaxLengthToBoundary(currentX, currentZ, currentDirection);
          currentDirection = initialDirection;
          currentRotation = GetRotationForDirection(currentDirection);
        }

        var (endX, endZ) = CreateStraightRoadSegment(currentX, currentZ, currentDirection, segmentLength, currentRotation);
        prevCurrentX = currentX;
        prevCurrentZ = currentZ;
        currentX = endX;
        currentZ = endZ;

        if (turn >= turnsCount - 1)
          continue;

        var excludedDirections = previousTurnDirection.HasValue
          ? new[] { previousTurnDirection.Value, excludedDirection }
          : new[] { excludedDirection };

        var nextDirection = GetNextDirection(currentDirection, excludedDirections);
        previousTurnDirection = nextDirection;
        currentDirection = nextDirection;
        currentRotation = GetRotationForDirection(currentDirection);
      }
      
      if (isReachedBounds) 
        Tiles[(prevCurrentX, prevCurrentZ)].Type = RoadEnd;
      else 
        Tiles[(currentX, currentZ)].Type = RoadEnd;
    }

    /// <summary>
    /// Создаёт прямой сегмент дороги заданной длины.
    /// </summary>
    /// <param name="startX">Начальная координата X</param>
    /// <param name="startZ">Начальная координата Z</param>
    /// <param name="direction">Направление дороги</param>
    /// <param name="length">Длина сегмента</param>
    /// <param name="rotation">Угол поворота тайла</param>
    /// <returns>Координаты конечной точки сегмента</returns>
    private (int endX, int endZ) CreateStraightRoadSegment(
      int startX,
      int startZ,
      Tile.TileDirection direction,
      int length,
      int rotation)
    {
      int endX = startX, endZ = startZ;

      for (var i = 0; i < length; i++)
      {
        var (x, z) = GetPositionAtDistance(startX, startZ, direction, i);

        if (IsOutOfMapBounds(x, z))
        {
          break;
        }

        const Tile.TileType tileType = Road;
        var tile = new Tile(tileType, x, z, direction, rotation);

        if (!Tiles.TryAdd((x, z), tile))
        {
          if (Tiles[(x, z)].Type == Road)
          {
            var existingTile = Tiles[(x, z)];
            if (existingTile.Direction != direction)
            {
              existingTile.Type = RoadCorner;
              existingTile.Rotation = GetCornerRotation(existingTile.Direction, direction);
            }
          }
          else
          {
            Debug.LogWarning($"Не удалось добавить тайл в ({x}, {z}), уже занят тайлом {Tiles[(x, z)].Type}");
          }
        }

        endX = x;
        endZ = z;
      }

      return (endX, endZ);
    }

    #endregion

    #region Приватные методы - Вспомогательные функции направления

    /// <summary>
    /// Получает максимальную длину до границы карты в заданном направлении
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="z">Координата Z</param>
    /// <param name="direction">Направление</param>
    /// <returns>Максимальная длина до границы</returns>
    private static int GetMaxLengthToBoundary(int x, int z, Tile.TileDirection direction)
    {
      var maxLength = direction switch
      {
        North or South => settings.MapSizeZ - Mathf.Abs(z),
        East or West => settings.MapSizeX - Mathf.Abs(x),
        _ => 1
      };
      return maxLength;
    }

    /// <summary>
    /// Получает угол поворота для углового тайла дороги
    /// </summary>
    /// <param name="from">Направление входа</param>
    /// <param name="to">Направление выхода</param>
    /// <returns>Угол поворота в градусах</returns>
    private static int GetCornerRotation(Tile.TileDirection from, Tile.TileDirection to)
    {
      var rotation = (from, to) switch
      {
        (North, West) => 0,
        (North, East) => 270,
        (West, South) => 270,
        (West, North) => 180,
        (South, East) => 180,
        (South, West) => 90,
        (East, North) => 90,
        (East, South) => 0,
        _ => 180
      };
      return rotation;
    }

    /// <summary>
    /// Получает следующую позицию в заданном направлении
    /// </summary>
    /// <param name="x">Текущая координата X</param>
    /// <param name="z">Текущая координата Z</param>
    /// <param name="direction">Направление</param>
    /// <returns>Кортеж с новыми координатами (x, z)</returns>
    private static (int x, int z) GetNextPosition(int x, int z, Tile.TileDirection direction)
    {
      var pos = direction switch
      {
        North => (x, z + 1),
        East => (x + 1, z),
        South => (x, z - 1),
        West => (x - 1, z),
        _ => (x, z)
      };
      return pos;
    }

    /// <summary>
    /// Получает позицию на заданном расстоянии в указанном направлении
    /// </summary>
    /// <param name="startX">Начальная координата X</param>
    /// <param name="startZ">Начальная координата Z</param>
    /// <param name="direction">Направление</param>
    /// <param name="distance">Расстояние</param>
    /// <returns>Кортеж с координатами в нужной точке</returns>
    private static (int x, int z) GetPositionAtDistance(int startX, int startZ, Tile.TileDirection direction, int distance)
    {
      var pos = direction switch
      {
        North => (startX, startZ + distance),
        East => (startX + distance, startZ),
        South => (startX, startZ - distance),
        West => (startX - distance, startZ),
        _ => (startX, startZ)
      };
      return pos;
    }

    /// <summary>
    /// Получает угол поворота для заданного направления
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns>Угол поворота в градусах</returns>
    private static int GetRotationForDirection(Tile.TileDirection direction)
    {
      var rotation = direction switch
      {
        North => 90,
        East => 180,
        South => 270,
        _ => 0
      };
      return rotation;
    }

    /// <summary>
    /// Получает следующее направление, исключая указанные
    /// </summary>
    /// <param name="currentDirection">Текущее направление</param>
    /// <param name="excludedDirections">Массив исключенных направлений</param>
    /// <returns>Случайное допустимое направление</returns>
    private static Tile.TileDirection GetNextDirection(Tile.TileDirection currentDirection, Tile.TileDirection[] excludedDirections)
    {
      var allDirections = (Tile.TileDirection[])Enum.GetValues(typeof(Tile.TileDirection));
      var validDirections = allDirections
        .Where(dir => dir != currentDirection && dir != GetOpposite(currentDirection) && !excludedDirections.Contains(dir))
        .ToList();
      
      var nextDirection = validDirections[Random.Range(0, validDirections.Count)];
      return nextDirection;
    }

    /// <summary>
    /// Получает противоположное направление
    /// </summary>
    /// <param name="dir">Исходное направление</param>
    /// <returns>Противоположное направление</returns>
    private static Tile.TileDirection GetOpposite(Tile.TileDirection dir)
    {
      var opposite = dir switch
      {
        North => South,
        East => West,
        South => North,
        West => East,
        _ => dir
      };
      return opposite;
    }

    #endregion

    #region Приватные методы - Валидация

    /// <summary>
    /// Проверяет, находится ли позиция за пределами карты
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="z">Координата Z</param>
    /// <returns>true если позиция за пределами карты, иначе false</returns>
    private static bool IsOutOfMapBounds(int x, int z)
    {
      var outOfBounds = Mathf.Abs(x) > settings.MapSizeX || Mathf.Abs(z) > settings.MapSizeZ;
      return outOfBounds;
    }

    #endregion
  }
}