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
  public class TileManager : MonoBehaviour
  {


    public readonly Dictionary<(int x, int z), Tile> Tiles = new();
    
    private static GroundGeneratorSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }

    public void InitTiles()
    {
      var castleTile = GetCastleTile();
      Tiles.Add(castleTile.pos, castleTile.tile);
      Debug.Log($"Initialized castle tile at position: {castleTile.pos}");
      
      // Создаем дороги в нескольких направлениях от замка
      CreateRoadFromCastle(castleTile.pos, North);
      // CreateRoadFromCastle(castleTile.pos, Tile.TileDirection.East);
      CreateRoadFromCastle(castleTile.pos, South);
      // CreateRoadFromCastle(castleTile.pos, Tile.TileDirection.West);
    }

    private static ((int x, int z) pos, Tile tile) GetCastleTile()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDelta, settings.CastleMaxPositionDelta));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDelta, settings.CastleMaxPositionDelta));
      return ((posX, posZ), new Tile(Castle, posX, posZ, North));
    }

    private void CreateRoadFromCastle((int x, int z) castlePos, Tile.TileDirection initialDirection)
    {
      var startPos = GetNextPosition(castlePos.x, castlePos.z, initialDirection);
      var startRotation = GetRotationForDirection(initialDirection);
      
      CreateRoadWithTurns(startPos.x, startPos.z, initialDirection, startRotation);
    }

    private void CreateRoadWithTurns(int startX, int startZ, Tile.TileDirection initialDirection, int initialRotation)
    {
      var currentX = startX;
      var currentZ = startZ;
      var currentDirection = initialDirection;
      var excludedDirection = GetOpposite(initialDirection);
      var currentRotation = initialRotation;
      Tile.TileDirection? previousTurnDirection = null;

      var turnsCount = Random.Range(settings.RoadMinTurns, settings.RoadMaxTurns + 1);

      for (var turn = 0; turn < turnsCount; turn++)
      {
        if (IsOutOfMapBounds(currentX, currentZ))
          break;

        // Последний сегмент — до границы карты
        var segmentLength = Random.Range(settings.RoadMinLength, settings.RoadMaxLength + 1);
        
        if (turn == turnsCount - 1)
        {
          segmentLength = GetMaxLengthToBoundary(currentX, currentZ, currentDirection);
          currentDirection = initialDirection;
        }

        var (endX, endZ) = CreateStraightRoadSegment(currentX, currentZ, currentDirection, segmentLength, currentRotation);
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
    }

    private (int endX, int endZ) CreateStraightRoadSegment(int startX, int startZ, Tile.TileDirection direction, int length, int rotation)
    {
      int endX = startX, endZ = startZ;
      
      for (var i = 0; i < length; i++)
      {
        var (x, z) = GetPositionAtDistance(startX, startZ, direction, i);
        Debug.Log($"Creating road tile at ({x},{z}) with rotation {rotation}");
        
        // Проверяем границы
        if (IsOutOfMapBounds(x, z))
          break;
        
        // Определяем тип тайла (обычная дорога)
        const Tile.TileType tileType = Road;
        var tile = new Tile(tileType, x, z, direction, rotation);
        
        if (!Tiles.TryAdd((x, z), tile))
        {
          if (tile.Type == Road)
          {
            var existingTile = Tiles[(x, z)];
            if (existingTile.Direction == direction)
            {
              Debug.Log($"Tile at {x}, {z} already exists with same direction {direction}, skipping.");
            }
            else
            {
              existingTile.Type = RoadCorner;
              existingTile.Rotation = GetCornerRotation(existingTile.Direction, direction);
              Debug.Log($"Tile at {x}, {z} direction: {existingTile.Direction} -> {direction} updated to Turn with rotation {existingTile.Rotation}.");
            }
          }
          else
          {
            Debug.LogWarning($"Could not add tile at ({x},{z}), already occupied by {Tiles[(x, z)].Type}");
          }
        }
        
        endX = x;
        endZ = z;
      }
      
      return (endX, endZ);
    }

    // Вспомогательные методы
    private static int GetMaxLengthToBoundary(int x, int z, Tile.TileDirection direction)
    {
      return direction switch
      {
        North => settings.MapSizeZ - Mathf.Abs(z),
        South => settings.MapSizeZ - Mathf.Abs(z),
        East => settings.MapSizeX - Mathf.Abs(x),
        West => settings.MapSizeX - Mathf.Abs(x),
        _ => 1
      };
    }
    private static int GetCornerRotation(Tile.TileDirection from, Tile.TileDirection to)
    {
      // North->West: 0, West->South: 90, South->East: 180, East->North: 270
      return (from, to) switch
      {
        (North, West) => 0,
        (North, East) => 270,
        (West, South) => 270,
        (West, North) => 180,
        (South, East) => 180,
        (South, West) => 90,
        (East, North) => 90,
        (East, South) => 0,
        // Add other valid turns if needed
        _ => 180
      };
    }
    private static (int x, int z) GetNextPosition(int x, int z, Tile.TileDirection direction)
    {
      return direction switch
      {
        North => (x, z + 1),
        East => (x + 1, z),
        South => (x, z - 1),
        West => (x - 1, z),
        _ => (x, z)
      };
    }

    private static (int x, int z) GetPositionAtDistance(int startX, int startZ, Tile.TileDirection direction, int distance)
    {
      return direction switch
      {
        North => (startX, startZ + distance),
        East => (startX + distance, startZ),
        South => (startX, startZ - distance),
        West => (startX - distance, startZ),
        _ => (startX, startZ)
      };
    }

    private static int GetRotationForDirection(Tile.TileDirection direction)
    {
      return direction switch
      {
        North => 90,   // Повернут вправо
        East => 180, // Повернут на 180
        South => 270,  // Повернут влево
        West => 0,  // Не повернут
        _ => 0
      };
    }

    private static Tile.TileDirection GetNextDirection(Tile.TileDirection currentDirection, Tile.TileDirection[] excludedDirections)
    {
      var allDirections = (Tile.TileDirection[])Enum.GetValues(typeof(Tile.TileDirection));
      var validDirections = allDirections
        .Where(dir => dir != currentDirection && dir != GetOpposite(currentDirection) && !excludedDirections.Contains(dir))
        .ToList();
      Debug.Log($"Current: {currentDirection}, Excluded Turns: {string.Join(", ", excludedDirections)}, Valid Next: {string.Join(", ", validDirections)}");

      return validDirections[Random.Range(0, validDirections.Count)];
    }
    private static Tile.TileDirection GetOpposite(Tile.TileDirection dir) => dir switch
    {
      North => South,
      East => West,
      South => North,
      West => East,
      _ => dir
    };

    private static bool IsOutOfMapBounds(int x, int z) => Mathf.Abs(x) > settings.MapSizeX || Mathf.Abs(z) > settings.MapSizeZ;

    private static TileManager _instance;
    public static TileManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<TileManager>();
      return _instance;
    }
  }
}