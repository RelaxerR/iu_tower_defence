using UnityEngine;

namespace Internal.Scripts.Bootstrap.Settings
{
  [CreateAssetMenu(fileName = "Resource", menuName = "Settings/Resource", order = 0)]
  public class ResourceSettings : ScriptableObject
  {
    public string Id;
    [Min(1)]
    public int MinAmount = 3;
    public int MaxAmount = 7;
    public Color SelectedColor;
    public Color DefaultColor = Color.white;
    
    public const string TreeId = "Tree";
    public const string StoneId = "Stone";
    public const string DiamondId = "Diamond";
  }
}
