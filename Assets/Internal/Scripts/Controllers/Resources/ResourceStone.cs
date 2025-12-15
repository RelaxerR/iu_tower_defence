using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  /// <summary>
  /// Класс ресурса камня, наследующий базовую функциональность ресурса
  /// </summary>
  [RequireComponent(typeof(Renderer))]
  public class ResourceStone : ResourceBasic
  {
    /// <summary>
    /// Возвращает идентификатор ресурса камня
    /// </summary>
    public override string Id { get => ResourceSettings.StoneId; }
  }
}
