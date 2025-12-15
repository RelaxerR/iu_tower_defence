using System;
using UnityEngine;

namespace Internal.Scripts.Controllers.Buildings
{
  [Serializable]
  public class ResourceCost
  {
    public string resourceId; // Например, "wood", "stone", "diamond"
    public int amount;        // Количество требуемого ресурса
  }
}
