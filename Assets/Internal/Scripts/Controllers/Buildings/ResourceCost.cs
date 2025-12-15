using System;
using UnityEngine;

namespace Internal.Scripts.Controllers.Buildings
{
  /// <summary>
  /// Класс, представляющий стоимость ресурса для строительства или улучшения здания
  /// </summary>
  [Serializable]
  public class ResourceCost
  {
    /// <summary>
    /// Идентификатор ресурса (например, "wood", "stone", "diamond")
    /// </summary>
    [Tooltip("Идентификатор ресурса (например, \"wood\", \"stone\", \"diamond\")")]
    public string resourceId;

    /// <summary>
    /// Количество требуемого ресурса
    /// </summary>
    [Tooltip("Количество требуемого ресурса")]
    public int amount;
  }
}
