using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Models;
using Unity.Mathematics;
using UnityEngine.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Generators
{
  /// <summary>
  /// Генератор наземных тайлов на основе данных из TileManager.
  /// </summary>
  public class GroundGenerator : MonoBehaviour
  {
    #region Unity Lifecycle

    private void Start()
    {
      // Debug.Log("Starting ground generation...");
      TileManager.GetInstance().InitTiles(1);
      GenerateGround();
      // Debug.Log("Ground generation completed");
    }

    #endregion

    #region Private Methods
    
    /// <summary>
    /// Генерирует наземные тайлы на основе данных из TileManager.
    /// </summary>
    private void GenerateGround()
    {
      // Debug.Log("Generating ground tiles...");
      
      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      var tileManager = TileManager.GetInstance();
      
      var generatedCount = 0;
      var skippedCount = 0;
      
      for (var x = -settings.MapSizeX; x <= settings.MapSizeX; x++)
      {
        for (var z = -settings.MapSizeZ; z <= settings.MapSizeZ; z++)
        {
          var position = new Vector3(x * settings.DefaultTileSize, 0, z * settings.DefaultTileSize);
          var tile = tileManager.Tiles.GetValueOrDefault((x, z));
          
          if (tile == null)
          {
            skippedCount++;
            continue;
          }

          try
          {
            switch (tile.Type)
            {
              case Tile.TileType.Castle:
                // Debug.Log($"Instantiating castle tile at ({x}, {z})");
                var castleObj = Instantiate(settings.CastleTiles[Random.Range(0, settings.CastleTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: transform);
                var castleTileComponent = castleObj.GetComponent<TileController>();
                castleTileComponent.Tile.Type = tile.Type;
                castleTileComponent.Tile.X = tile.X;
                castleTileComponent.Tile.Z = tile.Z;
                castleTileComponent.Tile.Direction = tile.Direction;
                castleTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.RoadEnd:
                // Debug.Log($"Instantiating road end tile at ({x}, {z}) with rotation {tile.Rotation}");
                var roadEndObj = Instantiate(settings.RoadEndTiles[Random.Range(0, settings.RoadEndTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation + settings.RoadRotationOffset, Vector3.up), parent: transform);
                var roadEndTileComponent = roadEndObj.GetComponent<TileController>();
                roadEndTileComponent.Tile.Type = tile.Type;
                roadEndTileComponent.Tile.X = tile.X;
                roadEndTileComponent.Tile.Z = tile.Z;
                roadEndTileComponent.Tile.Direction = tile.Direction;
                roadEndTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.Road:
                // Debug.Log($"Instantiating road tile at ({x}, {z}) with rotation {tile.Rotation}");
                var roadObj = Instantiate(settings.RoadTiles[Random.Range(0, settings.RoadTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation + settings.RoadRotationOffset, Vector3.up), parent: transform);
                var roadTileComponent = roadObj.GetComponent<TileController>();
                roadTileComponent.Tile.Type = tile.Type;
                roadTileComponent.Tile.X = tile.X;
                roadTileComponent.Tile.Z = tile.Z;
                roadTileComponent.Tile.Direction = tile.Direction;
                roadTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.Ground:
                // Debug.Log($"Instantiating ground tile at ({x}, {z})");
                var groundObj = Instantiate(settings.GroundTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: transform);
                var groundTileComponent = groundObj.GetComponent<TileController>();
                groundTileComponent.Tile.Type = tile.Type;
                groundTileComponent.Tile.X = tile.X;
                groundTileComponent.Tile.Z = tile.Z;
                groundTileComponent.Tile.Direction = tile.Direction;
                groundTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.RoadCorner:
                // Debug.Log($"Instantiating road corner tile at ({x}, {z}) with rotation {tile.Rotation}");
                var cornerObj = Instantiate(settings.GroundCornerTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation + settings.RoadCornerRotationOffset, Vector3.up), parent: transform);
                var cornerTileComponent = cornerObj.GetComponent<TileController>();
                cornerTileComponent.Tile.Type = tile.Type;
                cornerTileComponent.Tile.X = tile.X;
                cornerTileComponent.Tile.Z = tile.Z;
                cornerTileComponent.Tile.Direction = tile.Direction;
                cornerTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.ResourceTree:
                // Debug.Log($"Instantiating resource tile at ({x}, {z})");
                var treeObj = Instantiate(settings.ResourceTreeTiles[Random.Range(0, settings.ResourceTreeTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: transform);
                var treeTileComponent = treeObj.GetComponent<TileController>();
                treeTileComponent.Tile.Type = tile.Type;
                treeTileComponent.Tile.X = tile.X;
                treeTileComponent.Tile.Z = tile.Z;
                treeTileComponent.Tile.Direction = tile.Direction;
                treeTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.ResourceStone:
                // Debug.Log($"Instantiating resource tile at ({x}, {z})");
                var stoneObj = Instantiate(settings.ResourceStoneTiles[Random.Range(0, settings.ResourceStoneTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: transform);
                var stoneTileComponent = stoneObj.GetComponent<TileController>();
                stoneTileComponent.Tile.Type = tile.Type;
                stoneTileComponent.Tile.X = tile.X;
                stoneTileComponent.Tile.Z = tile.Z;
                stoneTileComponent.Tile.Direction = tile.Direction;
                stoneTileComponent.Tile.Rotation = tile.Rotation;
                break;
              case Tile.TileType.ResourceDiamond:
                // Debug.Log($"Instantiating resource tile at ({x}, {z})");
                var diamondObj = Instantiate(settings.ResourceDiamondTiles[Random.Range(0, settings.ResourceDiamondTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: transform);
                var diamondTileComponent = diamondObj.GetComponent<TileController>();
                diamondTileComponent.Tile.Type = tile.Type;
                diamondTileComponent.Tile.X = tile.X;
                diamondTileComponent.Tile.Z = tile.Z;
                diamondTileComponent.Tile.Direction = tile.Direction;
                diamondTileComponent.Tile.Rotation = tile.Rotation;
                break;
              default:
                Debug.LogError($"Unknown tile type: {tile.Type} at ({x}, {z})");
                throw new ArgumentOutOfRangeException();
            }
            generatedCount++;
          }
          catch (Exception e)
          {
            Debug.LogError($"Error while placing tiles: [{e.Message} | {e.StackTrace}]");
            skippedCount++;
          }
        }
      }
      
      // Debug.Log($"Ground generation summary - Generated: {generatedCount}, Skipped: {skippedCount}, Total: {generatedCount + skippedCount}");
    }

    #endregion
  }
}