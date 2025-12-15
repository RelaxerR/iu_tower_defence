using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Generators
{
  /// <summary>
  /// Генератор тайлов надземных на основе данных из TileManager.
  /// </summary>
  public class GroundGenerator : MonoBehaviour
  {
    #region Жизненный цикл Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует генерацию наземных тайлов
    /// </summary>
    private void Start()
    {
      var level = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 1;
      TileManager.GetInstance().InitTiles(level);
      GenerateGround();
    }

    #endregion

    #region Внутренние методы
    
    /// <summary>
    /// Генерирует наземные тайлы на основе данных из TileManager.
    /// </summary>
    private void GenerateGround()
    {
      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      var tileManager = TileManager.GetInstance();

      for (var x = -settings.MapSizeX; x <= settings.MapSizeX; x++)
      {
        for (var z = -settings.MapSizeZ; z <= settings.MapSizeZ; z++)
        {
          var position = new Vector3(x * settings.DefaultTileSize, 0, z * settings.DefaultTileSize);
          var tile = tileManager.Tiles.GetValueOrDefault((x, z));
          
          if (tile == null)
          {
            continue;
          }

          try
          {
            GameObject instantiatedObject = null;
            GameObject[] prefabArray;
            int rotationOffset;

            switch (tile.Type)
            {
              case Tile.TileType.Castle:
                prefabArray = settings.CastleTiles;
                rotationOffset = 0;
                break;
              case Tile.TileType.RoadEnd:
                prefabArray = settings.RoadEndTiles;
                rotationOffset = settings.RoadRotationOffset;
                break;
              case Tile.TileType.Road:
                prefabArray = settings.RoadTiles;
                rotationOffset = settings.RoadRotationOffset;
                break;
              case Tile.TileType.Ground:
                prefabArray = settings.GroundTiles;
                rotationOffset = 0;
                break;
              case Tile.TileType.RoadCorner:
                prefabArray = settings.GroundCornerTiles;
                rotationOffset = settings.RoadCornerRotationOffset;
                break;
              case Tile.TileType.ResourceTree:
                prefabArray = settings.ResourceTreeTiles;
                rotationOffset = 0;
                break;
              case Tile.TileType.ResourceStone:
                prefabArray = settings.ResourceStoneTiles;
                rotationOffset = 0;
                break;
              case Tile.TileType.ResourceDiamond:
                prefabArray = settings.ResourceDiamondTiles;
                rotationOffset = 0;
                break;
              default:
                Debug.LogError($"Неизвестный тип тайла: {tile.Type} в ({x}, {z})");
                throw new ArgumentOutOfRangeException();
            }

            if (prefabArray != null && prefabArray.Length > 0)
            {
              var randomPrefabIndex = Random.Range(0, prefabArray.Length);
              var rotation = Quaternion.AngleAxis(tile.Rotation + rotationOffset, Vector3.up);
              instantiatedObject = Instantiate(prefabArray[randomPrefabIndex], position, rotation, parent: transform);
            }

            if (instantiatedObject)
            {
              var tileController = instantiatedObject.GetComponent<TileController>();
              if (tileController)
              {
                // Упрощаем копирование данных тайла
                tileController.Tile = new Tile(tile.Type, tile.X, tile.Z, tile.Direction, tile.Rotation);
              }
            }

          }
          catch (Exception e)
          {
            Debug.LogError($"Ошибка при размещении тайлов: [{e.Message} | {e.StackTrace}]");
          }
        }
      }
    }

    #endregion
  }
}