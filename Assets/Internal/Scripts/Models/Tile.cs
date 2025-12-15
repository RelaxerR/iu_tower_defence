using System;
using UnityEngine;

namespace Internal.Scripts.Models
{
  [Serializable]
  public class Tile
  {
    public enum TileType
    {
      Ground,
      Road,
      RoadEnd,
      RoadCorner,
      ResourceTree,
      ResourceStone,
      ResourceDiamond,
      Castle
    }
    public enum TileDirection
    {
      North, // +Z
      East,  // +X
      South, // -Z
      West   // -X
    }

    public TileType Type;
    public TileDirection Direction;
    public int X;
    public int Z;
    public int Rotation; // Y Rotation in degrees (0, 90, 180, 270)

    public Tile(TileType type, int x, int z, TileDirection direction, int rotation = 0)
    {
      Type = type;
      Direction = direction;
      X = x;
      Z = z;
      Rotation = rotation;
    }
  }
}
