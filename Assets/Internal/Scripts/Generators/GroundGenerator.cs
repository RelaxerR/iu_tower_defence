using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Generators
{
  public class GroundGenerator : MonoBehaviour
  {
    private void Start()
    {
      TileManager.GetInstance().InitTiles();
      GenerateGround();
    }

    private void GenerateGround()
    {
      var settings = GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
      var tileManager = TileManager.GetInstance();
      
      for (var x = -settings.MapSizeX; x < settings.MapSizeX; x++)
      {
        for (var z = -settings.MapSizeZ; z < settings.MapSizeZ; z++)
        {
          var position = new Vector3(x * settings.DefaultTileSize, 0, z * settings.DefaultTileSize);
          var tile = tileManager.Tiles.GetValueOrDefault((x, z));
          if (tile == null) continue; // TODO: throw exception or log warning
          
          switch (tile.Type)
          {
            case Tile.TileType.Castle:
              Instantiate(settings.CastleTiles[Random.Range(0, settings.CastleTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            case Tile.TileType.Road:
              Instantiate(settings.RoadTiles[Random.Range(0, settings.RoadTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: this.transform);
              break;
            case Tile.TileType.Ground:
              Instantiate(settings.GroundTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            case Tile.TileType.RoadCorner:
              Instantiate(settings.GroundCornerTiles[Random.Range(0, settings.GroundTiles.Length)], position, Quaternion.AngleAxis(tile.Rotation, Vector3.up), parent: this.transform);
              break;
            case Tile.TileType.Resource:
              Instantiate(settings.ResourceTiles[Random.Range(0, settings.ResourceTiles.Length)], position, Quaternion.identity, parent: this.transform);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }
  }
}
