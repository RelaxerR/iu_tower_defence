using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Internal.Scripts.Bootstrap.Settings
{
  [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings", order = 0)]
  public class GameSettings : ScriptableObject
  {
    #region MyRegion
    
    public GroundGeneratorSettings GroundGeneratorSettings;

    #endregion

  }
  
  [Serializable]
  public class GroundGeneratorSettings
  {
    #region MyRegion

    public float DefaultTileSize = 5f;
    public int MapSizeX = 20; // Tiles
    public int MapSizeZ = 20; // Tiles
      
    #endregion
    
    public int CastleMaxPositionDelta = 5; // Максимальное отклонения Замка игрока от центра
    public int RoadMinLength = 2; // Расстояние, которое должна "пройти" дорога перед следующим поворотом
    public int RoadMaxLength = 5; // Расстояние, которое должна "пройти" дорога перед следующим поворотом
    public int RoadMinTurns = 5; // Максимальное количество поворотов дороги от замка
    public int RoadMaxTurns = 10; // Максимальное количество поворотов дороги от замка
    
    #region Prefabs

    public GameObject[] GroundTiles;
    public GameObject[] GroundCornerTiles;
    public GameObject[] RoadTiles;
    public GameObject[] ResourceTiles;
    public GameObject[] CastleTiles;
    
    #endregion
  }
}
