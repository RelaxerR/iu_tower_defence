using System;
using System.Collections.Generic;
using Internal.Scripts.Bootstrap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Generators
{
  public class GroundGenerator : MonoBehaviour
  {
    [SerializeField]
    private List<GameObject> groundTiles;

    private void Start()
    {
      var settings = GameSettingsManager.GetInstance().Settigns;
      GenerateGround(settings.MapSize, settings.DefaultTileSize);
    }

    private void GenerateGround(int mapSize, float defaultTileSize)
    {
      // Example logic to generate ground tiles
      for (var x = -mapSize; x < mapSize; x++)
      {
        for (var z = -mapSize; z < mapSize; z++)
        {
          var position = new Vector3(x * defaultTileSize, 0, z * defaultTileSize);
          Instantiate(groundTiles[Random.Range(0, groundTiles.Count)], position, Quaternion.identity, parent: this.transform);
        }
      }
    }
  }
}
