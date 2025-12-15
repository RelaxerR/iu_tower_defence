using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Generators
{
  /// <summary>
  /// Генератор дополнительных объектов (декораций, препятствий, ресурсов) на карте
  /// </summary>
  public class MiscGenerator : MonoBehaviour
  {
    #region Свойства

    private static TileManager tileManager
    {
      get => TileManager.GetInstance();
    }
    
    private static GroundGeneratorSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, генерирует дополнительные объекты на карте
    /// </summary>
    private void Start()
    {
      foreach (var tile in tileManager.Tiles.Values)
      {
        GameObject prefab;
        Vector3 position;
        var rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        
        switch (tile.Type)
        {
          case Tile.TileType.Road:
          case Tile.TileType.RoadEnd:
            prefab = settings.RoadDecorationTiles[Random.Range(0, settings.RoadDecorationTiles.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          case Tile.TileType.Castle:
            prefab = settings.PlayerCastlePrefabs[Random.Range(0, settings.PlayerCastlePrefabs.Length)];
            position = new Vector3(tile.X * settings.DefaultTileSize, settings.MiscHeightOffset, tile.Z * settings.DefaultTileSize);
            rotation = Quaternion.identity;
            break;
          case Tile.TileType.Ground:
            prefab = settings.MiscObstaclePrefabs[Random.Range(0, settings.MiscObstaclePrefabs.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          case Tile.TileType.RoadCorner:
            prefab = settings.RoadDecorationTiles[Random.Range(0, settings.RoadDecorationTiles.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          case Tile.TileType.ResourceTree:
            prefab = settings.TreeResourcePrefabs[Random.Range(0, settings.TreeResourcePrefabs.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          case Tile.TileType.ResourceStone:
            prefab = settings.StoneResourcePrefabs[Random.Range(0, settings.StoneResourcePrefabs.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          case Tile.TileType.ResourceDiamond:
            prefab = settings.DiamondResourcePrefabs[Random.Range(0, settings.DiamondResourcePrefabs.Length)];
            position = GetRandomPositionOnTile(tile.X, tile.Z);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        
        Instantiate(
          prefab,
          position,
          rotation,
          transform);
      }
    }

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Получает случайную позицию внутри тайла
    /// </summary>
    /// <param name="x">Координата X тайла</param>
    /// <param name="z">Координата Z тайла</param>
    /// <returns>Случайная позиция внутри тайла</returns>
    private static Vector3 GetRandomPositionOnTile(int x, int z)
    {
      var tileSize = settings.DefaultTileSize;
      var halfTileSize = tileSize / 2f;
      
      var randomX = Random.Range(-halfTileSize + 0.5f, halfTileSize - 0.5f);
      var randomZ = Random.Range(-halfTileSize + 0.5f, halfTileSize - 0.5f);
      
      return new Vector3(x * tileSize + randomX, settings.MiscHeightOffset, z * tileSize + randomZ);
    }

    #endregion
  }
}