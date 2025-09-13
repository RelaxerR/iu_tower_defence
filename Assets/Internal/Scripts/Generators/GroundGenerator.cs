using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Models;
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
      Debug.Log("Starting ground generation...");
      TileManager.GetInstance().InitTiles();
      GenerateGround();
      Debug.Log("Ground generation completed");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Генерирует наземные тайлы на основе данных из TileManager.
    /// </summary>
    private void GenerateGround()
    {
      Debug.Log("Generating ground tiles...");
      
      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      var tileManager = TileManager.GetInstance();
      
      var generatedCount = 0;
      var skippedCount = 0;
      
      for (var x = -settings.MapSizeX; x < settings.MapSizeX; x++)
      {
        for (var z = -settings.MapSizeZ; z < settings.MapSizeZ; z++)
        {
          var position = new Vector3(x * settings.DefaultTileSize, 0, z * settings.DefaultTileSize);
          var tile = tileManager.Tiles.GetValueOrDefault((x, z));
          
          if (tile == null)
          {
            skippedCount++;
            continue;
          }
          
          switch (tile.Type)
          {
            case Tile.TileType.Castle:
              Debug.Log($"Instantiating castle tile at ({x}, {z})");
              Instantiate(settings.CastleTiles[Random.Range(0, settings.CastleTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            case Tile.TileType.Road:
              Debug.Log($"Instantiating road tile at ({x}, {z}) with rotation {tile.Rotation}");
              Instantiate(settings.RoadTiles[Random.Range(0, settings.RoadTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: this.transform);
              break;
            case Tile.TileType.Ground:
              Debug.Log($"Instantiating ground tile at ({x}, {z})");
              Instantiate(settings.GroundTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            case Tile.TileType.RoadCorner:
              Debug.Log($"Instantiating road corner tile at ({x}, {z}) with rotation {tile.Rotation}");
              Instantiate(settings.GroundCornerTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: this.transform);
              break;
            case Tile.TileType.Resource:
              Debug.Log($"Instantiating resource tile at ({x}, {z})");
              Instantiate(settings.ResourceTiles[Random.Range(0, settings.ResourceTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            default:
              Debug.LogError($"Unknown tile type: {tile.Type} at ({x}, {z})");
              throw new ArgumentOutOfRangeException();
          }
          generatedCount++;
        }
      }
      
      Debug.Log($"Ground generation summary - Generated: {generatedCount}, Skipped: {skippedCount}, Total: {generatedCount + skippedCount}");
    }

    #endregion
  }
}