using System;
using UnityEngine;

namespace Internal.Scripts.Bootstrap.Settings
{
  /// <summary>
  /// Настройки игры, содержащие конфигурацию основных систем игры
  /// </summary>
  [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings", order = 0)]
  public class GameSettings : ScriptableObject
  {
    #region Системы настроек

    /// <summary>
    /// Настройки генерации игровой местности
    /// </summary>
    [Tooltip("Настройки генерации игровой местности")]
    public GroundGeneratorSettings GroundGeneratorSettings;

    /// <summary>
    /// Настройки игрока
    /// </summary>
    [Tooltip("Настройки игрока")]
    public PlayerSettings PlayerSettings;

    /// <summary>
    /// Настройки камеры
    /// </summary>
    [Tooltip("Настройки камеры")]
    public CameraSettings CameraSettings;

    #endregion

    /// <summary>
    /// Максимальное значение уровня
    /// </summary>
    [Tooltip("Максимальное значение уровня")]
    public float LevelMaxValue = 100f;

    /// <summary>
    /// Продолжительность уровня в секундах (по умолчанию 5 минут)
    /// </summary>
    [Tooltip("Продолжительность уровня в секундах (по умолчанию 5 минут)")]
    public float LevelDurationSeconds = 300f; // 5 минут
  }

  /// <summary>
  /// Настройки генерации игровой местности
  /// </summary>
  [Serializable]
  public class GroundGeneratorSettings
  {
    #region Настройки тайловой карты

    [Header("Настройки тайловой карты")]
    [Tooltip("Размер тайла по умолчанию")]
    public float DefaultTileSize = 5f;

    /// <summary>
    /// Ширина карты в тайлах
    /// </summary>
    [Tooltip("Ширина карты в тайлах")]
    public int MapSizeX = 20; // Тайлы

    /// <summary>
    /// Глубина карты в тайлах
    /// </summary>
    [Tooltip("Глубина карты в тайлах")]
    public int MapSizeZ = 20; // Тайлы

    /// <summary>
    /// Максимальное количество попыток создания тайла, прежде чем отказаться от его создания
    /// </summary>
    [Tooltip("Максимальное количество попыток создания тайла, прежде чем отказаться от его создания")]
    public int CreationAttemptLimit = 5;

    /// <summary>
    /// Высота, на которой будут размещаться случайные объекты (деревья, камни и т.д.)
    /// </summary>
    [Tooltip("Высота, на которой будут размещаться случайные объекты (деревья, камни и т.д.)")]
    public float MiscHeightOffset = 1f;

    [Header("Настройки поворота тайлов")]
    [Tooltip("Смещение поворота для угловых тайлов дороги")]
    public int RoadRotationOffset = 90;

    /// <summary>
    /// Смещение поворота для угловых тайлов дороги
    /// </summary>
    [Tooltip("Смещение поворота для угловых тайлов дороги")]
    public int RoadCornerRotationOffset = 90;

    #endregion

    #region Настройки дороги

    [Header("Настройки замка")]
    [Tooltip("Максимальное отклонение позиции замка игрока от центра по оси X")]
    public int CastleMaxPositionDeltaX = 5;

    /// <summary>
    /// Максимальное отклонение позиции замка игрока от центра по оси Z
    /// </summary>
    [Tooltip("Максимальное отклонение позиции замка игрока от центра по оси Z")]
    public int CastleMaxPositionDeltaZ = 5;

    [Header("Настройки дороги")]
    [Tooltip("Минимальная длина дороги до следующего поворота")]
    public int RoadMinLength = 2;

    /// <summary>
    /// Максимальная длина дороги до следующего поворота
    /// </summary>
    [Tooltip("Максимальная длина дороги до следующего поворота")]
    public int RoadMaxLength = 5;

    /// <summary>
    /// Минимальное количество поворотов дороги от замка
    /// </summary>
    [Tooltip("Минимальное количество поворотов дороги от замка")]
    public int RoadMinTurns = 5;

    /// <summary>
    /// Максимальное количество поворотов дороги от замка
    /// </summary>
    [Tooltip("Максимальное количество поворотов дороги от замка")]
    public int RoadMaxTurns = 10;

    [Header("Требования к уровню дороги")]
    [Tooltip("Требуемый уровень для генерации второй дороги")]
    public int RoadLevelRequirement2 = 10;

    /// <summary>
    /// Требуемый уровень для генерации третьей дороги
    /// </summary>
    [Tooltip("Требуемый уровень для генерации третьей дороги")]
    public int RoadLevelRequirement3 = 20;

    /// <summary>
    /// Требуемый уровень для генерации четвертой дороги
    /// </summary>
    [Tooltip("Требуемый уровень для генерации четвертой дороги")]
    public int RoadLevelRequirement4 = 30;

    #endregion

    #region Настройки ресурсов

    [Header("Настройки ресурсов")]
    [Tooltip("Модификатор уровня для деревьев")]
    public float TreeLevelModifier = 3f;

    /// <summary>
    /// Модификатор уровня для камней
    /// </summary>
    [Tooltip("Модификатор уровня для камней")]
    public float StoneLevelModifier = 2f;

    /// <summary>
    /// Модификатор уровня для алмазов
    /// </summary>
    [Tooltip("Модификатор уровня для алмазов")]
    public float DiamondLevelModifier = 1f;

    [Header("Настройки бесплатных ресурсов")]
    [Tooltip("Минимальное количество свободных деревьев")]
    public int FreeRandomTreeMin = 5;

    /// <summary>
    /// Максимальное количество свободных деревьев
    /// </summary>
    [Tooltip("Максимальное количество свободных деревьев")]
    public int FreeRandomTreeMax = 10;

    /// <summary>
    /// Минимальное количество свободных камней
    /// </summary>
    [Tooltip("Минимальное количество свободных камней")]
    public int FreeRandomStoneMin = 3;

    /// <summary>
    /// Максимальное количество свободных камней
    /// </summary>
    [Tooltip("Максимальное количество свободных камней")]
    public int FreeRandomStoneMax = 7;

    /// <summary>
    /// Минимальное количество свободных алмазов
    /// </summary>
    [Tooltip("Минимальное количество свободных алмазов")]
    public int FreeRandomDiamondMin = 1;

    /// <summary>
    /// Максимальное количество свободных алмазов
    /// </summary>
    [Tooltip("Максимальное количество свободных алмазов")]
    public int FreeRandomDiamondMax = 3;

    #endregion

    #region Префабы

    [Header("Префабы тайлов")]
    [Tooltip("Массив префабов основных тайлов местности")]
    public GameObject[] GroundTiles;

    /// <summary>
    /// Массив префабов угловых тайлов местности
    /// </summary>
    [Tooltip("Массив префабов угловых тайлов местности")]
    public GameObject[] GroundCornerTiles;

    /// <summary>
    /// Массив префабов тайлов дороги
    /// </summary>
    [Tooltip("Массив префабов тайлов дороги")]
    public GameObject[] RoadTiles;

    /// <summary>
    /// Массив префабов конечных тайлов дороги
    /// </summary>
    [Tooltip("Массив префабов конечных тайлов дороги")]
    public GameObject[] RoadEndTiles;

    /// <summary>
    /// Массив префабов деревьев как ресурсов
    /// </summary>
    [Tooltip("Массив префабов деревьев как ресурсов")]
    public GameObject[] ResourceTreeTiles;

    /// <summary>
    /// Массив префабов камней как ресурсов
    /// </summary>
    [Tooltip("Массив префабов камней как ресурсов")]
    public GameObject[] ResourceStoneTiles;

    /// <summary>
    /// Массив префабов алмазов как ресурсов
    /// </summary>
    [Tooltip("Массив префабов алмазов как ресурсов")]
    public GameObject[] ResourceDiamondTiles;

    /// <summary>
    /// Массив префабов замка
    /// </summary>
    [Tooltip("Массив префабов замка")]
    public GameObject[] CastleTiles;

    [Header("Различные префабы")]
    [Tooltip("Массив префабов замка игрока")]
    public GameObject[] PlayerCastlePrefabs;

    /// <summary>
    /// Массив префабов деревьев как ресурсов
    /// </summary>
    [Tooltip("Массив префабов деревьев как ресурсов")]
    public GameObject[] TreeResourcePrefabs;

    /// <summary>
    /// Массив префабов камней как ресурсов
    /// </summary>
    [Tooltip("Массив префабов камней как ресурсов")]
    public GameObject[] StoneResourcePrefabs;

    /// <summary>
    /// Массив префабов алмазов как ресурсов
    /// </summary>
    [Tooltip("Массив префабов алмазов как ресурсов")]
    public GameObject[] DiamondResourcePrefabs;

    /// <summary>
    /// Массив префабов различных препятствий
    /// </summary>
    [Tooltip("Массив префабов различных препятствий")]
    public GameObject[] MiscObstaclePrefabs;

    /// <summary>
    /// Массив префабов декоративных элементов дороги
    /// </summary>
    [Tooltip("Массив префабов декоративных элементов дороги")]
    public GameObject[] RoadDecorationTiles;

    #endregion
  }

  /// <summary>
  /// Настройки игрока
  /// </summary>
  [Serializable]
  public class PlayerSettings
  {
    #region Настройки игрока

    /// <summary>
    /// Скорость движения игрока
    /// </summary>
    [Tooltip("Скорость движения игрока")]
    public float MoveSpeed = 5f;

    #endregion

    #region Настройки замка

    /// <summary>
    /// Максимальное здоровье замка
    /// </summary>
    [Tooltip("Максимальное здоровье замка")]
    public float CastleMaxHealth = 100f;

    #endregion
  }

  /// <summary>
  /// Настройки камеры
  /// </summary>
  [Serializable]
  public class CameraSettings
  {
    #region Настройки камеры

    /// <summary>
    /// Смещение камеры относительно целевой точки
    /// </summary>
    [Tooltip("Смещение камеры относительно целевой точки")]
    public Vector3 CameraOffset = new Vector3(0, 10, -10);

    /// <summary>
    /// Скорость следования камеры за игроком
    /// </summary>
    [Tooltip("Скорость следования камеры за игроком")]
    public float CameraFollowSpeed = 5f;

    /// <summary>
    /// Модификатор минимальной границы по оси Z
    /// </summary>
    [Tooltip("Модификатор минимальной границы по оси Z")]
    public float BoundsMinZModifier = -1f;

    /// <summary>
    /// Модификатор максимальной границы по оси Z
    /// </summary>
    [Tooltip("Модификатор максимальной границы по оси Z")]
    public float BoundsMaxZModifier = -4f;

    /// <summary>
    /// Модификатор минимальной границы по оси X
    /// </summary>
    [Tooltip("Модификатор минимальной границы по оси X")]
    public float BoundsMinXModifier = +1f;

    /// <summary>
    /// Модификатор максимальной границы по оси X
    /// </summary>
    [Tooltip("Модификатор максимальной границы по оси X")]
    public float BoundsMaxXModifier = -1f;

    #endregion
  }
}