namespace Internal.Scripts.Models
{
  public class Tile
  {
    public enum TileType
    {
      Ground,
      Road,
      RoadCorner,
      Resource,
      Castle
    }

    public TileType Type;
    public int X;
    public int Z;
    public int Rotation; // Y Rotation in degrees (0, 90, 180, 270)

    public Tile(TileType type, int x, int z, int rotation = 0)
    {
      Type = type;
      X = x;
      Z = z;
      Rotation = rotation;
    }
  }
}
