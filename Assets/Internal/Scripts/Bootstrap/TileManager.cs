using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Bootstrap
{
  public class TileManager : MonoBehaviour
  {
    private enum Direction
    {
      Straight,
      RightTurn,
      LeftTurn
    }
    
    public readonly Dictionary<(int x, int z), Tile> Tiles = new();
    private static GroundGeneratorSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }

    private void Awake()
    {
      DontDestroyOnLoad(this.gameObject);
    }

    public void InitTiles()
    {
      var castleTile = GetCastleTile();
      Tiles.Add(castleTile.pos, castleTile.tile);
      Debug.Log($"Initialized castle tile at position: {castleTile.pos}");
      
      InitRoadTiles
      (
        xMultiplier: 1,
        zMultiplier: 0,
        startX: castleTile.pos.x,
        startZ: castleTile.pos.z
      );
    }

    private static ((int x, int z) pos, Tile tile) GetCastleTile()
    {
      var posX = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDelta, settings.CastleMaxPositionDelta));
      var posZ = Mathf.RoundToInt(Random.Range(-settings.CastleMaxPositionDelta, settings.CastleMaxPositionDelta));
      return ((posX, posZ), new Tile(Tile.TileType.Castle, posX, posZ));
    }

    private void CreateDirectRoadX
    (
      int startX,
      int targetX,
      int z,
      int rotation = 0,
      bool isTurned = false
    )
    {
      if (targetX > startX)
      {
        for (var x = startX; x < targetX; x++)
        {
          var tile = new Tile(isTurned && x == startX ? Tile.TileType.RoadCorner : Tile.TileType.Road, x, z, rotation);
          Debug.Log($"Creating road tile at position: {x},{z}; rotation: {rotation} (direction X+)");
          if (!Tiles.TryAdd((x, z), tile)) throw new Exception($"Failed to create road tile at position: {x},{z} - item already exists ({Tiles[(x, z)].Type})");
        }
      }
      else
      {
        for (var x = startX; x > targetX; x--)
        {
          var tile = new Tile(isTurned && x == startX ? Tile.TileType.RoadCorner : Tile.TileType.Road, x, z, rotation);
          Debug.Log($"Creating road tile at position: {x},{z}; rotation: {rotation} (direction X-)");
          if (!Tiles.TryAdd((x, z), tile)) throw new Exception($"Failed to create road tile at position: {x},{z} - item already exists ({Tiles[(x, z)].Type})");
        }
      }
    }
    private void CreateDirectRoadZ
    (
      int startZ,
      int targetZ,
      int x,
      int rotation = 0,
      bool isTurned = false
    )
    {
      if (targetZ > startZ)
      {
        for (var z = startZ; z < targetZ; z++)
        {
          var tile = new Tile(isTurned && z == startZ ? Tile.TileType.RoadCorner : Tile.TileType.Road, x, z, rotation);
          Debug.Log($"Creating road tile at position: {x},{z}; rotation: {rotation} (direction Z+)");
          if (!Tiles.TryAdd((x, z), tile)) throw new Exception($"Failed to create road tile at position: {x},{z} - item already exists ({Tiles[(x, z)].Type})");
        }
      }
      else
      {
        for (var z = startZ; z > targetZ; z--)
        {
          var tile = new Tile(isTurned && z == startZ ? Tile.TileType.RoadCorner : Tile.TileType.Road, x, z, rotation);
          Debug.Log($"Creating road tile at position: {x},{z}; rotation: {rotation} (direction Z-)");
          if (!Tiles.TryAdd((x, z), tile)) throw new Exception($"Failed to create road tile at position: {x},{z} - item already exists ({Tiles[(x, z)].Type})");
        }
      }
    }
    
    private void InitRoadTiles
      (
        int xMultiplier, // Множитель по X (для определения направления дороги)
        int zMultiplier, // Множитель по Z (для определения направления дороги)
        int startX, // Начальная позиция по X
        int startZ // Начальная позиция по Z
      )
    {
      if (
        xMultiplier == 0 && zMultiplier == 0 ||
        Mathf.Abs(xMultiplier) > 0 && Mathf.Abs(zMultiplier) > 0
        ) throw new ArgumentException("Both multipliers cannot be zero or non-zero at the same time.");
      
      var setting = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      
      var currentX = startX + xMultiplier;
      var currentZ = startZ + zMultiplier;
      
      var globalDirection = Direction.Straight;
      var turnsCount = Random.Range(setting.RoadMinTurns, setting.RoadMaxTurns + 1);
      
      var rotation = 0;
      var rotationPrev = rotation;

      for (var currentTurn = 1; currentTurn < turnsCount; currentTurn++)
      {
        if (Mathf.Abs(currentX) >= Mathf.Abs(setting.MapSizeX) || Mathf.Abs(currentZ) >= Mathf.Abs(setting.MapSizeZ))
        {
          Debug.Log("Reached the edge of the map while creating road segments.");
          break;
        }
        int targetX;
        int targetZ;
        
        var length = Random.Range(setting.RoadMinLength, setting.RoadMaxLength + 1);
        
        Debug.Log($"Creating road segment {currentTurn}/{turnsCount} with length {length} and direction {globalDirection}");
        
        switch (globalDirection)
        {
          // right turn
          case Direction.RightTurn:
          {
            targetX = currentX + length * zMultiplier;
            targetZ = currentZ + length * xMultiplier;
            rotation = 90;

            break;
          }
          // left turn
          case Direction.LeftTurn:
          {
            targetX = currentX - length * zMultiplier;
            targetZ = currentZ - length * xMultiplier;
            rotation = 270;

            break;
          }
          // straight
          case Direction.Straight:
          {
            targetX = currentX + length * xMultiplier;
            targetZ = currentZ + length * zMultiplier;
            rotation = 0;

            break;
          }
          default:
            throw new ArgumentOutOfRangeException();
        }
        try
        {
          if (targetX > setting.MapSizeX) targetX = setting.MapSizeX;
          if (targetX < -setting.MapSizeX) targetX = -setting.MapSizeX;
          if (targetZ > setting.MapSizeZ) targetZ = setting.MapSizeZ;
          if (targetZ < -setting.MapSizeZ) targetZ = -setting.MapSizeZ;
          
          CreateDirectRoadX(currentX, targetX, currentZ, rotation, rotation != rotationPrev);
          CreateDirectRoadZ(currentZ, targetZ, currentX, rotation, rotation != rotationPrev);
        }
        catch (Exception e)
        {
          Debug.LogError($"Failed to create road segment {currentTurn}/{turnsCount} from {currentX},{currentZ} to {targetX},{targetZ}: {e.Message}");
          throw;
        }
        currentX = targetX;
        currentZ = targetZ;

        rotationPrev = rotation;
        
        if (globalDirection is Direction.RightTurn or Direction.LeftTurn) globalDirection = Direction.Straight;
        else globalDirection = (Direction) Random.Range(0, 3);
      }
      
      var globalTargetX = xMultiplier != 0 ? setting.MapSizeX : currentX;
      var globalTargetZ = zMultiplier != 0 ? setting.MapSizeZ : currentZ;
      
      Debug.Log($"Creating final road segment to the edge of the map at position: {globalTargetX},{globalTargetZ}");
      try
      {
        CreateDirectRoadX(currentX, globalTargetX, currentZ, 0, rotation != rotationPrev);
        CreateDirectRoadZ(currentZ, globalTargetZ, currentX, 0, rotation != rotationPrev);
      }
      catch (Exception e)
      {
        Debug.LogError($"Failed to create final road segment from {currentX},{currentZ} to {globalTargetX},{globalTargetZ}: {e.Message}");
        throw;
      }
    }
    
    private static TileManager _instance;
    public static TileManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<TileManager>();
      return _instance;
    }
  }
}
