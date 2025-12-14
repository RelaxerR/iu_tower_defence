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
    #region Fields

    /// <summary>
    /// Словарь всех тайлов на карте, индексированных по координатам (x, z).
    /// </summary>
    public readonly Dictionary<(int x, int z), Tile> Tiles = new();

    private static GroundGeneratorSettings settings => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;

    private static TileManager _instance;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
      _instance = this;
      Debug.Log("TileManager initialized");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Инициализирует тайлы на карте: размещает замок и генерирует дороги от него.
    /// </summary>
    public void InitTiles(int level)
    {
      Debug.Log("Initializing tiles...");
      var castleTile = GetCastleTile();
      Tiles.Add(castleTile.pos, castleTile.tile);
      Debug.Log($"Initialized castle tile at position: {castleTile.pos}");
      
      CreateRoadFromCastle(castleTile.pos, North);
      if (level > settings.RoadLevelRequirement2) CreateRoadFromCastle(castleTile.pos, South);
      if (level > settings.RoadLevelRequirement3) CreateRoadFromCastle(castleTile.pos, West);
      if (level > settings.RoadLevelRequirement4) CreateRoadFromCastle(castleTile.pos, East);
      
      InitResources(level);
      
      Debug.Log($"Total tiles created: {Tiles.Count}");

      FillGroundTiles();
    }

    /// <summary>
    /// Возвращает экземпляр TileManager.
    /// </summary>
    public static TileManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<TileManager>();
      return _instance;
    }

    #endregion

    #region Private Methods - Tile Creation

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
    private void InitResources(int level)
    {
      InitResourceTiles(ResourceTree, Mathf.CeilToInt(level * settings.TreeLevelModifier) + Random.Range(settings.FreeRandomTreeMin, settings.FreeRandomTreeMax));
      InitResourceTiles(ResourceStone, Mathf.CeilToInt(level * settings.StoneLevelModifier) + Random.Range(settings.FreeRandomStoneMin, settings.FreeRandomStoneMax));
      InitResourceTiles(ResourceDiamond, Mathf.CeilToInt(level * settings.DiamondLevelModifier) + Random.Range(settings.FreeRandomDiamondMin, settings.FreeRandomDiamondMax));
    }
    private void InitResourceTiles(Tile.TileType type, int amount)
    {
      for (var i = 0; i < amount; i++)
      {
        var (posX, posZ) = GetFreeRandomPosition();
        
        var tile = new Tile(type, posX, posZ, North);
        if (Tiles.TryAdd((posX, posZ), tile))
        {
          Debug.Log($"Added resource tile of type {type} at ({posX}, {posZ})");
        }
        else
        {
          Debug.LogError($"Could not add resource tile at ({posX}, {posZ}), already occupied by {Tiles[(posX, posZ)].Type} - position was not free");
        }
      }
    }
    private static (int x, int z) GetRandomPosition()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.MapSizeX, settings.MapSizeX));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.MapSizeZ, settings.MapSizeZ));
      return (posX, posZ);
    }
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
        
        Debug.LogWarning("Exceeded maximum attempts to find free position for resource");
        return pos;

      } while (Tiles.ContainsKey(pos));
      
      return pos;
    }
    private static ((int x, int z) pos, Tile tile) GetCastleTile()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDeltaX, settings.CastleMaxPositionDeltaX));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDeltaZ, settings.CastleMaxPositionDeltaZ));
      Debug.Log($"Generating castle at position: ({posX}, {posZ})");
      return ((posX, posZ), new Tile(Castle, posX, posZ, North));
    }

    private void CreateRoadFromCastle((int x, int z) castlePos, Tile.TileDirection initialDirection)
    {
      Debug.Log($"Creating road from castle at {castlePos} in direction {initialDirection}");
      var startPos = GetNextPosition(castlePos.x, castlePos.z, initialDirection);
      var startRotation = GetRotationForDirection(initialDirection);

      CreateRoadWithTurns(startPos.x, startPos.z, initialDirection, startRotation);
    }

    /// <summary>
    /// Создаёт сегмент дороги с поворотами.
    /// </summary>
    private void CreateRoadWithTurns(int startX, int startZ, Tile.TileDirection initialDirection, int initialRotation)
    {
      Debug.Log($"Starting road creation from ({startX}, {startZ}) in direction {initialDirection}");
      
      var currentX = startX;
      var currentZ = startZ;
      var prevCurrentX = startX;
      var prevCurrentZ = startZ;
      bool isRechedBounds = false;
      var currentDirection = initialDirection;
      var excludedDirection = GetOpposite(initialDirection);
      var currentRotation = initialRotation;
      Tile.TileDirection? previousTurnDirection = null;

      var turnsCount = Random.Range(settings.RoadMinTurns, settings.RoadMaxTurns + 1);
      Debug.Log($"Road will have {turnsCount} turns");

      for (var turn = 0; turn < turnsCount; turn++)
      {
        if (IsOutOfMapBounds(currentX, currentZ))
        {
          Debug.Log($"Road stopped at ({currentX}, {currentZ}) - out of bounds");
          isRechedBounds = true;
          break;
        }

        var segmentLength = Random.Range(settings.RoadMinLength, settings.RoadMaxLength + 1);

        if (turn == turnsCount - 1)
        {
          segmentLength = GetMaxLengthToBoundary(currentX, currentZ, currentDirection);
          Debug.Log($"Last segment length adjusted to {segmentLength} to reach map boundary");
          currentDirection = initialDirection;
          currentRotation = GetRotationForDirection(currentDirection);
        }

        Debug.Log($"Creating road segment #{turn + 1}: length={segmentLength}, direction={currentDirection}");
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
        Debug.Log($"Next turn direction: {nextDirection}");
        previousTurnDirection = nextDirection;
        currentDirection = nextDirection;
        currentRotation = GetRotationForDirection(currentDirection);
      }
      
      if (isRechedBounds) Tiles[(prevCurrentX, prevCurrentZ)].Type = RoadEnd;
      else Tiles[(currentX, currentZ)].Type = RoadEnd;
      
      Debug.Log($"Finished road creation ending at ({currentX}, {currentZ})");
    }

    /// <summary>
    /// Создаёт прямой сегмент дороги заданной длины.
    /// </summary>
    private (int endX, int endZ) CreateStraightRoadSegment(
      int startX,
      int startZ,
      Tile.TileDirection direction,
      int length,
      int rotation)
    {
      Debug.Log($"Creating straight road segment from ({startX}, {startZ}), direction={direction}, length={length}");
      
      int endX = startX, endZ = startZ;

      for (var i = 0; i < length; i++)
      {
        var (x, z) = GetPositionAtDistance(startX, startZ, direction, i);

        if (IsOutOfMapBounds(x, z))
        {
          Debug.Log($"Road segment stopped at ({x}, {z}) - out of bounds");
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
              Debug.Log($"Updated tile at ({x}, {z}) to corner: {existingTile.Direction} -> {direction}, rotation={existingTile.Rotation}");
            }
            else
            {
              Debug.Log($"Tile at ({x}, {z}) already exists with same direction, skipping");
            }
          }
          else
          {
            Debug.LogWarning($"Could not add tile at ({x}, {z}), already occupied by {Tiles[(x, z)].Type}");
          }
        }
        else
        {
          Debug.Log($"Added road tile at ({x}, {z}) with rotation {rotation}");
        }

        endX = x;
        endZ = z;
      }

      Debug.Log($"Road segment completed, ending at ({endX}, {endZ})");
      return (endX, endZ);
    }

    #endregion

    #region Private Methods - Direction Utilities

    private static int GetMaxLengthToBoundary(int x, int z, Tile.TileDirection direction)
    {
      var maxLength = direction switch
      {
        North or South => settings.MapSizeZ - Mathf.Abs(z),
        East or West => settings.MapSizeX - Mathf.Abs(x),
        _ => 1
      };
      Debug.Log($"Max length to boundary from ({x}, {z}) in direction {direction}: {maxLength}");
      return maxLength;
    }

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
      Debug.Log($"Corner rotation for {from} -> {to}: {rotation}");
      return rotation;
    }

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
      Debug.Log($"Next position from ({x}, {z}) in direction {direction}: ({pos.Item1}, {pos.Item2})");
      return pos;
    }

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

    private static int GetRotationForDirection(Tile.TileDirection direction)
    {
      var rotation = direction switch
      {
        North => 90,
        East => 180,
        South => 270,
        West => 0,
        _ => 0
      };
      Debug.Log($"Rotation for direction {direction}: {rotation}");
      return rotation;
    }

    private static Tile.TileDirection GetNextDirection(Tile.TileDirection currentDirection, Tile.TileDirection[] excludedDirections)
    {
      var allDirections = (Tile.TileDirection[])Enum.GetValues(typeof(Tile.TileDirection));
      var validDirections = allDirections
        .Where(dir => dir != currentDirection && dir != GetOpposite(currentDirection) && !excludedDirections.Contains(dir))
        .ToList();
      
      Debug.Log($"Current direction: {currentDirection}, Excluded: [{string.Join(", ", excludedDirections)}], Valid: [{string.Join(", ", validDirections)}]");
      
      var nextDirection = validDirections[Random.Range(0, validDirections.Count)];
      Debug.Log($"Selected next direction: {nextDirection}");
      return nextDirection;
    }

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
      Debug.Log($"Opposite of {dir} is {opposite}");
      return opposite;
    }

    #endregion

    #region Private Methods - Validation

    private static bool IsOutOfMapBounds(int x, int z)
    {
      var outOfBounds = Mathf.Abs(x) > settings.MapSizeX || Mathf.Abs(z) > settings.MapSizeZ;
      if (outOfBounds)
      {
        Debug.Log($"Position ({x}, {z}) is out of map bounds");
      }
      return outOfBounds;
    }

    #endregion
  }
}