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
    public PlayerSettings PlayerSettings;
    public CameraSettings CameraSettings;
    public ResourceSettings ResourceSettings;

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
    public float MiscHeightOffset = 1f; // Высота, на которой будут размещаться случайные объекты (деревья, камни и т.д.)
    
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
    
    [Header("Misc prefabs")]
    public GameObject[] PlayerCastlePrefabs;
    public GameObject[] TreeResourcePrefabs;
    public GameObject[] StoneResourcePrefabs;
    public GameObject[] DiamondResourcePrefabs;
    public GameObject[] MiscObstaclePrefabs;
    public GameObject[] RoadDecorationTiles;

    #endregion
  }
  
  [Serializable]
  public class PlayerSettings
  {
    #region Player Settings

    public float MoveSpeed = 5f;
    public float RotationSpeed = 720f; // Degrees per second
    
    #endregion
  }
  
  [Serializable]
  public class CameraSettings
  {
    #region Camera Settings

    public Vector3 CameraOffset = new Vector3(0, 10, -10);
    public float CameraFollowSpeed = 5f;

    public float BoundsMinZModifier = -1f;
    public float BoundsMaxZModifier = -4f;
    public float BoundsMinXModifier = +1f;
    public float BoundsMaxXModifier = -1f;

    #endregion
  }
  
  [Serializable]
  public class ResourceSettings
  {
    #region Resource Settings

    public int TreeMinAmount = 3;
    public int TreeMaxAmount = 7;
    public int StoneMinAmount = 2;
    public int StoneMaxAmount = 5;
    public int DiamondMinAmount = 1;
    public int DiamondMaxAmount = 3;

    #endregion
  }
}
