using System;

namespace Internal.Scripts.Models
{
  /// <summary>
  /// Класс, представляющий тайл на карте с его типом, позицией и направлением
  /// </summary>
  [Serializable]
  public class Tile
  {
    #region Перечисления

    /// <summary>
    /// Типы тайлов, доступные на карте
    /// </summary>
    public enum TileType
    {
      /// <summary>
      /// Земля
      /// </summary>
      Ground,
      
      /// <summary>
      /// Дорога
      /// </summary>
      Road,
      
      /// <summary>
      /// Конечный тайл дороги
      /// </summary>
      RoadEnd,
      
      /// <summary>
      /// Угловой тайл дороги
      /// </summary>
      RoadCorner,
      
      /// <summary>
      /// Ресурс дерево
      /// </summary>
      ResourceTree,
      
      /// <summary>
      /// Ресурс камень
      /// </summary>
      ResourceStone,
      
      /// <summary>
      /// Ресурс алмаз
      /// </summary>
      ResourceDiamond,
      
      /// <summary>
      /// Замок
      /// </summary>
      Castle
    }
    
    /// <summary>
    /// Направления, доступные для тайлов
    /// </summary>
    public enum TileDirection
    {
      /// <summary>
      /// Север (+Z)
      /// </summary>
      North,
      
      /// <summary>
      /// Восток (+X)
      /// </summary>
      East,
      
      /// <summary>
      /// Юг (-Z)
      /// </summary>
      South,
      
      /// <summary>
      /// Запад (-X)
      /// </summary>
      West
    }

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Возвращает или устанавливает, занят ли тайл
    /// </summary>
    public bool IsOccupied { get; set; } = false;

    /// <summary>
    /// Тип тайла
    /// </summary>
    public TileType Type;
    
    /// <summary>
    /// Направление тайла
    /// </summary>
    public TileDirection Direction;
    
    /// <summary>
    /// Координата X тайла
    /// </summary>
    public int X;
    
    /// <summary>
    /// Координата Z тайла
    /// </summary>
    public int Z;
    
    /// <summary>
    /// Поворот тайла по оси Y в градусах (0, 90, 180, 270)
    /// </summary>
    public int Rotation; // Y Rotation in degrees (0, 90, 180, 270)

    #endregion

    #region Конструкторы

    /// <summary>
    /// Создает новый тайл с указанными параметрами
    /// </summary>
    /// <param name="type">Тип тайла</param>
    /// <param name="x">Координата X</param>
    /// <param name="z">Координата Z</param>
    /// <param name="direction">Направление тайла</param>
    /// <param name="rotation">Поворот тайла в градусах (по умолчанию 0)</param>
    public Tile(TileType type, int x, int z, TileDirection direction, int rotation = 0)
    {
      Type = type;
      Direction = direction;
      X = x;
      Z = z;
      Rotation = rotation;
    }

    #endregion
  }
}