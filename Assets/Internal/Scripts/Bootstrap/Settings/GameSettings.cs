using UnityEngine;

namespace Internal.Scripts.Bootstrap.Settings
{
  [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings", order = 0)]
  public class GameSettings : ScriptableObject
  {
    public float DefaultTileSize = 5f;
    public int MapSize = 20; // Tiles
  }
}
