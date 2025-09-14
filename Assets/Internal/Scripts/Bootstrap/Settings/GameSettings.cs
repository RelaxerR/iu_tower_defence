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
    #region Tile Map Settings

    [Header("Tile Map Settings")]
    public float DefaultTileSize = 5f;
    public int MapSizeX = 20; // Tiles
    public int MapSizeZ = 20; // Tiles
    public int CreationAttemptLimit = 5; // Максимальное количество попыток создания тайла, прежде чем отказаться от его создания
    
    [Header("Tile Rotation Settings")]
    public int RoadRotationOffset = 90; // Смещение поворота для угловых тайлов дороги
    public int RoadCornerRotationOffset = 90; // Смещение поворота для угловых тайлов дороги
      
    #endregion

    #region Road Settings

    [Header("Castle Settings")]
    public int CastleMaxPositionDeltaX = 5; // Максимальное отклонения Замка игрока от центра
    public int CastleMaxPositionDeltaZ = 5; // Максимальное отклонения Замка игрока от центра
    
    [Header("Road Settings")]
    public int RoadMinLength = 2; // Расстояние, которое должна "пройти" дорога перед следующим поворотом
    public int RoadMaxLength = 5; // Расстояние, которое должна "пройти" дорога перед следующим поворотом
    public int RoadMinTurns = 5; // Максимальное количество поворотов дороги от замка
    public int RoadMaxTurns = 10; // Максимальное количество поворотов дороги от замка
    
    [Header("Road Level Requirements")]
    public int RoadLevelRequirement2 = 10; // Требуемый уровень для генерации второй дороги
    public int RoadLevelRequirement3 = 20; // Требуемый уровень для генерации третей дороги
    public int RoadLevelRequirement4 = 30; // Требуемый уровень для генерации четвертой дороги
    
    #endregion

    #region Resource Settings
    
    [Header("Resource Settings")]
    public float TreeLevelModifier = 3f; // Модификатор уровня для деревьев
    public float StoneLevelModifier = 2f; // Модификатор уровня для камней
    public float DiamondLevelModifier = 1f; // Модификатор уровня для алмазов
    
    [Header("Free Resource Settings")]
    public int FreeRandomTreeMin = 5; // Минимальное количество свободных деревьев
    public int FreeRandomTreeMax = 10; // Максимальное количество свободных деревьев
    public int FreeRandomStoneMin = 3; // Минимальное количество свободных камней
    public int FreeRandomStoneMax = 7; // Максимальное количество свободных камней
    public int FreeRandomDiamondMin = 1; // Минимальное количество свободных алмазов
    public int FreeRandomDiamondMax = 3; // Максимальное количество свободных алмазов
    
    #endregion
    
    #region Prefabs
    
    [Header("Prefabs")]
    public GameObject[] GroundTiles;
    public GameObject[] GroundCornerTiles;
    public GameObject[] RoadTiles;
    public GameObject[] ResourceTreeTiles;
    public GameObject[] ResourceStoneTiles;
    public GameObject[] ResourceDiamondTiles;
    public GameObject[] CastleTiles;
    
    #endregion
  }
}
